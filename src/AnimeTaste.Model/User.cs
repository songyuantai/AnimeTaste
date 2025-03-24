using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [SugarTable(TableDescription = "用户表")]
    public class User
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true, ColumnDescription = "用户id")]
        public int Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "用户编号")]
        public string UserNo { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(Length = 50, ColumnDescription = "用户名称")]
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(Length = 50, ColumnDescription = "用户密码")]
        public string? Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(Length = 100, ColumnDescription = "邮箱")]
        public string? Email { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
