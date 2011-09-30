using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ReadOutput
{
    class Program
    {
        // 0 to 15, + 1
        static int currentColor = 0;
        static ConsoleColor getColor()
        {
            ConsoleColor output = ConsoleColor.Blue;
            Enum.TryParse<ConsoleColor>((currentColor + 1).ToString(), out output);

            currentColor = (currentColor + 1) % 15;

            return output;
        }

        static Dictionary<int, ConsoleColor> colors = new Dictionary<int, ConsoleColor>();

        public static String f1 = "C:/Official/CSharp/PrintHellows/PrintHellows/bin/Release/PrintHellows";
        public static String f2 = @"C:\dev\h2\aid\target\bin\webapp.bat";

        public static List<Process> processList = new List<Process>();

        public static void RunProcesses(List<String> pList)
        {
            foreach (String fileName in pList)
            {
                Process p = new Process();

                if (p.StartInfo.EnvironmentVariables.ContainsKey("REPO"))
                {
                    //    Console.WriteLine("current value: " + p.StartInfo.EnvironmentVariables["REPO"]);
                }
                else
                {
                    p.StartInfo.EnvironmentVariables.Add("REPO", @"C:\Users\niqbal\.m2\repository");

                }

                p.StartInfo.FileName = fileName;
                p.StartInfo.WorkingDirectory = (new FileInfo(fileName)).DirectoryName;

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                p.OutputDataReceived += new DataReceivedEventHandler(printData);
                p.ErrorDataReceived += new DataReceivedEventHandler(printData);

                processList.Add(p);

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();


            }
        }

        static void Main(string[] args)
        {
            List<String> pList = new List<String>();
            pList.Add(f1);
            pList.Add(f2);
            pList.Add(f1);

            RunProcesses(pList);
            Console.ReadLine();

            foreach (int pid in colors.Keys)
            {
                try
                {
                    Process.GetProcessById(pid).Kill();
                    Console.WriteLine("Killed process: " + pid);
                }
                catch (ArgumentException e)
                {
                    // eat it.
                }
            }
        }


        public static Object lockObject = new Object();

        public static void printData(object sender, DataReceivedEventArgs e)
        {
            // Critical section
            int pId = ((Process)sender).Id;

            if (!colors.ContainsKey(pId))
            {
                lock (lockObject)
                {
                    if (!colors.ContainsKey(pId))
                    {
                        colors.Add(pId, getColor());
                    }
                }
            }

            Console.ForegroundColor = colors[pId];
            if (String.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Process " + (int)colors[pId] + " exited.");
            }
            else
            {
                Console.WriteLine((int)colors[pId] + " " + e.Data);
            }
            Console.ResetColor();
        }
    }
}
