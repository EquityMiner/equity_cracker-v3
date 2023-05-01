using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace equity_cracker
{
    static class EquityThings
    {
        public static async Task DownloadFile(string url, string filePath)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    using (var content = response.Content)
                    {
                        var fileBytes = await content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(filePath, fileBytes);
                    }
                }
            }
        }

        public static void Recap()
        {

        }

        public class MyException : Exception
        {
            public MyException() : base() { }
            public MyException(string message) : base(message) { }
            public MyException(string message, Exception e) : base(message, e) { }

            private string strExtraInfo;
            public string ExtraErrorInfo
            {
                get
                {
                    return strExtraInfo;
                }

                set
                {
                    strExtraInfo = value;
                }
            }
        }
    }
}
