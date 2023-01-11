using System;
using System.IO;
using System.Net;
using NBitcoin;
using ManagedCuda;
using System.Text;
using NBitcoin.JsonConverters;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Nethereum.Web3.Accounts;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities.Net;

namespace equity_cracker
{
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

            if (UseProxy == false)
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("We don't recommend using this miner without proxy! Please use the Python version, else enable 'useProxy' in the .config");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit..");
                Console.ReadLine();
                Environment.Exit(0);
            }

            lock (consoleLock) { Console.Clear(); }

            string path = @"..\hits.txt";
            string proxyFilePath = @"..\proxys.txt";
            if (UseProxy == true)
            {
                if (!(File.Exists(proxyFilePath)))
                {
                    using (FileStream fs = File.Create(proxyFilePath)) { };
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

            for (int i = 0; i < Threads; i++)
            {
                tasks[i] = Task.Run( async () =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (!(File.Exists(proxyFilePath))) { lock (consoleLock) { Console.WriteLine(proxyFilePath + " can't be found!"); Console.ReadLine(); Environment.Exit(0); } }
                    Console.ForegroundColor = ConsoleColor.White;

                    string[] proxys = File.ReadAllLines(proxyFilePath);

                    foreach (string proxy in proxys)
                    {
                        try
                        {
                            int currentProxyIndex = 0;

                            while (runCode)
                            {
                                var currentProxy = proxys[currentProxyIndex];
                                currentProxyIndex++;
                                string url = "https://rpc.ankr.com/" + cryptoToMine;
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

                                string json = "{\"jsonrpc\":\"2.0\",\"method\":\"eth_getBalance\",\"params\":[" +
                                              $"\"{address}\",\"latest\"" +
                                              "],\"id\":1}";

                                var content = new StringContent(json, Encoding.UTF8, "application/json");

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

                                    Write("Private Key: " + privateKey, consoleColor, duration.ToString(), proxy);

                                    checks++;

                                    if(proxyChanger == true) { if (checks > proxyChangerValue) { Console.WriteLine("Changing proxy.."); checks = 0; continue; } }

                                    if (currentProxyIndex >= proxys.Length) { currentProxyIndex = 0; }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (DebugOption == true) { Console.WriteLine("Exception - miner"); Console.WriteLine(e); }
                            continue;
                        }
                    }
                });
            }
            await Task.WhenAll(tasks);
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
}
