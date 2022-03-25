using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;

namespace DisplayControlFlyout.Services
{
    public class Television
    {
        private static Timer _tvRefreshTimer = new Timer();
        private static bool _tvIsOn;
        private static Queue<bool> _testTvResults = new Queue<bool>();

        static Television()
        {
            _tvRefreshTimer.Interval = 1000;
            _tvRefreshTimer.Elapsed += _tvRefreshTimer_Elapsed;
        }

        public static void StartMonitor()
        {
            _tvRefreshTimer.Start();
        }

        public static void StopMonitor()
        {
            _tvRefreshTimer.Stop();
        }

        public static void SetPowerOnState(bool powerOnState)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string service = "send_command";
            string postData = "{ \"entity_id\": \"remote.control_remoto_dormitorio_pequeno_remote\" , \"device\": \"" + "lg_tv" + "\" , \"command\": \"" + "turn_" + (powerOnState ? "on" : "off") + "\"}";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://192.168.1.53:8123/api/services/remote/{0}", service));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers["Authorization"] = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI2YmIwNTM3M2I3MTY0MGQyOTc3YWQwMzNlNmVmYTk2MiIsImlhdCI6MTU5MjI1OTg2MSwiZXhwIjoxOTA3NjE5ODYxfQ.-VcGCT-aztII480YwTErOEKk1fmEpajvizLlyalbSL0";
            httpWebRequest.Method = "POST";
            //httpWebRequest.KeepAlive = false;
            httpWebRequest.Timeout = 1000;

            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentLength = (long)bytes.Length;

            using Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            requestStream.Dispose();
            using (httpWebRequest.GetResponse())
            {
                //nada
            }
        }

        public static bool SetInputBByIndex(int input)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string service = "send_command";
            string postData = "{ \"entity_id\": \"remote.control_remoto_dormitorio_pequeno_remote\" , \"device\": \"" + "lg_tv" + "\" , \"command\": \"" + "set_hdmi" + (input) + "\"}";


            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://192.168.1.53:8123/api/services/remote/{0}", service));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers["Authorization"] = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI2YmIwNTM3M2I3MTY0MGQyOTc3YWQwMzNlNmVmYTk2MiIsImlhdCI6MTU5MjI1OTg2MSwiZXhwIjoxOTA3NjE5ODYxfQ.-VcGCT-aztII480YwTErOEKk1fmEpajvizLlyalbSL0";
            httpWebRequest.Method = "POST";
            //httpWebRequest.KeepAlive = false;
            httpWebRequest.Timeout = 1000;
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentLength = (long)bytes.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            requestStream.Dispose();
            try
            {
                using (httpWebRequest.GetResponse()) { }
                return true;
            }
            catch
            {
                return false;
            }

        }

        private static void _tvRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var testResult = Test();
            _testTvResults.Enqueue(testResult);
            if (_testTvResults.Count > 3)
                _testTvResults.Dequeue();

            _tvIsOn = _testTvResults.Contains(true) || testResult;
        }

        private static bool Test()
        {
            return Network.PingHost("192.168.1.37") || Network.CheckIfPortOsOpen("192.168.1.37", 9998, 500, 2);
        }

        public static bool IsOn => _tvIsOn;

        public static void Dispose()
        {
            _tvRefreshTimer.Dispose();
        }
    }
}
