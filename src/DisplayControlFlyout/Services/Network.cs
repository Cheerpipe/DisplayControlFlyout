using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DisplayControlFlyout.Services
{
    public static class Network
    {
        public static bool CheckIfPortOsOpen(string host, int port, int timeOut = 200, int maxRetryCount = 1)
        {
            var porIsOpen = false;

            var retryCount = 0;
            while (retryCount < maxRetryCount)
            {
                retryCount++;
                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    var result = socket.BeginConnect(host, port, null, null);
                    result.AsyncWaitHandle.WaitOne(timeOut);

                    if (socket.Connected)
                    {
                        socket.EndConnect(result);
                        porIsOpen = true;
                        break;
                    }

                    socket.Close();
                }
            }
            return porIsOpen;
        }
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            try
            {
                using (pinger = new Ping())
                {
                    PingReply reply = pinger.Send(nameOrAddress, 2000);
                    pingable = reply.Status == IPStatus.Success;
                    //Debug.WriteLine(nameOrAddress + " PingHost: " + (pingable));
                    return pingable;
                }
            }
            catch
            {
                //Debug.WriteLine("catch");
                return true;
            }
        }
    }
}
