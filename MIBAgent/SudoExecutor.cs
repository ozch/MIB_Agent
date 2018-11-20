using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Management.Automation;
using System.Collections.ObjectModel;

/*
 * this commnad executor runs command through cmd on client system
 */
namespace MIBAgent
{
    class SudoExecutor
    {
        Process process;
        public SudoExecutor()
        {
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }
        public string RunCommand(string cmd)
        {
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            //process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            return output;
        }
       
    }
    class PowerShellExecutor
    {
        Process process;
        public PowerShellExecutor()
        {
            process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }
        public string RunShellScript(string cmd)
        {
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            //process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            return output;
        }

    }
}
