namespace AnimeTaste.Service.Cache
{
    public class RedisOption
    {
        /// <summary>
        /// 配置键
        /// </summary>
        public const string KEY = "Redis";

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectString { get; set; } = "";
    }
}
