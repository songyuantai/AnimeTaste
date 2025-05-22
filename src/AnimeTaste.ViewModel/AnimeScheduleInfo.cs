namespace AnimeTaste.ViewModel
{
    /// <summary>
    /// 番剧日程信息
    /// </summary>
    public class AnimeScheduleInfo
    {
        /// <summary>
        /// 番剧id
        /// </summary>
        public int AnimeId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 评分
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 播出日期（周几）
        /// </summary>
        public int DayOfWeek { get; set; }

    }
}
