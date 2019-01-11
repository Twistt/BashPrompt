using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CommandLine.DNC
{
    class Program
    {
        private static StreamWriter stdIn;
        private static Process p;
        private static string Command = "";
        static void Main(string[] args)
        {
            p = new Process();
            if (OperatingSystem.IsWindows()) p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            if (OperatingSystem.IsLinux()) p.StartInfo.FileName = @"/bin/bash";


            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            stdIn = p.StandardInput;
            p.OutputDataReceived += Process_OutputDataReceived;
            p.ErrorDataReceived += Process_OutputDataReceived;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            while (Command != "quit")
            {
                Console.Write("do stuff->");
                Command = Console.ReadLine();

                stdIn.WriteLine(Command);
            }
        }

        private static void Process_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data == null)
                return;
            else
            {
                Console.WriteLine(outLine.Data);
            }
        }
        ///
        /// Synchronously closes the command promp.
        /// 

        public void SyncClose()
        {
            stdIn.WriteLine("exit");
            p.WaitForExit();
            p.Close();
        }
        ///
        /// Asynchronously closees the command prompt.
        /// 

        public void AsyncClose()
        {
            stdIn.WriteLine("exit");
            p.Close();
        }

    }


    public static class OperatingSystem
        {
            public static bool IsWindows() =>
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            public static bool IsMacOS() =>
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            public static bool IsLinux() =>
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

}
