namespace AnimeTaste.Core.Const
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public readonly struct Role
    {
        public static Dictionary<string, string> GetRoleDescDictonary()
            => new()
            {
                {
                    Admin, "管理员"
                },
                {
                    User, "一般用户"
                },
            };

        /// <summary>
        /// 管理员
        /// </summary>
        public const string Admin = "admin";

        /// <summary>
        /// 一般用户
        /// </summary>
        public const string User = "user";
    }
}
