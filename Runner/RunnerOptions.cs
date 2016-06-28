namespace Runner
{
    using CommandLine;
    using CommandLine.Text;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;

   // [Verb("")]
    class RunnerOptions : IOptions
    {
        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        public string Arguments { get; set; }

        public bool IsSelfcall { get; set; }

        public string FileName { get; set; }



    }

  //  [Verb("Selfcall",HelpText="get administrator permission from self")]
    class SelfRunnerOptions : IOptions
    {
        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        public string Arguments { get; set; }

        public bool IsSelfcall { get; set; }

        public string FileName { get; set; }



    }



}
