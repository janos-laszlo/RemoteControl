using System;
using System.Diagnostics;
using System.IO;

namespace RemoteControlService.ReceiverDevice
{
    static class CmdLineUtils
    {
        public static void InvokeCommandLineCommand(string arguments)
        {
            var process = new Process();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = arguments
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }
    }
}
