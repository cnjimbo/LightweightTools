namespace Runner
{
    using System;
    using System.Diagnostics;
    using System.Security.Principal;

    using CommandLine;

    /// <summary>
    ///     Class WindowsSecurityUtil
    /// </summary>
    public static class WindowsSecurityUtil
    {
        /// <summary>
        ///     Runs as administrator.
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="executablePath">The executable path.</param>
        /// <returns>PermissionCheck.</returns>
        internal static PermissionCheck RunAsAdministrator(IOptions opts, string executablePath)
        {
            var identity = WindowsIdentity.GetCurrent();
            if (new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
            {
                return PermissionCheck.Administrator;
            }
            if (opts == null) throw new ArgumentNullException(nameof(opts));
            if (opts.IsSelfcall)
            {
                return PermissionCheck.NoAdministrator;
            }

            opts.IsSelfcall = true;
            //创建启动对象
            var startInfo = new ProcessStartInfo
                                {
                                    FileName = executablePath,
                                    Arguments = Parser.Default.FormatCommandLine(opts),
                                    Verb = "runas"
                                };
            //设置运行文件

            //设置启动参数
            //设置启动动作,确保以管理员身份运行
            //如果不是管理员，则启动UAC
            Process.Start(startInfo);
            //退出
            return PermissionCheck.RestartTryGet;
        }
    }
}