using System;
using System.Net.NetworkInformation;

namespace webcrawler
{
    static class Helpers
    {
        static string s_httpProtocol = "http";
        static string s_httpsProtocol = "https";

        public static string FixUrlProtocolIfNeeded(string url)
        {
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                return new string("http://" + url);
            }

            return url;
        }

        public static int DefaultPortNumberByScheme(string scheme)
        {
            if (s_httpProtocol == scheme)
            {
                return 80;
            }

            if (s_httpsProtocol == scheme)
            {
                return 443;
            }

            throw new Exception("Undefined protocol. Unknown default port number");
        }

        public static string GetMacAddress()
        {
            string macAddresses = string.Empty;

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

        public static bool CompareUrls(Uri lhs, Uri rhs)
        {
            return ConvertUrlToCanonizedForm(lhs) == ConvertUrlToCanonizedForm(rhs);
        }

        static string ConvertUrlToCanonizedForm(Uri url)
        {
            string urlValue = url.ToString();

            if (urlValue.EndsWith("/"))
            {
                urlValue = urlValue.Remove(urlValue.Length - 1);
            }
            if (urlValue.StartsWith("http://www."))
            {
                urlValue = urlValue.Remove(0, 11);
            }
            if (urlValue.StartsWith("https://www."))
            {
                urlValue = urlValue.Remove(0, 12);
            }
            if (urlValue.StartsWith("http://"))
            {
                urlValue = urlValue.Remove(0, 7);
            }
            if (urlValue.StartsWith("https://"))
            {
                urlValue = urlValue.Remove(0, 8);
            }

            return urlValue;
        }
    }
}
