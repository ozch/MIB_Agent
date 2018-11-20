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
           return number;
       }
       public void SetNumber(int x)
       {
           number = x;
       }
       public string GetJson()
       {
           PowerShellExecutor pse = new PowerShellExecutor();
           string str = "{";
           int i = 0;
           string[] st = pse.RunShellScript("$nets = netstat -bano|select-string 'LISTENING|UDP'; foreach ($n in $nets)    {    $p = $n -replace ' +',' ';    $nar = $p.Split(' ');    $pname = $(Get-Process -id $nar[-1]).ProcessName;    $n -replace \"$($nar[-1])\",\"$($ppath) $($pname)\";     }").Split('\n');
           foreach (var line in st.Skip(4))
           {
               string str2 = Regex.Replace(line, @"\s+", ";");
               str2 = str2.Trim(';');
               str2 = str2.Replace(";LISTENING;", ";");
               string[] lb = str2.Split(';');
               string temp = string.Format(" \"{0}\":<\"type\":\"{1}\", \"address\":\"{2}\",\"process\":\"{3}\">,", i, lb[0], lb[1], lb[3]);
               str = str + temp;
               i++;
           }
           str = str + "}";
           SetNumber(i);
           string json = GetProcessedString(str);
           return json;
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
               //string temp = string.Format(" \"{0}\":<\"type\":\"{1}\", \"address\":\"{2}\",\"process\":\"{3}\">,", i, lb[0], lb[1], lb[3]);
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
   }
   
}
