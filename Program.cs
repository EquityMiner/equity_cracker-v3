using DiscordRPC;
using ManagedCuda.CudaBlas;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace equity_cracker
{
    internal static class Program
    {
        public static DiscordRpcClient client;

        #region Vars
        private static readonly object consoleLock = new object();
        public static bool runCode = false;
        public static int hits = 0;
        public static int earnedMoney = 0;
        public static int checks = 0;
        public static int proxyChangerValue = 0;
        public static bool DebugOption = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
        public static bool enableCustomRPC = Convert.ToBoolean(ConfigurationManager.AppSettings["enableCustomRPC"]);
        public static bool censoreRPC = Convert.ToBoolean(ConfigurationManager.AppSettings["censoreRPC"]);
        public static int Threads = Convert.ToInt16(ConfigurationManager.AppSettings["threads"]);
        public static string webhookUrl = Convert.ToString(ConfigurationManager.AppSettings["webhookUrl"]);
        public static int consoleRefreshRate = Convert.ToInt16(ConfigurationManager.AppSettings["consoleRefreshRate"]);
        public static bool discordWebhook = Convert.ToBoolean(ConfigurationManager.AppSettings["discordWebhook"]);
        public static string hitRecapEnabled = Convert.ToString(ConfigurationManager.AppSettings["hitRecapEnabled"]);
        public static bool recapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["recapEnabled"]);
        public static int recapSecondDelay = Convert.ToInt32(ConfigurationManager.AppSettings["recapSecondDelay"]);

        public static string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        public static string logPath = Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\latest.txt"));

        #endregion

        private static void Initialize()
        {
            try
            {
                client = new DiscordRpcClient("976039106150821898");

                client.OnReady += (sender, e) =>
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Received Ready from user {e.User.Username}{Environment.NewLine}");
                    //Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };

                client.OnPresenceUpdate += (sender, e) =>
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Received Update! {e.Presence}{Environment.NewLine}");
                    //Console.WriteLine("Received Update! {0}", e.Presence);
                };

                client.OnError += (sender, e) =>
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Discord RPC Error! Error information: {e}{Environment.NewLine}");
                };

                _ = client.Initialize();

                client.SetPresence(new RichPresence()
                {
                    Details = $"Mining with {Threads} threads",
                    Assets = new Assets()
                    {
                        LargeImageKey = "equityw",
                        LargeImageText = "EquityWMiner",
                        SmallImageKey = null
                    },

                    Buttons = new DiscordRPC.Button[] { }
                });
                _ = client.UpdateStartTime();
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, ex.ToString());
                client.Deinitialize();
            }
        }

        public static async void NewMenu()
        {
            int originalWidth = Console.WindowWidth;
            int originalHeight = Console.WindowHeight;

            Thread monitorThread = new Thread(() =>
            {
                while (true)
                {
                    if (Console.WindowWidth != originalWidth || Console.WindowHeight != originalHeight)
                    {
                        _ = MessageBox.Show("Please don't change the window size while loading the miner..", "EquityMiner v3", MessageBoxButtons.OK);

                        originalWidth = Console.WindowWidth;
                        originalHeight = Console.WindowHeight;
                    }
                    Thread.Sleep(1000);
                }
            });

            monitorThread.Start();

            try
            {
                if (!Directory.Exists(Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\"))))
                {
                    _ = Directory.CreateDirectory(Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\")));
                }

                if (File.Exists(logPath))
                {
                    string[] lines = File.ReadAllLines(logPath);
                    string secondLine = lines[1];

                    string modifiedString = secondLine.Replace(":", "-").Replace(" ", "_").Replace(".", "-");
                    modifiedString = modifiedString.Remove(0, 15);

                    File.Move(logPath, Path.GetFullPath(Path.Combine(exePath, $@"..\..\logs\{modifiedString}.txt")));
                }

                StringBuilder sb = new StringBuilder();

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    _ = sb.AppendLine("CPU: " + queryObj["Name"]);
                }

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    _ = sb.AppendLine("Operating system: " + queryObj["Caption"]);
                }

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                long totalMemory = 0;
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    totalMemory += Convert.ToInt64(queryObj["Capacity"]);
                }
                totalMemory /= 1024;
                _ = sb.AppendLine($"Memory: {totalMemory} MB");

                File.AppendAllText(logPath,
                    $"Software started" +
                    $"\n" +
                    $"=> Start time: {DateTime.Now}" +
                    $"\n" +
                    $"=> Settings = 1/{DebugOption} 2/Disabled 3/{censoreRPC} 4/{Threads} 5/ETH 6/{consoleRefreshRate} 7/{discordWebhook}" +
                    $"\n" +
                    $"\n" +
                    $"{sb}{Environment.NewLine}");
            }
            catch (Exception e)
            {
                if (DebugOption == true)
                {
                    Console.WriteLine(e.Message);
                }
            }

            string recapEnabledString = "Disabled";
            if (recapEnabled == true) { recapEnabledString = "Enabled"; }

            if (discordWebhook == true)
            {
                var embed = new
                {
                    title = "EquityCracker v3 [started]",
                    color = "16711680",
                    fields = new[] {
                        new {
                            name = "__Current Settings__",
                            value = $":alarm_clock: Start time: **{DateTime.Now}**" +
                            $"\n" +
                            $" :gear: Build version: **1901**" +
                            $"\n" +
                            $":gear: Threads: **{Threads}**" +
                            $"\n" +
                            $":gear: Recap: {recapEnabledString}" +
                            $"\n" +
                            $":gear: Recap Delay: {recapSecondDelay}s",
                            inline = false
                        },
                        new {
                            name = "Start time",
                            value = $":alarm_clock: {DateTime.Now}",
                            inline = false
                        },
                    }
                };

                var message = new
                {
                    embeds = new[] { embed }
                };

                string json = JsonConvert.SerializeObject(message);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage result = client.PostAsync(webhookUrl, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        File.AppendAllText(logPath, $"[{DateTime.Now}] Successfully sent the welcome webhook message.{Environment.NewLine}");
                    }
                    else
                    {
                        File.AppendAllText(logPath, $"[{DateTime.Now}] An error occur while sending the welcome webhook message: {result.StatusCode}{Environment.NewLine}");
                    }
                }
            }

            Console.Title = "Loading..";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string text = @"
