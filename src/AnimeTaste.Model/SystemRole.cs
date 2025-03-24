using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    [SugarTable(TableDescription = "系统角色表")]
    public class SystemRole
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true, ColumnDescription = "角色id")]
        public int Id { get; set; }

        [SugarColumn(ColumnDescription = "角色编号")]
        public string? RoleNo { get; set; }

        [SugarColumn(ColumnDescription = "角色名称")]
        public string? RoleName { get; set; }

    }
}
