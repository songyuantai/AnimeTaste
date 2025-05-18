using AnimeTaste.Model.Const;
using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧表
    /// </summary>
    [SugarTable(TableDescription = "番剧表")]
    public class Anime
    {
        /// <summary>
        /// 番剧id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "番剧id")]
        public int Id { get; set; }

        /// <summary>
        /// 番剧名称
        /// </summary>
        [SugarColumn(ColumnDescription = "番剧名称")]
        public string? Name { get; set; }

        /// <summary>
        /// 番剧别名
        /// </summary>
        [SugarColumn(ColumnDescription = "季度名称")]
        public string? Alias { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDescription = "描述")]
        public string? Description { get; set; }

        /// <summary>
        /// 季度id
        /// </summary>
        [SugarColumn(ColumnDescription = "季度id")]
        public int? SeasonId { get; set; }

        /// <summary>
        /// 番剧状态
        /// </summary>
        [SugarColumn(ColumnDescription = "番剧状态")]
        public AnimeStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
