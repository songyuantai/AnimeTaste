using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧分类表
    /// </summary>
    [SugarTable(TableDescription = "番剧分类表")]
    public class AnimeGenre
    {
        /// <summary>
        /// 分类id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int GenresId { get; set; }

        /// <summary>
        /// 番剧id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int AnimeId { get; set; }
    }
}
