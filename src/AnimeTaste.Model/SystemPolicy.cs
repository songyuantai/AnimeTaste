using SqlSugar;

namespace AnimeTaste.Model
{
    [SugarTable(TableDescription = "系统策略表")]
    public class SystemPolicy
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int PolicyId { get; set; }

        [SugarColumn(ColumnDescription = "角色ID")]
        public int RoleId { get; set; }

        [SugarColumn(ColumnDescription = "策略编号")]
        public string? PolicyNo { get; set; }

        [SugarColumn(ColumnDescription = "策略名称")]
        public string? PolicyName { get; set; }
    }
}
