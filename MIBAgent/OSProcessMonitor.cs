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
        //Return String All PrcessList and their usage in formated form
        public string GetString()
        {
            Console.WriteLine("Processing... Please Wait!");
            string str = "List Of Processes Running... \n ";
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                double[] info = GetProcessUsage(process);
                str = str + string.Format("{0} \t\t PID: {1} \t RAM:{2}MB \t CPU:{3}% \n ", process.ProcessName, process.Id, info[0], info[1]);
            }
            return str;
        }
        /*
         return Json file containing the process list with process name pid cpu usage in percent and ram uage in mbs for each process 
         Example
            {
              "0": {
                "pn": "dwm",
                "id": 984,
                "ru": "28MB",
                "cpu": "0"
              },
              "1": {
                "pn": "sqlservr",
                "id": 2756,
                "ru": "11MB",
                "cpu": "0"
              }
            }
         
         
         */
        public string GetJson()
        {
            int i = 0;
            string str = "{";
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {   
                double[] info = GetProcessUsage(process);
                string temp = string.Format(" \"{0}\":<\"pn\":\"{1}\", \"id\":{2},\"ru\":\"{3}MB\",\"cpu\":\"{4}\">,", i, process.ProcessName, process.Id, info[0], info[1]);
                i++;
                str = str + temp;
            }
            str = str + "}";
            string json = GetProcessedString(str);
            return json;
        }
        /*Return a process json string by replacing < > with { } respectively and removing last ,
        Example : 
         Input  = <"ip"="123","id"="2", >
         Output = {"ip"="123","id"="2"}
         */
        public string GetProcessedString(string p)
        {
            
            p = p.Replace("<", "{");
            p = p.Replace(">", "}");
            int place = p.LastIndexOf(",");
            p = p.Remove(place, 1);
            return p;
        }
        
        public double[] GetProcessUsage(Process p)
        {
            //Process p = new Process();
            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            System.Threading.Thread.Sleep(10);
            double ram = Math.Round(ramCounter.NextValue() / 1024 / 1024, 0);
            double cpu = Math.Round(cpuCounter.NextValue(),0);
            double[] info = {ram,cpu};
            //Console.WriteLine("RAM: "+(ram/1024/1024)+" MB; CPU: "+(cpu)+" %");
            return info;
        }
    }
}
