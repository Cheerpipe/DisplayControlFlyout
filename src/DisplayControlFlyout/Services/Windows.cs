using System.Diagnostics;

namespace DisplayControlFlyout.Services
{
    public class Windows
    {

        public static void Run(string commandLine, string arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = commandLine;
            startInfo.Arguments = arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden; //Hides GUI
            startInfo.CreateNoWindow = true; //Hides console
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
