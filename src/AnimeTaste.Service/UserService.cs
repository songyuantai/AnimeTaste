using AnimeTaste.Core;
using AnimeTaste.Core.Model;
using AnimeTaste.Model;
using AnimeTaste.ViewModel;
using SqlSugar;
using System.Security.Claims;

namespace AnimeTaste.Service
{
    /// <summary>
    /// 用户服务
    /// </summary>
    /// <param name="db"></param>
    public class UserService(ISqlSugarClient db)
    {
        /// <summary>
        /// 根据用户名和密码查询用户
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User?> GetUser(string userNo, string password)
        {
            if (string.IsNullOrEmpty(userNo) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            return await db.Queryable<User>().FirstAsync(m => m.UserNo == userNo && m.Password == password);
        }

        /// <summary>
        /// 获取用户所有认证信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            List<Claim> list = [];
            if (null == user) return [];

            var userRoleList = await db.Queryable<UserRole>()
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            var roleIdList = userRoleList.Select(m => m.SystemRoleId).ToArray();
            var systemRoleList = await db.Queryable<SystemRole>().Where(m => roleIdList.Contains(m.Id)).ToListAsync();

            list.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            list.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
            list.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));

            foreach (var role in systemRoleList)
            {
                if (!string.IsNullOrEmpty(role.RoleNo))
                    list.Add(new Claim(ClaimTypes.Role, role.RoleName ?? ""));
            }

            var policyList = await db.Queryable<SystemPolicy>().Where(m => roleIdList.Contains(m.RoleId)).ToListAsync();
            policyList
                .Select(m => m.PolicyNo)
                .Distinct()
                .ToList()
                .ForEach(m => list.Add(new Claim(m ?? "", "true")));

            return list;
        }

        public async Task<Result<bool>> RegisterAsync(UserRegisterVm registerVm)
        {
            var result = new Result<bool>();
            await Validate(registerVm, result);
            if (!result.IsSuccess)
                return result;
            var entity = GetUserInfo(registerVm);
            var identity = await db.Insertable(entity).ExecuteReturnIdentityAsync();

            return identity > 0 ? result.Ok("注册成果！") : result.Fail("注册失败，请重试！");
        }

        private async Task Validate(UserRegisterVm registerVm, Result<bool> result)
        {
            if (registerVm == null)
            {
                result.Fail("参数错误!");
                return;
            }

            if (string.IsNullOrEmpty(registerVm.UserNo))
            {
                result.Fail("账号不能为空!");
                return;
            }

            if (string.IsNullOrEmpty(registerVm.UserName))
            {
                result.Fail("姓名不能为空!");
                return;
            }

            if (string.IsNullOrEmpty(registerVm.Email))
            {
                result.Fail("邮箱不能为空!");
                return;
            }

            var exsits = await db.Queryable<User>()
                .Where(m => m.UserNo == registerVm.UserNo || m.Email == registerVm.Email)
                .AnyAsync();
            if (exsits)
            {
                result.Fail("账号或者邮箱已注册!");
                return;
            }

            result.Ok("校验成果！");
        }

        private User GetUserInfo(UserRegisterVm vm)
        {
            return new User
            {
                UserNo = vm.UserNo!,
                UserName = vm.UserName,
                Email = vm.Email,
                Password = vm.Password,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };

        }

    }
}
