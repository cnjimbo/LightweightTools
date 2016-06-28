namespace Runner
{
    using CommandLine;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Principal;

    interface IOptions
    {
        [Option('u', "username",
            SetName = "byUser",
            HelpText = "run the file with this account, please keep None/Empty if you want to run the file with an Adminisitrator account")]
        string UserName { get; set; }

        [Option('d', "domain",
            SetName = "byUser",
            HelpText = "domain of account")]
        string Domain { get; set; }

        [Option('p', "password",
            SetName = "byUser",
            HelpText = "password of account")]
        string Password { get; set; }

        [Option('a', "arguments", HelpText = "arguments of exe file")]
        string Arguments { get; set; }

        [Option('s', "selfcall", HelpText = "")]
        bool IsSelfcall { get; set; }

        [Value(0, MetaName = "exe file full path",
            HelpText = "full path, this is must be required and this file must exist.",
            Required = true)]
        string FileName { get; set; }
    }


    /// <summary>
    /// Enum PermissionCheck
    /// </summary>
    public enum PermissionCheck
    {
        /// <summary>
        /// 当前用户无法获取管理员权限
        /// </summary>
        NoAdministrator,
        /// <summary>
        /// 当前用户为管理员
        /// </summary>
        Administrator,
        /// <summary>
        /// 正在重启尝试获取管理员
        /// </summary>
        RestartTryGet,
    }
    /// <summary>
    /// Class WindowsSecurityUtil
    /// </summary>
    public static class WindowsSecurityUtil
    {

        /// <summary>
        /// Runs as administrator.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="executablePath">The executable path.</param>
        /// <returns>PermissionCheck.</returns>
        internal static PermissionCheck RunAsAdministrator(IOptions opts, string executablePath)
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity != null && new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
            {
                return PermissionCheck.Administrator;
            }
            if (opts != null && opts.IsSelfcall)
            {
                return PermissionCheck.NoAdministrator;
            }

            opts.IsSelfcall = true;
            //创建启动对象
            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = CommandLine.Parser.Default.FormatCommandLine(opts),
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