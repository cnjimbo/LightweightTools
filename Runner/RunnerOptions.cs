namespace Runner
{
    using System.Collections.Generic;

    using CommandLine.Text;

    // [Verb("")]
    internal class RunnerOptions : IOptions
    {
        [Usage]
        public static IEnumerable<Example> Usage
        {
            get
            {
                yield return
                    new Example(
                        "Example",
                        new RunnerOptions
                            {
                                FileName = "notepad",
                                UserName = "apac\\tangj15",
                                Domain = "",
                                Arguments = "\\p"
                            });
            }
        }

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        public string Arguments { get; set; }

        public bool IsSelfcall { get; set; }

        public string FileName { get; set; }
    }

    //  [Verb("Selfcall",HelpText="get administrator permission from self")]
    internal class SelfRunnerOptions : IOptions
    {
        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        public string Arguments { get; set; }

        public bool IsSelfcall { get; set; }

        public string FileName { get; set; }
    }
}