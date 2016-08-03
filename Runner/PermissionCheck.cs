namespace Runner
{
    /// <summary>
    ///     Enum PermissionCheck
    /// </summary>
    public enum PermissionCheck
    {
        /// <summary>
        ///     当前用户无法获取管理员权限
        /// </summary>
        NoAdministrator,

        /// <summary>
        ///     当前用户为管理员
        /// </summary>
        Administrator,

        /// <summary>
        ///     正在重启尝试获取管理员
        /// </summary>
        RestartTryGet
    }
}