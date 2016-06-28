using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    using CommandLine;
    using CommandLine.Text;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security;

    class Program
    {
        public static void Main(string[] args)
        {
            //Debugger.Launch();
            Func<IOptions, string> SelfcallSuccessHandler = opts =>
                {
                    if (!string.IsNullOrWhiteSpace(opts.FileName) && File.Exists(opts.FileName))
                    {
                        var startInfo = new ProcessStartInfo { FileName = opts.FileName, };
                        if (!string.IsNullOrEmpty(opts.UserName))
                        {
                            if (string.IsNullOrWhiteSpace(opts.Domain))
                            {
                                var idx = opts.UserName.IndexOf('@');
                                if (idx > -1)
                                {
                                    startInfo.Domain = opts.UserName.Substring(idx + 1);
                                    startInfo.UserName = opts.UserName.Substring(0, idx);
                                }
                                else if ((idx = opts.UserName.IndexOf('\\')) > -1)
                                {
                                    startInfo.Domain = opts.UserName.Substring(0, idx);
                                    startInfo.UserName = opts.UserName.Substring(idx + 1);
                                }
                                else
                                {
                                    startInfo.UserName = opts.UserName;
                                }
                            }
                            else
                            {
                                startInfo.Domain = opts.Domain;
                                startInfo.UserName = opts.UserName;
                            }

                            startInfo.Password = buildSecureString(opts.Password);
                        }
                        else
                        {
                            var status = WindowsSecurityUtil.RunAsAdministrator(
                                opts,
                                executablePath: Assembly.GetExecutingAssembly().Location);
                            if (status== PermissionCheck.NoAdministrator)
                            {
                                return (" fail to try get administrator permission ");
                            }else if (status == PermissionCheck.RestartTryGet) return "";
                        }

                        if (!string.IsNullOrEmpty(opts.Arguments)) startInfo.Arguments = opts.Arguments;
                        startInfo.UseShellExecute = false;
                        startInfo.LoadUserProfile = true;

                        var p = new Process();
                        p.StartInfo = startInfo;

                        p.Start();

                        return "";
                    }
                    else
                    {
                        return ($"file must exist. '{opts.FileName}'");
                        //Console.ReadLine();
                        // exe file must be existing.
                    }
                    ;
                };



            var parser = new Parser(with => with.HelpWriter = Console.Out);

            var result = parser.ParseArguments<RunnerOptions>(args);

            var outMessage = result.MapResult<RunnerOptions, string>(
                SelfcallSuccessHandler,
                errors =>
                    {
                        return string.Join(Environment.NewLine, errors.Select(x => x.Tag.ToString()))
                               + Environment.NewLine + HelpText.RenderUsageText(result);
                    });

            if (!string.IsNullOrEmpty(outMessage))
            {
                Console.WriteLine(outMessage);
                Console.ReadLine();
            }
        }
        private static SecureString buildSecureString(string strIn)
        {
            SecureString str = new SecureString();
            foreach (char ch in strIn.ToCharArray())
            {
                str.AppendChar(ch);
            }
            str.MakeReadOnly();
            return str;
        }
    }
}
