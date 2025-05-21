using AnimeTaste.Core.Const;
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
        [SugarColumn(ColumnDescription = "番剧别名")]
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
        /// 播出日期
        /// </summary>
        [SugarColumn(ColumnDescription = "播出日期")]
        public DateTime? AiringDate { get; set; }

        /// <summary>
        /// 播出结束时间
        /// </summary>
        [SugarColumn(ColumnDescription = "结束时间")]
        public DateTime? AiriedDate { get; set; }

        /// <summary>
        /// 每周播出日期
        /// </summary>
        [SugarColumn(ColumnDescription = "每周播出日期")]
        public int? BroadcastDay { get; set; }

        /// <summary>
        /// 每周播出时间
        /// </summary>
        [SugarColumn(ColumnDescription = "每周播出时间")]
        public string? BroadcastTime { get; set; }

        /// <summary>
        /// 集数
        /// </summary>
        [SugarColumn(ColumnDescription = "集数")]
        public int? EpisodeCount { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [SugarColumn(ColumnDescription = "评分")]
        public double? Scroe { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        [SugarColumn(ColumnDescription = "排名")]
        public int? Rank { get; set; }

        /// <summary>
        /// 评级
        /// </summary>
        [SugarColumn(ColumnDescription = "评级")]
        public string? Rating { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
