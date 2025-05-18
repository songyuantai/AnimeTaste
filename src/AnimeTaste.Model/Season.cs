using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧季度
    /// </summary>
    [SugarTable(TableDescription = "番剧季度表")]
    public class Season
    {
        /// <summary>
        /// 季度id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 季度名称
        /// </summary>
        [SugarColumn(ColumnDescription = "季度名称")]
        public string? Name { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [SugarColumn(ColumnDescription = "年份")]
        public int Year { get; set; }

        /// <summary>
        /// 一年中的季度名春夏秋冬（"summer" "winter" "spring" "fall"）
        /// </summary>
        [SugarColumn(ColumnDescription = "一年中的季度名")]
        public string? SeasonOfYear { get; set; }

        /// <summary>
        /// 季度号 1234
        /// </summary>
        [SugarColumn(ColumnDescription = "季度号")]
        public int SeasonNumber { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnDescription = "开始时间")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnDescription = "结束时间")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateDate { get; set; }
    }
}
