using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace NetworkServices
{
    public static class NetworkMethods
    {
        public static bool CheckInternetConnection()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 2000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
}