███████╗ ██████╗ ██╗   ██╗██╗████████╗██╗   ██╗    ██╗   ██╗██████╗ 
██╔════╝██╔═══██╗██║   ██║██║╚══██╔══╝╚██╗ ██╔╝    ██║   ██║╚════██╗
█████╗  ██║   ██║██║   ██║██║   ██║    ╚████╔╝     ██║   ██║ █████╔╝
██╔══╝  ██║▄▄ ██║██║   ██║██║   ██║     ╚██╔╝      ╚██╗ ██╔╝ ╚═══██╗
███████╗╚██████╔╝╚██████╔╝██║   ██║      ██║        ╚████╔╝ ██████╔╝
╚══════╝ ╚══▀▀═╝  ╚═════╝ ╚═╝   ╚═╝      ╚═╝         ╚═══╝  ╚═════╝ 
";
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(10);
            }
            string undertitle = "Version: v3.0.1 - build 2009";
            Console.CursorVisible = false;
            int width = 30;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            int i = 0;

            for (; i <= 1; i++)
            {
                if (i == 1)
                {
                    Thread t = new Thread(BackgroundThread);
                    t.Start();
                }
                Console.SetCursorPosition(left, top);
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(left + i, top);
                Console.Write("#");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(left + width, top);
                Console.Write("]");
                Console.SetCursorPosition(left + width + 1, top);
                Console.Write(" {0}%", i * 100 / width);
                Thread.Sleep(100);
            }

            Console.SetCursorPosition(0, 8);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, 11);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Checking Hit saving..");
            Console.ForegroundColor = ConsoleColor.White;

            string testExePath = System.Reflection.Assembly.GetEntryAssembly().Location;

            string testPath = Path.GetFullPath(Path.Combine(testExePath, @"..\..\hits.txt"));

            try
            {
                if (!File.Exists(testPath))
                {
                    using (FileStream fs = File.Create(testPath)) { };
                }
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, 8);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 9);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("There was an error while checking hit saving.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Error: {0}", e);
            }

            try
            {
                File.AppendAllText(testPath, $"[TEST HIT] Private Key: 0x00000000000000000000 {Environment.NewLine}");
                Console.SetCursorPosition(0, 8);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 8);
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, 8);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 8);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("There was an error while checking hit saving.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Error: {0}", e);
            }

            for (i = 0; i <= width; i++)
            {
                Console.SetCursorPosition(left, top);
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(left + i, top);
                Console.Write("#");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(left + width, top);
                Console.Write("]");
                Console.SetCursorPosition(left + width + 1, top);
                Console.Write(" {0}%", i * 100 / width);
                Thread.Sleep(100);
            }

            Console.CursorVisible = true;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.ForegroundColor = ConsoleColor.DarkGray;

            foreach (char c in undertitle)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.ForegroundColor = ConsoleColor.White;
            string choice1 = "ENTER";
            string choice1text = " Start Miner";
            string choice2 = "C";
            string choice2text = " Open config in notepad";
            string choice3 = "ESC";
            string choice3text = "  Exit";
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (char c in choice1)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("]");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (char c in choice1text)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (char c in choice2)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("]    ");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (char c in choice2text)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (char c in choice3)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (char c in choice3text)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("-> Starting the Miner, this could take few seconds..");
                    monitorThread.Abort();
                    for (int x = 0; x < Threads; x++)
                    {
                        string pathO = Path.GetFullPath(Path.Combine(exePath, @"..\..\endpoints.txt"));
                        if (File.Exists(pathO))
                        {
                            lines = File.ReadAllLines(pathO);
                        }
                        else
                        {
                            await EquityThings.DownloadFile("https://raw.githubusercontent.com/EquityMiner/equity_cracker-v3/6de51f851d711ffab4513acece3cd8c680246dd2/endpoints.txt", pathO);
                            lines = File.ReadAllLines(pathO);
                        };
                        if (lines.Length <= 0)
                        {
                            MessageBox.Show("No data in endpoints.txt!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }
                        Thread t = new Thread(webhookRecap);
                        t.Start();
                        runCode = true;
                        Thread minerThreads = new Thread(MainMiner);
                        minerThreads.Start();
                    }
                }
                else
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exiting..");
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
                else
                if (key.Key == ConsoleKey.C)
                {
                    string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

                    string path = Path.GetFullPath(Path.Combine(exePath, @"..\..\config\settings.config"));
                    _ = Process.Start("notepad.exe", path);
                }
            }
        }

        private static string address;
        private static TimeSpan duration;
        private static Nethereum.Hex.HexTypes.HexBigInteger balance;
        private static string final;
        private static string cpuUsage;

        private static void BackgroundThread()
        {
            while (true)
            {
                if (runCode)
                {
                    try
                    {
                        Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";

                        PerformanceCounter cpuCounter;

                        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                        if (runCode == false)
                        {
                            _ = Console.ReadLine();
                        }
                        int firstCheck = checks;
                        Thread.Sleep(consoleRefreshRate);
                        int secondCheck = checks;
                        int local = secondCheck - firstCheck;
                        int local2 = local;
                        final = local2.ToString();
                        cpuUsage = cpuCounter.NextValue() + "%";

                        Console.SetCursorPosition(0, 9);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 9);

                        Console.SetCursorPosition(0, 10);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 10);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("         Wallet: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(address);

                        Console.SetCursorPosition(0, 11);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 11);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Generation Time: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(duration);

                        Console.SetCursorPosition(0, 12);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 12);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("        Balance: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(balance);

                        Console.SetCursorPosition(0, 13);
                        Console.Write(new string(' ', Console.BufferWidth));

                        Console.SetCursorPosition(0, 14);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 14);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("         Checks: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(checks);

                        Console.SetCursorPosition(0, 15);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 15);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("            CPS: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(final);

                        Console.SetCursorPosition(0, 16);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 16);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("      CPU Usage: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(cpuUsage);

                        Console.SetCursorPosition(0, 17);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 17);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("       Endpoint: ");
                        Console.ForegroundColor = ConsoleColor.White;

                        if (censoreRPC == true)
                        {
                            Console.WriteLine("[censored]");
                        }
                        else
                        {
                            Console.Write(rpc);
                        }

                        Console.SetCursorPosition(0, 18);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 18);

                        Console.SetCursorPosition(0, 19);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 19);

                        Console.SetCursorPosition(0, 20);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 20);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllText(logPath, $"[{DateTime.Now}] Error while Main Task: {e}{Environment.NewLine}");
                        DialogResult result = MessageBox.Show("There was an critical error in a main task for displaying! Continuing can cause damage to the program or to your PC, do you still want to continue?", "EquityMiner | Error Report", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            File.AppendAllText(logPath, $"[{DateTime.Now}] Dialog continue after error{Environment.NewLine}");
                        }
                        else if (result == DialogResult.No)
                        {
                            File.AppendAllText(logPath, $"[{DateTime.Now}] Dialog exit after error{Environment.NewLine}");
                            Environment.Exit(002);
                        }
                    }
                }
            }
        }

        private static void webhookRecap()
        {
            if (recapEnabled == true)
            {
                int milliseconds = recapSecondDelay * 1000;

                if (discordWebhook == true)
                {
                    while (runCode)
                    {

                        Thread.Sleep(milliseconds);
                        int firstCheck = checks;
                        Thread.Sleep(consoleRefreshRate);
                        int secondCheck = checks;
                        int local = secondCheck - firstCheck;
                        int local2 = local;
                        final = local2.ToString();
                        string[] lines;
                        string pathO = Path.GetFullPath(Path.Combine(exePath, @"..\..\endpoints.txt"));
                        lines = File.ReadAllLines(pathO);
                        if (lines.Length > 0)
                        {
                            MessageBox.Show("Error", "There are no endpoints in the endpoints.txt file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }
                        rpc = lines[0];

                        var embed = new
                        {
                            title = "EquityCracker V3 Recap",
                            color = "16711680",
                            description = $"**Current checks**: {checks}" +
                            $"CPS: {final}" +
                            $"Hits: {hits}" +
                            $"RPC's: {lines.Length}",
                        };

                        var message = new
                        {
                            embeds = new[] { embed }
                        };

                        string json = JsonConvert.SerializeObject(message);

                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage result = client.PostAsync(webhookUrl, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                            if (result.IsSuccessStatusCode)
                            {
                                File.AppendAllText(logPath, $"[{DateTime.Now}] Successfully Discord Webhook recap.{Environment.NewLine}");
                            }
                            else
                            {
                                File.AppendAllText(logPath, $"[{DateTime.Now}] An error occur while sending Discord Webhook recap: {result.StatusCode}{Environment.NewLine}");
                            }
                        }
                    }
                }
                
            }
        }

        private static string rpc = "Loading..";
        private static string[] lines;

        private static async void MainMiner()
        {
            int lineIndex = 0;
            rpc = lines[0];
            Console.CursorVisible = false;
            try
            {
                int errorsCount = 0;
                while (true)
                {
                    while (runCode)
                    {
                        Web3 web3 = new Web3(lines[0]);
                        DateTime startTime = DateTime.Now;
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] privateKeyBytes = new byte[32];
                        rng.GetBytes(privateKeyBytes);
                        string privateKey = BitConverter.ToString(privateKeyBytes).Replace("-", "").ToLower();
                        DateTime endTime = DateTime.Now;
                        duration = endTime - startTime;
                        Nethereum.Web3.Accounts.Account account = new Nethereum.Web3.Accounts.Account(privateKey);
                        address = account.Address;
                        decimal etherAmount;
                        try
                        {
                            balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
                            etherAmount = Web3.Convert.FromWei(balance.Value);
                        }
                        catch (Exception)
                        {
                            errorsCount++;
                            if(errorsCount >= 3)
                            {
                                if (lineIndex == lines.Length - 1)
                                {
                                    lineIndex = 0;
                                    rpc = lines[lineIndex];
                                    continue;
                                }
                                else
                                {
                                    lineIndex++;
                                    rpc = lines[lineIndex];
                                    continue;
                                }
                            }
                            continue;
                        }

                        checks++;

                        decimal etherAmount2 = etherAmount;
                        if (etherAmount > 0)
                        {
                            int walletValue = 0;
                            try
                            {
                                runCode = false;

                                string url = "https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD";

                                using (HttpClient client = new HttpClient())
                                {
                                    HttpResponseMessage response = client.GetAsync(url).Result;
                                    dynamic price = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                                    decimal etherPrice = price.USD;

                                    string etherAmountTempString = etherAmount.ToString();
                                    string etherPriceTempString = etherPrice.ToString();

                                    etherAmountTempString = etherAmountTempString.Replace(",", ".");
                                    etherPriceTempString = etherPriceTempString.Replace(",", ".");

                                    Console.WriteLine(etherAmountTempString + " | " + etherPriceTempString);
                                    _ = int.TryParse(etherAmountTempString.ToString(), out int etherAmountOut);
                                    _ = int.TryParse(etherPriceTempString.ToString(), out int etherPriceOut);

                                    walletValue = etherAmountOut * etherPriceOut;
                                    earnedMoney += walletValue;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }

                            string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

                            string path = Path.GetFullPath(Path.Combine(exePath, @"..\..\hits.txt"));

                            Web3 tempweb3 = new Web3("https://rpc.ankr.com/eth");
                            try
                            {
                                balance = await tempweb3.Eth.GetBalance.SendRequestAsync(address);
                                etherAmount = Web3.Convert.FromWei(balance.Value);
                            }
                            catch (Exception)
                            {
                                File.AppendAllText(path,
                                        $"[Possible Ghost Hit] Private Key: {privateKey} | Bal: {etherAmount2} {Environment.NewLine}");
                                break;
                            }

                            hits++;

                            try
                            {
                                if (!File.Exists(path))
                                {
                                    using (FileStream fs = File.Create(path)) { };
                                }
                            }
                            catch (Exception)
                            {
                                runCode = false;
                                Thread.Sleep(1000);
                                Console.WriteLine("Something went wrong with saving hits! Please save the private key down below!");
                                //Console.WriteLine("If you want to support us, you can donate to this ethereum address: 0xe0f37a884658556d7577a5d34184f8054a4f752e");
                                Console.WriteLine();
                                Console.WriteLine($"Private Key: {privateKey} | Bal: {etherAmount2}");
                                _ = Console.ReadLine();
                            }

                            Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";

                            try
                            {

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"YOU GOT A HIT! | PrivateKey: {privateKey} | Bal: {etherAmount2}");
                                earnedMoney += walletValue;
                                Console.ForegroundColor = ConsoleColor.White;
                                File.AppendAllText(path,
                                        $"Private Key: {privateKey} {Environment.NewLine}");
                                Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";
                            }
                            catch (Exception)
                            {
                                runCode = false;
                                Thread.Sleep(2000);
                                Console.WriteLine("Something went wrong with saving hits! Please save the private key down below!");
                                Console.WriteLine("If you want to support us, you can donate to this ethereum address: 0xe0f37a884658556d7577a5d34184f8054a4f752e");
                                Console.WriteLine();
                                Console.WriteLine($"Private Key: {privateKey} | Bal: {etherAmount2}");
                                _ = Console.ReadLine();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private static async Task Main()
        {
            Initialize();
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            _ = SetConsoleCtrlHandler(handler, true);

            await Task.Run(NewMenu);
            _ = Console.ReadLine();
            Environment.Exit(0);
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                File.AppendAllText(logPath, $"[{DateTime.Now}] Stopped and closed miner {Environment.NewLine}");
            }
            return false;
        }

        private static ConsoleEventDelegate handler;

        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
