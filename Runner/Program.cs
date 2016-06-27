using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    using System.Diagnostics;
    using System.Security;

    class Program
    {
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
        static void _Main(string[] args)
        {

            try
            {
                if (args.Length == 4)
                {
                    Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo(args[0])
                    };
                    process.StartInfo.Domain = args[1];
                    process.StartInfo.UserName = args[2];
                    process.StartInfo.Password = buildSecureString(args[3]);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.LoadUserProfile = true;
                    process.Start();
                }
                else
                {
                    //  MessageBox.Show("Usage: BetterRunAs.exe \"application\" \"domain\" \"userid\" \"password\"");
                }
            }
            catch (Exception exception)
            {
                // MessageBox.Show(exception.Message);
            }
        }
    }
}
