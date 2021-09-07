using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SensingSite.ClientSDK.Common
{
    public class MacIPHelper
    {

        //public string getLocalIP()
        //{
        //    string strHostName = Dns.GetHostName(); //得到本机的主机名
        //    IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
        //    string strAddr = ipEntry.AddressList[0].ToString();
        //    return (strAddr);
        //}

        /// <summary>
        /// 获取本机MAC地址
        /// </summary>
        /// <returns>MAC数据 string</returns>
        public static string GetClientMac()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>IP地址 string</returns>
        //public string GetClientIP()
        //{
        //    try
        //    {
        //        string ip = null;
        //        ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
        //        ManagementObjectCollection queryCollection = query.Get();
        //        foreach (ManagementObject mo in queryCollection)
        //        {
        //            if ((bool)mo["IPEnabled"] == true)
        //            {
        //                string[] IPAddresses = (string[])mo["IPAddress"];
        //                if (IPAddresses.Length > 0)
        //                    ip = IPAddresses[0];
        //            }
        //        }
        //        return (ip);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error:" + ex.Message;
        //    }
        //}

    }
}
