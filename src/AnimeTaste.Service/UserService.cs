using AnimeTaste.Model;
using SqlSugar;
using System.Security.Claims;

namespace AnimeTaste.Service
{
    public class UserService(ISqlSugarClient db)
    {
        public async Task<User?> GetUser(string userNo, string password)
        {
            if (string.IsNullOrEmpty(userNo) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            return await db.Queryable<User>().FirstAsync(m => m.UserNo == userNo && m.Password == password);
        }

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
    }
}
