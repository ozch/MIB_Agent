using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
/*
 * all these functions return string
 * Return 0  # Function executed successfully
 * Return -1 # Exception not handled
 * Return 2  # Please this program with admin priviledges 
 * Return 6  # Service not Found
 */
namespace MIBAgent
{
    class ServiceController
    {
        public string StartService(string sv_name)
        {
            try
            {
                ManagementObject classInstance = new ManagementObject("root\\CIMV2", "Win32_Service.Name='" + sv_name + "'", null);
                ManagementBaseObject outParams = classInstance.InvokeMethod("StartService", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException err)
            {
                return "-1";
            }
        }
        public string StopService(string sv_name)
        {
            try
            {
                ManagementObject classInstance = new ManagementObject("root\\CIMV2", "Win32_Service.Name='" + sv_name + "'", null);
                ManagementBaseObject outParams = classInstance.InvokeMethod("StopService", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException err)
            {
                return "-1";
            }
        }
        public string DeleteService(string sv_name)
        {
            try
            {
                ManagementObject classInstance = new ManagementObject("root\\CIMV2", "Win32_Service.Name='" + sv_name + "'", null);
                ManagementBaseObject outParams = classInstance.InvokeMethod("Delete", null, null);
                return outParams["ReturnValue"].ToString();
            }
            catch (ManagementException e)
            {
                return "-1";
            }
        }
    }
}
