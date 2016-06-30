namespace Runner
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    using CommandLine;
    using CommandLine.Text;

    internal class Program
    {
        private static readonly Func<IOptions, OkErrorString> SelfcallSuccessHandler = opts =>
            {
                if (!string.IsNullOrWhiteSpace(opts.FileName) && File.Exists(opts.FileName))
                {
                    var status = WindowsSecurityUtil.RunAsAdministrator(opts, Assembly.GetExecutingAssembly().Location);
                    if (status == PermissionCheck.RestartTryGet) return OkErrorString.OK;
                    var startInfo = new ProcessStartInfo { FileName = opts.FileName };
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
                        if (status == PermissionCheck.NoAdministrator)
                        {
                            return OkErrorString.Error(" fail to try get administrator permission ");
                        }
                    }

                    if (!string.IsNullOrEmpty(opts.Arguments)) startInfo.Arguments = opts.Arguments;
                    startInfo.UseShellExecute = false;
                    startInfo.LoadUserProfile = true;

                    var p = new Process();
                    p.StartInfo = startInfo;

                    p.Start();

                    return OkErrorString.OK;
                }
                return OkErrorString.Error($"file must exist. '{opts.FileName}'");
                //Console.ReadLine();
                // exe file must be existing.
                ;
            };

        public static void Main(string[] args)
        {
            var parser = Parser.Default;
            var result = parser.ParseArguments<RunnerOptions>(args);
            try
            {
                var outMessage = result.MapResult(
                    SelfcallSuccessHandler,
                    errors =>
                    OkErrorString.Error(string.Join(Environment.NewLine, errors.Select(x => x.Tag.ToString()))));

                if (outMessage.State == OkError.Error)
                {
                    Console.WriteLine(outMessage);
                    Console.WriteLine(HelpText.RenderUsageText(result));
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(HelpText.AutoBuild(result));
                Console.ReadLine();
            }
        }

        private static SecureString buildSecureString(string strIn)
        {
            return strIn.Aggregate(
                new SecureString(),
                (result, cur) =>
                    {
                        result.AppendChar(cur);
                        return result;
                    });
        }
    }
}