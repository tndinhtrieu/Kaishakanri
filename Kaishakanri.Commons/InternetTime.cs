using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Kaishakanri.Commons
{
    public class InternetTime
    {
        private static IPAddress timeServerIpAddress = IPAddress.Parse("207.200.81.113");
        private static char[] SeparatorArray = new char[] { ' ' };
        private static byte[] buffer = new byte[256];
        private static System.Globalization.CultureInfo EnglishUSACulture = new System.Globalization.CultureInfo("ja-JP");

        /// <summary>
        /// Get NIST time, include delay time by slow connection
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNistTime()
        {
            DateTime start = DateTime.Now;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(timeServerIpAddress, 13);
            socket.Connect(hostEndPoint);
            int numberOfBytes = socket.Receive(buffer);
            if (numberOfBytes == 51)
            {
                string daytimeString = ASCIIEncoding.ASCII.GetString(buffer, 0, numberOfBytes).Trim();
                DateTime datetime = ParseDaytimeProtocol(daytimeString);
                TimeSpan ts = DateTime.Now - start;
                int ellapse = (int)ts.Ticks / 10000;
                datetime.AddMilliseconds(ellapse);
                return datetime;
            }
            return DateTime.Now;
        }

        private static DateTime ParseDaytimeProtocol(string daytimeString)
        {
            string[] resultTokens = daytimeString.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries);
            if (resultTokens[7] != "UTC(NIST)" || resultTokens[8] != "*")
            {
                throw new ApplicationException(string.Format("Invalid RFC-867 daytime protocol string: '{0}'", daytimeString));
            }
            int mjd = int.Parse(resultTokens[0]); // "JJJJ is the Modified Julian Date (MJD). The MJD has a starting point of midnight on November 17, 1858."
            DateTime now = new DateTime(1858, 11, 17);
            now = now.AddDays(mjd);

            string[] timeTokens = resultTokens[2].Split(':');
            int hours = int.Parse(timeTokens[0]);
            int minutes = int.Parse(timeTokens[1]);
            int seconds = int.Parse(timeTokens[2]);
            double millis = double.Parse(resultTokens[6], EnglishUSACulture);

            now = now.AddHours(hours);
            now = now.AddMinutes(minutes);
            now = now.AddSeconds(seconds);
            now = now.AddMilliseconds(-millis);
            return now;
        }
    }
}
