using SqlSugar;

namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧图片
    /// </summary>
    [SugarTable(TableDescription = "番剧图片表")]
    public class AnimeImage
    {
        /// <summary>
        /// 番剧图片id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "番剧图片id")]
        public int Id { get; set; }

        /// <summary>
        /// 番剧id
        /// </summary>
        [SugarColumn(ColumnDescription = "番剧id")]
        public int AnimeId { get; set; }

        /// <summary>
        /// 图片类型
        /// </summary>
        [SugarColumn(ColumnDescription = "图片类型")]
        public string? ImageType { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        [SugarColumn(ColumnDescription = "文件后缀")]
        public string? Suffix { get; set; }

        /// <summary>
        /// 远程地址
        /// </summary>
        [SugarColumn(ColumnDescription = "远程地址")]
        public string? RemoteUrl { get; set; }

        /// <summary>
        /// 存储地址
        /// </summary>
        [SugarColumn(ColumnDescription = "存储地址")]
        public string? StorageUrl { get; set; }

        /// <summary>
        /// 存储类型
        /// </summary>
        [SugarColumn(ColumnDescription = "存储类型")]
        public string? StorageType { get; set; }
    }
}
