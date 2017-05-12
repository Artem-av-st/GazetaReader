using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace GazetaReaderFrontend.ViewModel
{
    class ViewModelServices
    {
        public static async Task<Stream> GetHttpResponseAsync(string httpUri)
        {
            var http = new HttpClient();
            Stream result=null;

            try
            {
                var response = await http.GetAsync(new Uri(httpUri));

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new HttpRequestException(response.StatusCode.ToString());

                result = await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException)
            {
                return null;
            }
            
            return result;
        }

        #region CheckInternetConnection
        public static bool CheckInternetConnection()
        {
            //возвращает true в случае наличия подключения к сети
            return InternetGetConnectedState(out _flags, 0U);
        }
        [DllImport("Wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetGetConnectedState(out INET_CONNECTION_STATE lpdwFlags, uint dwReserved);

        [Flags]
        enum INET_CONNECTION_STATE : uint
        {
            INTERNET_CONNECTION_CONFIGURED = 0x40,
            INTERNET_CONNECTION_LAN = 0x02,
            INTERNET_CONNECTION_MODEM = 0x01,
            INTERNET_CONNECTION_MODEM_BUSY = 0x08,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_PROXY = 0x04,
            INTERNET_RAS_INSTALLED = 0x10
        }

        static INET_CONNECTION_STATE _flags;
        #endregion

    }
}
 