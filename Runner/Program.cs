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
        private const string Usage = @"  exe file full path (pos. 0)    Required. full path, this is must be required
                                 and this file must exist.

  -u, --username                 run the file with this account, please keep
                                 None/Empty if you want to run the file with an
                                 Adminisitrator account

  -d, --domain                   domain of account

  -p, --password                 password of account

  -a, --arguments                arguments of exe file

  --help                         Display this help screen.";

        private static readonly Func<IOptions, RunState> SelfcallSuccessHandler = opts =>
            {
                if (!string.IsNullOrWhiteSpace(opts.FileName) && File.Exists(opts.FileName))
                {
                    var status = WindowsSecurityUtil.RunAsAdministrator(opts, Assembly.GetExecutingAssembly().Location);
                    if (status == PermissionCheck.RestartTryGet) return RunState.OK;
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

                        startInfo.Password = BuildSecureString(opts.Password);
                    }
                    else
                    {
                        if (status == PermissionCheck.NoAdministrator)
                        {
                            return RunState.Fail(" fail to try get administrator permission ");
                        }
                    }

                    if (!string.IsNullOrEmpty(opts.Arguments)) startInfo.Arguments = opts.Arguments;
                    startInfo.UseShellExecute = false;
                    startInfo.LoadUserProfile = true;
                    //startInfo.RedirectStandardError = true;
                    //startInfo.RedirectStandardOutput = true;
                    using (var p = new Process { StartInfo = startInfo })
                    {
                        p.Start();
                        p.WaitForExit();
                        if (p.ExitCode == 0)
                            return RunState.OK;
                        //                        var str = $@"{p.StandardOutput.ReadToEnd()}
                        //{p.StandardOutput.ReadToEnd()}";

                        // return RunState.Fail(str);
                        return RunState.Fail("Unkown ERROR");
                    }
                }
                return RunState.Fail($"file must exist. '{opts.FileName}'");
            };

        public static int Main(string[] args)
        {
            Console.Title = "Run a file -- Copyright © 2016 Power by Tang jingbo";
            Console.WriteLine("Copyright © 2016 Power by Tang jingbo");

            var parser = Parser.Default;

            try
            {
                var result = parser.ParseArguments<RunnerOptions>(args);
                var outMessage = result.MapResult(
                    SelfcallSuccessHandler,
                    errors =>
                    RunState.ParseError(string.Join(Environment.NewLine, errors.Select(x => x.Tag.ToString()))));

                if (outMessage.State == State.ParseError)
                {
                    Console.WriteLine(outMessage.Message);
                    return 1;
                }
                if (outMessage.State == State.Fail)
                {
                    Console.WriteLine(outMessage.Message);
                    Console.WriteLine(Usage);
                    return 1;
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return 1;
            }
        }

        private static SecureString BuildSecureString(string strIn)
        {
            var seed = new SecureString();
            if (string.IsNullOrEmpty(strIn)) return seed;
            return strIn.Aggregate(
                seed,
                (result, cur) =>
                    {
                        result.AppendChar(cur);
                        return result;
                    });
        }
    }
}