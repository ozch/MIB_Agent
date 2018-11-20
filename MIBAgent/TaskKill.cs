using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class TaskKill
    {
        public string KillProcess(int process_id){
            try
            {
                ManagementObject classInstance = 
                    new ManagementObject("root\\CIMV2", 
                    "Win32_Process.Handle='"+process_id+"'",
                    null);

                ManagementBaseObject inParams = classInstance.GetMethodParameters("Terminate");

                ManagementBaseObject outParams =  classInstance.InvokeMethod("Terminate", inParams, null);

                return outParams["ReturnValue"].ToString();
            }
            catch(ManagementException err)
            {
                return "-1";
            }
        }
        public void KillByName(string str)
        {
            PowerShellExecutor pse = new PowerShellExecutor();
            string str2 = string.Format("taskkill /F /IM {0} /T", str);
            string val= pse.RunShellScript(str2);
        }
    }
}
