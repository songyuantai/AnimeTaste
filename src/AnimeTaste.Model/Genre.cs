using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 分类表
    /// </summary>
    [SugarTable(TableDescription = "分类表")]
    public class Genre
    {
        /// <summary>
        /// 分类Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "分类Id")]
        public int Id { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [SugarColumn(ColumnDescription = "类别名称")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnDescription = "类型")]
        public string? Type { get; set; }

        /// <summary>
        /// 类别别名
        /// </summary>
        [SugarColumn(ColumnDescription = "类别别名")]
        public string? Alias { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(ColumnDescription = "地址")]
        public string? Url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateDate { get; set; }
    }
}
