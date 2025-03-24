namespace AnimeTaste.Core.Const
{
    public readonly struct Policy
    {
        public static List<string> GetPolicyList() => [
            ADD_ROLE,
            EDIT_ROLE,
            VIEW_ROLE,
            DEL_ROLE];

        public const string ADD_ROLE = nameof(ADD_ROLE);

        public const string EDIT_ROLE = nameof(EDIT_ROLE);

        public const string VIEW_ROLE = nameof(VIEW_ROLE);

        public const string DEL_ROLE = nameof(DEL_ROLE);
    }
}
