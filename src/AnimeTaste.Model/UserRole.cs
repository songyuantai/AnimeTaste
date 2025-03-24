using SqlSugar;

namespace AnimeTaste.Model
{
    [SugarTable(TableDescription = "用户角色表")]
    public class UserRole
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true, ColumnDescription = "用户角色id")]
        public int UserRoleId { get; set; }

        [SugarColumn(ColumnDescription = "角色id")]
        public int SystemRoleId { get; set; }

        [SugarColumn(ColumnDescription = "用户id")]
        public int UserId { get; set; }
    }
}
