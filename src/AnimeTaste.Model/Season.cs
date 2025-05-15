namespace AnimeTaste.Model
{
    /// <summary>
    /// 番剧季度
    /// </summary>
    public class Season
    {
        /// <summary>
        /// 季度id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 季度名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 一年中的季度名春夏秋冬（"summer" "winter" "spring" "fall"）
        /// </summary>
        public string? SeasonOfYear { get; set; }

        /// <summary>
        /// 季度号 1234
        /// </summary>
        public int SeasonNumber { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
