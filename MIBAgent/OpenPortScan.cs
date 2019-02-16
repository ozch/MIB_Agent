using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MIBAgent
{
   class OpenPortScan
   {
       int number = 0;
       public int GetNumber()
       {
           return this.number;
       }
       public void SetNumber(int x)
       {
           this.number = x;
       }
       public string GetJson()
       {
           PowerShellExecutor pse = new PowerShellExecutor();
           IDictionary<int, OpenPortModel> list = new Dictionary<int, OpenPortModel>();
           int i = 0;
           string[] st = pse.RunShellScript("$nets = netstat -bano|select-string 'LISTENING|UDP'; foreach ($n in $nets)    {    $p = $n -replace ' +',' ';    $nar = $p.Split(' ');    $pname = $(Get-Process -id $nar[-1]).ProcessName;    $n -replace \"$($nar[-1])\",\"$($ppath) $($pname)\";     }").Split('\n');
           
           foreach (var line in st.Skip(4))
           {
               //Todo : Issue where process name is not parsed correctly
               Console.WriteLine(line);
               string str2 = Regex.Replace(line, @"\s+", ";");
               str2 = str2.Trim(';');
               str2 = str2.Replace(";LISTENING;", ";");
               string[] lb = str2.Split(';');
                try
                {
                    list.Add(i, new OpenPortModel(lb[0], lb[1], lb[2]));
                }catch(Exception e)
                {
                    //Hahaha
                }
               i++;
           }
           
           SetNumber(i);
           return JsonConvert.SerializeObject(list);
       }
       public string GetPortProcessName(int X)
       {
           string proc = "-1";
           PowerShellExecutor pse = new PowerShellExecutor();
           string[] st = pse.RunShellScript("$nets = netstat -bano|select-string 'LISTENING|UDP'; foreach ($n in $nets)    {    $p = $n -replace ' +',' ';    $nar = $p.Split(' ');    $pname = $(Get-Process -id $nar[-1]).ProcessName;    $n -replace \"$($nar[-1])\",\"$($ppath) $($pname)\";     }").Split('\n');
           foreach (var line in st.Skip(4))
           {
               string str2 = Regex.Replace(line, @"\s+", ";");
               str2 = str2.Trim(';');
               str2 = str2.Replace(";LISTENING;", ";");
               string[] lb = str2.Split(';');
               string[] port = lb[1].Split(':');
               
               try{
                   if (Convert.ToInt16(port[1]) == X)
                       return lb[3];
               }
               catch(Exception e){
                   continue;
               }
           }
           
           return proc;
       }
       
   }
   
}
