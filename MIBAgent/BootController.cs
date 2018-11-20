using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class BootController
    {
        public string RestartWindows()
        {
            try
            {
                PowerShellExecutor pse = new PowerShellExecutor();
                pse.RunShellScript("shutdown /r");
                return "0";
                ManagementObject classInstance = new ManagementObject("root\\CIMV2","Win32_OperatingSystem.ReplaceKeyPropery='ReplaceKeyPropertyValue'",null);

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams = classInstance.InvokeMethod("Reboot", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException err)
            {
                return "-1";
            }
        }
        public string ShutdownWindows()
        {
            try
            {
                PowerShellExecutor pse = new PowerShellExecutor();
                pse.RunShellScript("shutdown /s");
                return "0";
                ManagementObject classInstance = new ManagementObject("root\\CIMV2", "Win32_OperatingSystem.ReplaceKeyPropery='ReplaceKeyPropertyValue'", null);

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams = classInstance.InvokeMethod("Shutdown", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException err)
            {
                return "-1";
            }
        }
        public string SleepWindows()
        {
            try
            {
                PowerShellExecutor pse = new PowerShellExecutor();
                pse.RunShellScript("shutdown /l");
                return "0";
                ManagementObject classInstance = new ManagementObject("root\\CIMV2", "Win32_OperatingSystem.ReplaceKeyPropery='ReplaceKeyPropertyValue'", null);

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams = classInstance.InvokeMethod("Sleep", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException err)
            {
                return "-1";
            }
        }
    }
}
