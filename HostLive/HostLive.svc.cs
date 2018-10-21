using Org.Apollo.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace HostLive
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HostLive" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HostLive.svc or HostLive.svc.cs at the Solution Explorer and start debugging.
    public class HostLive : IHostLive
    {
        private static int SystemContinuosOnCount = 1;
        private static DateTime LastRecordedTime;
        /// <summary>
        /// Set Entry point : First Route method which will check below things.
        /// 1. On System start we will insert log in to C:\ServiceLogs\HostLive.txt
        /// 2. Check if Internet connection is Live or not.
        /// 3. If Live => We will send a email to client that machine is started. If not we will skip this process.
        /// 4. We will check every 1 hour if system is still running then we will step back and execute step 1 and do continue other process.
        /// </summary>
        public bool Start()
        {
            bool IsDone = false;
            try
            {
                IsDone = StoreLogsInFileAndSendEmail();
            }
            catch(Exception e)
            {
                throw e;
            }

            return IsDone;
        }

        private bool StoreLogsInFileAndSendEmail()
        {
            bool IsDone = false;
            try
            {
                SystemDetails s1 = GetSystemDetails();
                LastRecordedTime = s1.PreciseTimeStamp;
                string systemLog = SerializeJSONData(s1);
                IsDone = HostFileUtility.Instance.WriteLog(systemLog);
                SendEmail(s1);
                IncrementCount();
            }
            catch(Exception e)
            {
                throw e;
            }

            return IsDone;
        }

        private void IncrementCount()
        {
            SystemContinuosOnCount++;
        }

        private bool SendEmail(SystemDetails s1)
        {
            //prepare to send an email with login creds

            string Subject = GetSubjectLine();
            string Body = GetBody(s1);

            Mail m = new Mail();
            m.Send("smtp.gmail.com", "amitpatange88@gmail.com", "password", "amitpatange88@gmail.com", Subject, Body);

            return true;
        }

        private string GetSubjectLine()
        {
            return string.Format("HostLive Notification : Your system is booted up and running since {0}+ hours.", SystemContinuosOnCount);
        }

        private string GetBody(SystemDetails s1)
        {
            string body = string.Empty;

            if (SystemContinuosOnCount == 1)
            {
                body += "Here are the system booted up details, This is first time system has started.<br>";
            }
            else
            {
                body += string.Format("This is booted up system is online since {0}+ hours. <br>", SystemContinuosOnCount);
            }

            body += "Here are system details : <br><br>";
            body += "<table width='100%' cellspacing='2' cellpadding='0' border='0' align='center' bgcolor='#ff6600'>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40' bgcolor='#ffdb99'>System Property</td>";
            body += "<td width='50%' bgcolor='#ffdb99'>Value</td>";
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width= '50%' height='40'>Precise DateTime</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.PreciseTimeStamp);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>UTC DateTime</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.UTCTimeStamp);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>MachineName</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.MachineName);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>Is64Bit OS</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.Is64BitOperatingSystem);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>OSVersion</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.OSVersion);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>ProcessorCount</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.PreciseTimeStamp);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>Hours Running</td>";
            body += string.Format("<td width='50%'>{0}</td>", s1.OnCount);
            body += "</tr>";
            body += "<tr bgcolor='#ffffff'>";
            body += "<td width='50%' height='40'>Running Processes</td>";
            body += string.Format("<td width='50%'>{0}</td>", ProcessesRunningAttachInEmail());
            body += "</tr>";
            body += "</table>";


            return body;
        }

        private SystemDetails GetSystemDetails()
        {
            SystemDetails s = new SystemDetails()
            {
                LogId = Guid.NewGuid(),
                PreciseTimeStamp = DateTime.Now,
                UTCTimeStamp = DateTime.UtcNow,
                MachineName = System.Environment.MachineName,
                OSVersion = System.Environment.OSVersion.ToString(),
                Is64BitOperatingSystem = System.Environment.Is64BitOperatingSystem,
                ProcessorCount = System.Environment.ProcessorCount,
                OnCount = SystemContinuosOnCount,
                ProcessesRunning = ProcessesRunningForLogs()
            };

            return s;
        }

        /// <summary>
        /// Serialize the JSON data. Using with Default contract resolver for non public fields.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string SerializeJSONData(SystemDetails e)
        {
            Newtonsoft.Json.JsonSerializerSettings jss = new Newtonsoft.Json.JsonSerializerSettings();

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            jss.ContractResolver = dcr;

            var response = Newtonsoft.Json.JsonConvert.SerializeObject(e, jss);

            return response;
        }

        private string ProcessesRunningAttachInEmail()
        {
            string processesRunning = string.Empty;
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    processesRunning += p.MainWindowTitle + "<br>-----------------------------------<br>";
                }
            }

            return processesRunning;
        }

        private string ProcessesRunningForLogs()
        {
            string processesRunning = string.Empty;
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    processesRunning += p.MainWindowTitle + "&nbsp;";
                }
            }

            return processesRunning;
        }
    }
}
