namespace BetterRunAs
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    interface IOptions
    {
        [Option('u', "user",
            SetName = "byUser",
            HelpText = "以此账号运行程序，账号可包含域名")]
       string User { get; set; }

        [Option('p', "password",
            SetName = "byUser",
            HelpText = "账号密码，如果仅提升为管理员权限请留空")]
        string Password { get; set; }


        [CommandLine.Text.Usage]
        string Useage { get; set; }

        [Value(0, MetaName = "exe file full path",
            HelpText = "执行文件",
            Required = true)]
        string FileName { get; set; }
    }

  
    class HeadOptions : IOptions
    {
        public string User { get; set; }

        public string Password { get; set; }

        public string FileName { get; set; }
         
        public string Useage { get; set; }
    }

    


    class Program
    {
        public static int Main(string[] args)
        {

            Func<IOptions, string> header = opts =>
            { 
                var builder = new StringBuilder("Reading ");

                builder.AppendLine(opts.FileName).AppendLine(opts.User).AppendLine(opts.Password);

                builder.AppendLine(opts.Useage);
                return builder.ToString();
            };

            var result = Parser.Default.ParseArguments<HeadOptions>(args);
            var texts = result.MapResult(header,_=> MakeError());
                
             

            return texts.Equals(MakeError()) ? 1 : 0;
        }

        private static object MakeError()
        {
            return string.Empty;
        }
    }


}
