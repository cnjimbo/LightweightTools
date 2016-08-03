namespace Runner
{
    using System.Collections.Generic;

    using CommandLine.Text;

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
                                FileName = "%windir%\\system32\\notepad.exe",
                                UserName = "apac\\tangj15",
                                Domain = ""
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
}