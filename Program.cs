using System;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.Security.Cryptography;
using System.IO;
using Nethereum.Web3.Accounts;
using System.Net;

namespace equity_cracker
{
    internal class Program
    {
        private static object  consoleLock = new object();
        public  static Boolean runCode     = true;
        public  static int     hits        = 0;

        public static async Task Main()
        {

            Console.Title = "Equity cracker v3 - Hits: " + hits;

            Console.WriteLine("Starting miner.. Good Luck!");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Task[] tasks = new Task[40];

            for (int i = 0; i < 40; i++)
            {
                tasks[i] = Task.Run( async () =>
                {
                    while (runCode)
                    {
                        try
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                            var startTime = DateTime.Now;
                            var rng = new RNGCryptoServiceProvider();
                            var privateKeyBytes = new byte[32];
                            rng.GetBytes(privateKeyBytes);
                            var privateKey = BitConverter.ToString(privateKeyBytes).Replace("-", "").ToLower();
                            var endTime = DateTime.Now;
                            var duration = endTime - startTime;

                            var account = new Account(privateKey);

                            Web3 web3 = new Web3("https://rpc.ankr.com/eth");
                            string walletAddress = account.Address;
                            HexBigInteger balance = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);
                            decimal etherBalance = Web3.Convert.FromWei(balance.Value);
                            var consoleColor = ConsoleColor.White;
                            if (etherBalance != 0)
                            {
                                consoleColor = ConsoleColor.Green;
                                hits++;
                                Console.Title = "Equity cracker v3 - Hits: " + hits;
                                string textToSave = privateKey + " | Bal: " + etherBalance;
                                string path = "./hits.txt";

                                File.WriteAllText(path, textToSave);
                            }
                            else
                            {
                                consoleColor = ConsoleColor.Red;
                            }
                            Write("Private Key: " + privateKey, consoleColor, duration.ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                });
            }
            await Task.WhenAll(tasks);
        }

        public static void Write(string text, ConsoleColor color, string duration)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                if (duration != "no")
                {
                    Console.Write(text);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" | Generation time: " + duration);
                } else
                {
                    Console.WriteLine(text);
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}