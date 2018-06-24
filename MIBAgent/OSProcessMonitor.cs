using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIBAgent
{
    //This class provides you with the method to monitor all the runnig processes in the operating system
    class OSProcessMonitor
    {
        static void Main(string[] args)
        {
            
        }
        //public string GetProcessUsageInfo()
        //{
        //    Process p = /*get the desired process here*/;
        //    PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
        //    PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
        //    while (true)
        //    {
        //    Thread.Sleep(500);
        //    double ram = ramCounter.NextValue();
        //    double cpu = cpuCounter.NextValue();
        //    Console.WriteLine("RAM: "+(ram/1024/1024)+" MB; CPU: "+(cpu)+" %");
        //    }
        //}
    }
}
