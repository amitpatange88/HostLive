using Org.Apollo.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MsgBroker = RabbitMQ;

namespace HostLive
{
    /// <summary>
    /// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HostLive" in code, svc and config file together.
    /// NOTE: In order to launch WCF Test Client for testing this service, please select HostLive.svc or HostLive.svc.cs at the Solution Explorer 
    /// and start debugging.
    /// </summary>
    public class HostLive : IHostLive
    {
        private static string FilePathSecrets = @"C:\ServiceLogs\keys\HostLiveSecrets.txt";
        private static int SystemContinuosOnCount = 0;
        private static DateTime LastRecordedTime;
        public delegate void _CallbackConsumerDel(string message);
        public static _CallbackConsumerDel _callback = new _CallbackConsumerDel(HostLive.DoOnConsumeMessages);

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

                if (Internet.IsConnectionActive())
                {
                    //Consume RabbitMQ messages here somewhere.
                    SendEmail(s1);
                }
                else
                {
                    //Call RabbitMQ as message broker.
                    using (MsgBroker.RabbitMQ Rpc = new MsgBroker.RabbitMQ())
                    {
                        Rpc.MessageBrokerPublish(systemLog);
                    }
                }

                IncrementCount();
            }
            catch(Exception e)
            {
                throw e;
            }

            return IsDone;
        }

        private string ReadServerSecrets()
        {
            string secrets = string.Empty;
            using (var fileStream = new FileStream(FilePathSecrets, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                secrets = streamReader.ReadToEnd();
            }

            return secrets;
        }

        public static void DoOnConsumeMessages(string message)
        {
            var details = DeserializeJSONData<SystemDetails>(message);
            Console.WriteLine(" [x] Received {0}", message);
        }

        private void IncrementCount()
        {
            SystemContinuosOnCount++;
        }

        private bool SendEmail(SystemDetails s1)
        {
            //prepare to send an email with login creds
            string Secrets = ReadServerSecrets();

            if (string.IsNullOrWhiteSpace(Secrets))
                return false;

            var Keys = DeserializeJSONData<Secrets>(Secrets);

            string Subject = GetSubjectLine();
            string Body = GetBody(s1);

            Mail m = new Mail();
            m.Send("smtp.gmail.com", Keys.HostLiveSecrets.Gmail.username, Keys.HostLiveSecrets.Gmail.password, Keys.HostLiveSecrets.Gmail.username, Subject, Body);

            return true;
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

        
        private string SerializeJSONData(SystemDetails e)
        {
            Newtonsoft.Json.JsonSerializerSettings jss = new Newtonsoft.Json.JsonSerializerSettings();

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            jss.ContractResolver = dcr;

            var response = Newtonsoft.Json.JsonConvert.SerializeObject(e, jss);

            return response;
        }

        /// <summary>
        /// Serialize the JSON data. Using with Default contract resolver for non public fields.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static dynamic DeserializeJSONData<T>(string message)
        {
            Newtonsoft.Json.JsonSerializerSettings jss = new Newtonsoft.Json.JsonSerializerSettings();

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            jss.ContractResolver = dcr;

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);

            return response;
        }

        private string ProcessesRunningAttachInEmail()
        {
            string processesRunning = string.Empty;
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in processes)
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
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    processesRunning += p.MainWindowTitle + "&nbsp;";
                }
            }

            return processesRunning;
        }

        private string GetSubjectLine()
        {
            string subject = string.Empty;
            if (SystemContinuosOnCount == 0)
            {
                subject += "HostLive Notification : Your system is just come online.";
            }
            else
            {
                subject += string.Format("HostLive Notification : Your system is running since {0}+ hours.", SystemContinuosOnCount);
            }

            return subject;
        }

        private string GetBody(SystemDetails s1)
        {
            string body = string.Empty;

            if (SystemContinuosOnCount == 0)
            {
                body += "Your system has just boot up. Would you mind take a look at this.<br>";
            }
            else
            {
                body += string.Format("Your system is online since {0}+ hours. <br>", SystemContinuosOnCount);
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
    }
}
