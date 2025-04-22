namespace AnimeTaste.ViewModel
{
    public class UserRegisterVm
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string? UserNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 重复密码
        /// </summary>
        public string? PasswordRepeat { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 系统角色id
        /// </summary>
        public int? SystemRoleId { get; set; }

    }
}
