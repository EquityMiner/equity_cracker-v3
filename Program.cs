using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Threading.Tasks;
using Nethereum.Web3.Accounts;
using System.Security.Cryptography;
using System.Threading;

namespace equity_cracker
{
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

    internal static class Program
    {
        private static object  consoleLock  = new object();
        public  static Boolean runCode      = true;
        public  static int     hits         = 0;
        public  static int     checks       = 0;
        public  static int     proxyChangerValue = 0;
        public  static Boolean UseProxy     = Convert.ToBoolean(ConfigurationManager.AppSettings["UseProxy"]);
        public  static Boolean DebugOption  = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
        public  static int     Threads      = Convert.ToInt16(ConfigurationManager.AppSettings["threads"]);
        public  static string  cryptoToMine = Convert.ToString(ConfigurationManager.AppSettings["cryptoToMine"]);

        static int count = 0;
        static Boolean startedAtIDK = false;
        static Boolean MinerStarted = false;
        static DateTime startTimeX = DateTime.Now;

        public static Boolean  proxyChanger = Convert.ToBoolean(ConfigurationManager.AppSettings["proxyChanger"]);
        public static string   proxyChangerValueString = Convert.ToString(ConfigurationManager.AppSettings["proxyChangerValue"]);

        public static async Task Main()
        {
            if (cryptoToMine != "eth")
            {
                Console.WriteLine("Please use eth as CryptoToMine!");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Console.Title = "Equity cracker v3 - Hits: " + hits;

            try
            {
                proxyChangerValue = Int32.Parse(proxyChangerValueString);
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("proxyChangerValue needs to be a number! Check .config");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press enter to exit..");
                Console.ReadLine();
                Environment.Exit(0);
            }

            #region Show Settings
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Current settings:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("     UseProxy: ");
            if ( UseProxy == true ) { Console.ForegroundColor = ConsoleColor.Green; } else { Console.ForegroundColor = ConsoleColor.Red; }
            Console.WriteLine(UseProxy);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Proxy Changer: ");
            if (proxyChanger == true) { Console.ForegroundColor = ConsoleColor.Green; } else { Console.ForegroundColor = ConsoleColor.Red; }
            Console.WriteLine(proxyChangerValue);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" RPC endpoint: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("rpc.ankr.com*");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("       Crypto: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ethereum(eth)*");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("      Threads: ");
            if (Threads > 10) { Console.ForegroundColor = ConsoleColor.Red; } else { Console.ForegroundColor = ConsoleColor.Green; }
            Console.WriteLine(Threads);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("        Debug: ");
            if (DebugOption == true) { Console.ForegroundColor = ConsoleColor.Green; } else { Console.ForegroundColor = ConsoleColor.Red; }
            Console.WriteLine(DebugOption);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("* = can't be changed");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Press enter to start..");
            Console.ReadLine();
            #endregion

            lock (consoleLock) { Console.Clear(); }

            var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            
            string path = Path.GetFullPath(Path.Combine(exePath, @"..\..\hits.txt"));
            string proxyFilePath = Path.GetFullPath(Path.Combine(exePath, @"..\..\proxys.txt"));

            if (UseProxy == true)
            {
                if (!(File.Exists(proxyFilePath)))
                {
                    using (FileStream fs = File.Create(proxyFilePath)) { };
                    var lineCount = File.ReadLines(proxyFilePath).Count();
                    if (lineCount > 0)
                    {
                        Console.Write("Creating proxys.txt.. | ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You need to input proxy's!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Press enter to exit..");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!(File.Exists(path)))
            {
                Console.Write("Creating hits.txt.. | ");
                using (FileStream fs = File.Create(path)) { };
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("done.");
            }
            Console.WriteLine();

            Console.WriteLine("Starting miner.. Good Luck!");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Task[] tasks = new Task[Threads];

            int currentProxyIndex = 0;

            Thread t = new Thread(BackgroundTask);
            t.Start();

            for (int i = 0; i < Threads; i++)
            {
                tasks[i] = Task.Run( async () =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (!(File.Exists(proxyFilePath))) { lock (consoleLock) { Console.WriteLine(proxyFilePath + " can't be found!"); Console.ReadLine(); Environment.Exit(0); } }
                    Console.ForegroundColor = ConsoleColor.White;

                    string[] proxys = File.ReadAllLines(proxyFilePath);

                    MinerStarted = true;

                    while (true)
                    {
                        try
                        {
                            while(runCode)
                            {
                                if (startedAtIDK == false)
                                {
                                    startedAtIDK = true;
                                    startTimeX = DateTime.Now;
                                }
                                count++;
                                var currentProxy = proxys[currentProxyIndex];
                                currentProxyIndex++;
                                string url = "https://rpc.ankr.com/eth";
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                                var startTime = DateTime.Now;
                                var rng = new RNGCryptoServiceProvider();
                                var privateKeyBytes = new byte[32];
                                rng.GetBytes(privateKeyBytes);
                                var privateKey = BitConverter.ToString(privateKeyBytes).Replace("-", "").ToLower();
                                var endTime = DateTime.Now;
                                var duration = endTime - startTime;

                                var account = new Account(privateKey);
                                string address = account.Address;

                                HttpClientHandler handler = new HttpClientHandler();

                                HttpClient client = new HttpClient(handler);
                                client.Timeout = new TimeSpan(0, 0, 30);

                                string json = "{\"jsonrpc\":\"2.0\",\"method\":\"eth_getBalance\",\"params\":[" +
                                              $"\"{address}\",\"latest\"" +
                                              "],\"id\":1}";

                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                client.DefaultRequestHeaders.Connection.Clear();

                                HttpResponseMessage response = await client.PostAsync(url, content);


                                if (response.IsSuccessStatusCode)
                                {
                                    string responseString = response.Content.ReadAsStringAsync().Result;

                                    JObject responseJson = JObject.Parse(responseString);

                                    string balance = (string)responseJson["result"];

                                    var consoleColor = ConsoleColor.White;

                                    if (balance != "0x0")
                                    {
                                        consoleColor = ConsoleColor.Green;
                                        hits++;
                                        Console.Title = "Equity cracker v3 - Hits: " + hits;
                                        string textToSave = privateKey + " | Bal: " + balance;

                                        File.WriteAllText(path, textToSave);
                                    }
                                    else
                                    {
                                        consoleColor = ConsoleColor.Red;
                                    }

                                    var idk = count + "Private Key: " + privateKey;
                                    if (UseProxy == true)
                                    {
                                        idk = count + "Private Key: " + privateKey + " | Bal: " + balance;
                                    }

                                    Write(idk, consoleColor, duration.ToString(), "127.0.0.1");

                                    checks++;

                                    if (proxyChanger == true)
                                    {
                                        if (checks > proxyChangerValue)
                                        {
                                            Console.WriteLine("Changing proxy.."); checks = 0; MyException m;
                                            m = new MyException("Maximal checks reached");
                                            m.ExtraErrorInfo = "Maximal checks reached: (0)";
                                            throw m;
                                        }
                                    }

                                    if (currentProxyIndex >= proxys.Length) { currentProxyIndex = 0; }
                                }
                                else
                                {
                                    if (DebugOption == true) { Write("Failed getting balance | Status code: " + response.StatusCode, ConsoleColor.Red, "no", "no"); }
                                }
                            }
                        } 
                        catch (Exception e)
                        {
                            if (DebugOption == true) { Console.WriteLine("Exception - miner"); Console.WriteLine(e); }

                            if (currentProxyIndex >= proxys.Length) { currentProxyIndex = 0; }

                            continue;
                        }
                    }
                });
            }
            while (true)
            {
                await Task.WhenAll(tasks);
            }
        }

        static void BackgroundTask()
        {
            while(true)
            {
                if (MinerStarted)
                {
                    if (count >= 1500)
                    {
                        runCode = false;
                        lock (consoleLock) { Console.WriteLine("Stopped Miner. (This is an protection to not extend the ankr rate limited - DO NOT REPORT THAT AS AN ERROR!)"); }
                        TimeSpan elapsedTime = DateTime.Now - startTimeX;
                        double elapsedMilliseconds = elapsedTime.TotalMilliseconds;
                        if (elapsedMilliseconds < 0)
                        {
                            count = 0;
                            runCode = true;
                        } else
                        {
                            var timeoutTime = 60000 - elapsedMilliseconds; ;
                            var timeoutTimeInt = Convert.ToInt32(timeoutTime);
                            Thread.Sleep(timeoutTimeInt);
                            count = 0;
                            runCode = true;
                        }
                        
                    }
                }
            }
        }

        public static void Write(string text, ConsoleColor color, string duration, string proxy)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                if (duration != "no")
                {
                    Console.Write(text);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | Generation time: " + duration);
                    Console.WriteLine(" | Proxy: " + proxy);
                } else
                {
                    Console.WriteLine(text);
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    class ConfigHandler
    {

        private static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string configPath = Path.Combine(appDataPath, "EquityMiner", "settings.config");

        public static void main()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
            if (!File.Exists(configPath))
            {
                File.Create(configPath).Dispose();
                WriteConfig("UseProxy", "true");
                WriteConfig("debug", "false");
                WriteConfig("threads", "10");
                WriteConfig("cryptoToMine", "eth");
                WriteConfig("proxyChanger", "false");
                WriteConfig("proxyChangerValue", "500");
            }

            Program.UseProxy = Convert.ToBoolean(ReadConfig("UseProxy"));
            Program.DebugOption = Convert.ToBoolean(ReadConfig("debug"));
            Program.Threads = Convert.ToUInt16(ReadConfig("threads"));
            Program.cryptoToMine = ReadConfig("cryptoToMine");
            Program.proxyChanger = Convert.ToBoolean(ReadConfig("proxyChanger"));
            Program.proxyChangerValue = Convert.ToInt16(ReadConfig("proxyChangerValue"));
        }

        static void WriteConfig(string setting, string value)
        {
            using (StreamWriter writer = new StreamWriter(configPath, true))
            {
                writer.WriteLine(setting + "=" + value);
            }
        }

        public static string ReadConfig(string setting)
        {
            using (StreamReader reader = new StreamReader(configPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('=');
                    if (parts[0] == setting)
                    {
                        return parts[1];
                    }
                }
            }
            return null;
        }
    }
}
