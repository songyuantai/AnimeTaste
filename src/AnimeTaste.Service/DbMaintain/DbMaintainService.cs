using AnimeTaste.Core.Const;
using AnimeTaste.Model;
using SqlSugar;
using System.Reflection;

namespace AnimeTaste.Service.DbMaintain
{
    public class DbMaintainService(ISqlSugarClient db)
    {
        const string DLL = "AnimeTaste.Model.dll";

        public void InitTables()
        {
            //db.DbMaintenance.CreateDatabase("anime");

            var types = Assembly
                .LoadFrom(DLL)
                .GetTypes()
                .Where(t => !t.IsValueType || (t.IsValueType && !t.IsEnum))
                .ToArray();
            //.Where(it => it.FullName.Contains("OrmTest."))
            //.ToArray();

            db.CodeFirst.SetStringDefaultLength(200).InitTables(types);
        }

        public async Task SeedData()
        {
            await db.Fastest<SystemRole>().BulkCopyAsync([
                new SystemRole {
                    Id = 1,
                    RoleNo = Role.Admin,
                    RoleName = Role.GetRoleDescDictonary()[Role.Admin],
                },
                new SystemRole {
                    Id = 2,
                    RoleNo = Role.User,
                    RoleName = Role.GetRoleDescDictonary()[Role.User],
                },
            ]);

            await db.Fastest<SystemPolicy>().BulkCopyAsync([
                new SystemPolicy {
                    PolicyId = 1,
                    RoleId = 1,
                    PolicyNo = Policy.ADD_ROLE,
                    PolicyName = "新增角色",
                },
                new SystemPolicy {
                    PolicyId = 2,
                    RoleId = 1,
                    PolicyNo = Policy.EDIT_ROLE,
                    PolicyName = "编辑角色",
                },
                new SystemPolicy {
                    PolicyId = 3,
                    RoleId = 1,
                    PolicyNo = Policy.VIEW_ROLE,
                    PolicyName = "查看权限",
                },
                new SystemPolicy {
                    PolicyId = 4,
                    RoleId = 1,
                    PolicyNo = Policy.DEL_ROLE,
                    PolicyName = "删除权限",
                },
            ]);

            await db.Fastest<User>().BulkCopyAsync([
                new User {
                    Id = 1,
                    UserNo = "Admin",
                    UserName = "管理员",
                    Password = "admin",
                    Email = "admin@mail.com",
                },
                new User {
                    Id = 2,
                    UserNo = "User",
                    UserName = "用户",
                    Password = "user",
                    Email = "user@mail.com",
                },
            ]);

            //设置管理员角色
            await db.Fastest<UserRole>().BulkCopyAsync([
                new UserRole{
                    UserRoleId = 1,
                    UserId = 1,
                    SystemRoleId = 1,
                }
            ]);
        }

        public void DropTables()
        {
            Type[] types = Assembly
                .LoadFrom(DLL)
                .GetTypes();
            db.DbMaintenance.DropTable(types);
        }
    }
}
