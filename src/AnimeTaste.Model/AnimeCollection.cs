using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧收藏
    /// </summary>
    [SugarTable(TableDescription = "番剧收藏")]
    public class AnimeCollection
    {
        /// <summary>
        /// 收藏id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "收藏id")]
        public int Id { get; set; }

        /// <summary>
        /// 番剧id
        /// </summary>
        [SugarColumn(ColumnDescription = "番剧id")]
        public int AnimeId { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        [SugarColumn(ColumnDescription = "收藏时间")]
        public DateTime CollectDate { get; set; }
    }
}
