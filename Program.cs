using System;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using Nethereum.Web3;
using System.Text;
using System.Management;
using System.Windows.Forms;
using DiscordRPC;
<<<<<<< HEAD
using System.Linq;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;
=======
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b

namespace equity_cracker
{
    internal static class Program
    {
        public static DiscordRpcClient client;

        #region Vars
<<<<<<< HEAD
        private static readonly object  consoleLock  = new object();
=======
        private static object  consoleLock  = new object();
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
        public  static Boolean runCode      = false;
        public  static int     hits         = 0;
        public  static int     earnedMoney  = 0;
        public  static int     checks       = 0;
        public  static int     proxyChangerValue = 0;
        public  static Boolean DebugOption  = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
        public  static Boolean enableCustomRPC = Convert.ToBoolean(ConfigurationManager.AppSettings["enableCustomRPC"]);
        public  static Boolean censoreRPC   = Convert.ToBoolean(ConfigurationManager.AppSettings["censoreRPC"]);
        public  static int     Threads      = Convert.ToInt16(ConfigurationManager.AppSettings["threads"]);
        public  static string  cryptoToMine = Convert.ToString(ConfigurationManager.AppSettings["cryptoToMine"]);
        public  static string  customRPC    = Convert.ToString(ConfigurationManager.AppSettings["customRPC"]);
        public  static string  webhookUrl    = Convert.ToString(ConfigurationManager.AppSettings["webhookUrl"]);
        public  static int     consoleRefreshRate = Convert.ToInt16(ConfigurationManager.AppSettings["consoleRefreshRate"]);
        public  static Boolean discordWebhook = Convert.ToBoolean(ConfigurationManager.AppSettings["discordWebhook"]);
        public  static Boolean recapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["recapEnabled"]);
        public  static string  recapSecondDelay = Convert.ToString(ConfigurationManager.AppSettings["recapSecondDelay"]);

        public  static string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        public  static string logPath = Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\latest.txt"));

        #endregion

        static void Initialize()
        {
            try
            {
                client = new DiscordRpcClient("976039106150821898");

                client.OnReady += (sender, e) =>
                {
<<<<<<< HEAD
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Received Ready from user {e.User.Username}{Environment.NewLine}");
=======
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                    //Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };

                client.OnPresenceUpdate += (sender, e) =>
                {
<<<<<<< HEAD
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Received Update! {e.Presence}{Environment.NewLine}");
=======
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                    //Console.WriteLine("Received Update! {0}", e.Presence);
                };

                client.OnError += (sender, e) =>
                {
<<<<<<< HEAD
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Discord RPC Error! Error information: {e}{Environment.NewLine}");
=======
                    //client.Deinitialize();
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                };

                client.Initialize();

                client.SetPresence(new RichPresence()
                {
<<<<<<< HEAD
                    Details = $"Mining with {Threads} threads",
=======
                    Details = "Mining with " + Threads + " threads",
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                    Assets = new Assets()
                    {
                        LargeImageKey = "equityw",
                        LargeImageText = "EquityWMiner",
                        SmallImageKey = null
                    },

                    Buttons = new DiscordRPC.Button[] { }
                });
                client.UpdateStartTime();
            } catch(Exception ex)
            {
                File.AppendAllText(logPath, ex.ToString());
                client.Deinitialize();
            }
        }

        public static Task NewMenu()
        {
            int originalWidth = Console.WindowWidth;
            int originalHeight = Console.WindowHeight;

            Thread monitorThread = new Thread(() =>
            {
                while (true)
                {
                    if (Console.WindowWidth != originalWidth || Console.WindowHeight != originalHeight)
                    {
                        MessageBox.Show("Please don't change the window size while loading the miner..", "EquityMiner v3", MessageBoxButtons.OK);

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
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\")));
                }

                if (File.Exists(logPath))
                {
                    string[] lines = File.ReadAllLines(logPath);
                    string secondLine = lines[1];

                    string modifiedString = secondLine.Replace(":", "-").Replace(" ", "_").Replace(".", "-");
                    modifiedString = modifiedString.Remove(0, 15);

<<<<<<< HEAD
                    File.Move(logPath, Path.GetFullPath(Path.Combine(exePath, $@"..\..\logs\{modifiedString}.txt")));
=======
                    File.Move(logPath, Path.GetFullPath(Path.Combine(exePath, @"..\..\logs\" + modifiedString + ".txt")));
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                }

                StringBuilder sb = new StringBuilder();

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
<<<<<<< HEAD
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
=======
                foreach (ManagementObject queryObj in searcher.Get())
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                {
                    sb.AppendLine("CPU: " + queryObj["Name"]);
                }

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
<<<<<<< HEAD
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
=======
                foreach (ManagementObject queryObj in searcher.Get())
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                {
                    sb.AppendLine("Operating system: " + queryObj["Caption"]);
                }

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                long totalMemory = 0;
<<<<<<< HEAD
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
=======
                foreach (ManagementObject queryObj in searcher.Get())
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                {
                    totalMemory += Convert.ToInt64(queryObj["Capacity"]);
                }
                totalMemory /= 1024;
<<<<<<< HEAD
                sb.AppendLine($"Memory: {totalMemory} MB");

                File.AppendAllText(logPath, 
                    $"Software started" +
                    $"\n" +
                    $"=> Start time: {DateTime.Now}" +
                    $"\n" +
                    $"=> Settings = 1/{DebugOption} 2/{enableCustomRPC} 3/{censoreRPC} 4/{Threads} 5/{cryptoToMine} 6/{consoleRefreshRate} 7/{discordWebhook}" +
                    $"\n" +
                    $"\n" +
                    $"{sb}{Environment.NewLine}");
=======
                sb.AppendLine("Memory: " + totalMemory + " MB");

                // 1 = DebugOption
                // 2 = enableCustomRPC
                // 3 = censoreRPC
                // 4 = Threads
                // 5 = cryptoToMine
                // 6 = consoleRefreshRate
                // 7 = discordWebhook
                File.AppendAllText(logPath, "Software started\n=> Start time: " + DateTime.Now.ToString() + "\n=> Settings = 1/" + DebugOption + " 2/" + enableCustomRPC + " 3/" + censoreRPC + " 4/" + Threads + " 5/" + cryptoToMine + " 6/" + consoleRefreshRate + " 7/" + discordWebhook + "\n\n" + sb.ToString() + Environment.NewLine);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            }
            catch (Exception e)
            {
                if (DebugOption == true)
                {
                   Console.WriteLine(e.Message);
                }
            }

<<<<<<< HEAD
            string recapEnabledString = "Disabled";
            if (recapEnabled == true) { recapEnabledString = "Enabled"; }

=======
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            if (discordWebhook == true)
            {
                var embed = new
                {
                    title = "EquityCracker v3 [started]",
<<<<<<< HEAD
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
=======
                    //description = "‎",
                    description = "__**Current Settings**__\n\n:alarm_clock: Start time: **" + DateTime.Now.ToString() + "**\n :gear: Build version: **1901**" + "\n:gear: Threads: **" + Threads + "**",
                    color = "16711680",
                    fields = new[] {
                        new {
                            name = "Start time",
                            value = ":alarm_clock: " + DateTime.Now.ToString(),
                            inline = false
                        },
                        new {
                            name = "Build version",
                            value = ":gear: 1901",
                            inline = false
                        },
                        new {
                            name = "Threads",
                            value = ":gear: " + Threads,
                            inline = false
                        }
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                    }
                };

                var message = new
                {
                    embeds = new[] { embed }
                };

                var json = JsonConvert.SerializeObject(message);

                using (var client = new HttpClient())
                {
                    var result = client.PostAsync(webhookUrl, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (result.IsSuccessStatusCode)
                    {
<<<<<<< HEAD
                        File.AppendAllText(logPath, $"[{DateTime.Now}] Successfully sent the welcome webhook message.{Environment.NewLine}");
                    }
                    else
                    {
                        File.AppendAllText(logPath, $"[{DateTime.Now}] An error occur while sending the welcome webhook message: {result.StatusCode}{Environment.NewLine}");
=======
                        File.AppendAllText(logPath, "[" + DateTime.Now + "] " + "Successfully sent the welcome webhook message." + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(logPath, "[" + DateTime.Now + "] " + "An error occur while sending the welcome webhook message: " + result.StatusCode);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
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
<<<<<<< HEAD
            string undertitle = "Version: v3.0.1 - build 2009";
=======
            string undertitle = "Version: v3.0.1 - build 1901";
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            Console.CursorVisible = false;
            int width = 30;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            int i = 0;
            
            for(; i <= 1; i++)
            {
<<<<<<< HEAD
                if(i == 1)
                {
                    Thread t = new Thread(BackgroundThread);
                    t.Start();
                }
=======
                //i++;
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                Console.SetCursorPosition(left, top);
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(left + i, top);
                Console.Write("#");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(left + width, top);
                Console.Write("]");
                Console.SetCursorPosition(left + width + 1, top);
                Console.Write(" {0}%", (i * 100) / width);
                Thread.Sleep(100);
            }

<<<<<<< HEAD
=======
            Thread t = new Thread(BackgroundThread);
            t.Start();

            for (i = 0; i <= 5; i++)
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
                Console.Write(" {0}%", (i * 100) / width);
                Thread.Sleep(60);
            }

>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            Console.SetCursorPosition(0, 8);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, 11);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Checking Hit saving..");
            Console.ForegroundColor = ConsoleColor.White;

            var testExePath = System.Reflection.Assembly.GetEntryAssembly().Location;

            string testPath = Path.GetFullPath(Path.Combine(testExePath, @"..\..\hits.txt"));

            try
            {
                if (!(File.Exists(testPath)))
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
<<<<<<< HEAD
                Console.WriteLine("Error: {0}", e);
=======
                Console.WriteLine("Error: " + e);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            }

            try
            {
<<<<<<< HEAD
                File.AppendAllText(testPath, $"[TEST HIT] Private Key: 0x00000000000000000000 {Environment.NewLine}");
=======
                File.AppendAllText(testPath, "[TEST HIT] Private Key: " + "0x00000000000000000000" + Environment.NewLine);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                Console.SetCursorPosition(0, 8);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 8);
            }
            catch(Exception e) 
            {
                Console.SetCursorPosition(0, 8);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 8);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("There was an error while checking hit saving.");
                Console.ForegroundColor = ConsoleColor.White;
<<<<<<< HEAD
                Console.WriteLine("Error: {0}", e);
=======
                Console.WriteLine("Error: " + e);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
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
                Console.Write(" {0}%", (i * 100) / width);
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
                    monitorThread.Abort();
                    for (int x = 0; x < Threads; x++)
                    {
                        runCode = true;
<<<<<<< HEAD
                        Thread minerThreads = new Thread(MainMiner);
=======
                        Thread minerThreads = new Thread(mainMiner);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                        minerThreads.Start();
                    }
                }
                else
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.ForegroundColor= ConsoleColor.Red;
                    Console.WriteLine("Exiting..");
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                } else
                if (key.Key == ConsoleKey.C)
                {
                    var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

                    string path = Path.GetFullPath(Path.Combine(exePath, @"..\..\config\settings.config"));
                    Process.Start("notepad.exe", path);
                }
            }
        }

        static string address;
<<<<<<< HEAD
        static TimeSpan duration;
        static Nethereum.Hex.HexTypes.HexBigInteger balance;
=======
        static System.TimeSpan duration;
        static Nethereum.Hex.HexTypes.HexBigInteger balance;

>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
        static string final;
        static string cpuUsage;
        static void BackgroundThread()
        {
            while(true)
            {
                if (runCode)
                {
<<<<<<< HEAD
                    try
                    {
                        Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";

                        PerformanceCounter cpuCounter;

                        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

=======
                    Console.Title = "EquityCracker | Mining.. | Hits: " + hits + " | Earned money: $" + earnedMoney;
                    Console.WriteLine("ok");
                    PerformanceCounter cpuCounter;
                    PerformanceCounter ramCounter;

                    cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    while (true)
                    {
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                        if (runCode == false)
                        {
                            Console.ReadLine();
                        }
                        var firstCheck = checks;
<<<<<<< HEAD
                        Thread.Sleep(consoleRefreshRate);
=======
                        Thread.Sleep(1000);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                        var secondCheck = checks;
                        var local = secondCheck - firstCheck;
                        var local2 = local;
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
<<<<<<< HEAD

                        Console.SetCursorPosition(0, 19);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 19);

                        Console.SetCursorPosition(0, 20);
                        Console.Write(new string(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, 20);
                    } catch (Exception e)
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
=======
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                    }
                }
            }
        }

        static string rpc = "https://rpc.ankr.com/eth";
<<<<<<< HEAD
        static async void MainMiner()
=======
        static async void mainMiner()
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
        {
            Console.CursorVisible = false;
            try
            {
                if (enableCustomRPC == true)
                {
                    rpc = customRPC;
                }
                while (true)
                {
                    while (runCode)
                    {
<<<<<<< HEAD
                        var web3 = new Web3(rpc);
=======
                        var web3 = new Nethereum.Web3.Web3(rpc);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                        var startTime = DateTime.Now;
                        var rng = new RNGCryptoServiceProvider();
                        var privateKeyBytes = new byte[32];
                        rng.GetBytes(privateKeyBytes);
                        var privateKey = BitConverter.ToString(privateKeyBytes).Replace("-", "").ToLower();
                        var endTime = DateTime.Now;
                        duration = endTime - startTime;
                        var account = new Nethereum.Web3.Accounts.Account(privateKey);
<<<<<<< HEAD
=======
                        //var address = "0x1b3cb81e51011b549d78bf720b0d924ac763a7c2";
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                        address = account.Address;
                        decimal etherAmount;
                        try
                        {
                            balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
                            etherAmount = Web3.Convert.FromWei(balance.Value);
                        }
                        catch (Exception)
                        {
                            if (enableCustomRPC == true)
                            {
                                continue;
                            } else
                            if (rpc == "https://rpc.ankr.com/eth")
                            {
                                rpc = "https://eth.llamarpc.com";
                            }
                            else if (rpc == "https://eth.llamarpc.com")
                            {
                                rpc = "https://cloudflare-eth.com/";
                            }
                            else if (rpc == "https://cloudflare-eth.com/")
                            {
                                rpc = "https://rpc.ankr.com/eth";
                            }
                            continue;
                        }

                        checks++;

                        decimal etherAmount2 = etherAmount;
                        if (etherAmount > 0)
                        {
                            var walletValue = 0;
                            try
                            {
                                runCode = false;
<<<<<<< HEAD

                                var url = "https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD";

=======
                                var url = "https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD";
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                                using (var client = new HttpClient())
                                {
                                    var response = client.GetAsync(url).Result;
                                    var price = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                                    decimal etherPrice = price.USD;

<<<<<<< HEAD
                                    var etherAmountTempString = etherAmount.ToString();
                                    var etherPriceTempString = etherPrice.ToString();

                                    etherAmountTempString = etherAmountTempString.Replace(",", ".");
                                    etherPriceTempString = etherPriceTempString.Replace(",", ".");

                                    Console.WriteLine(etherAmountTempString + " | " + etherPriceTempString);
                                    Int32.TryParse(etherAmountTempString.ToString(), out int etherAmountOut);
                                    Int32.TryParse(etherPriceTempString.ToString(), out int etherPriceOut);
=======
                                    //var etherAmountOut = Convert.ToInt64(etherAmount);
                                    //var etherPriceIdk = Convert.ToInt64(etherPrice);
                                    //Console.WriteLine(etherAmount + " | " + etherPrice);
                                    //Console.ReadLine();
                                    var etherAmountTempString = etherAmount.ToString();
                                    var etherPriceTempString = etherPrice.ToString();
                                    etherAmountTempString = etherAmountTempString.Replace(",", ".");
                                    etherPriceTempString = etherPriceTempString.Replace(",", ".");
                                    Console.WriteLine(etherAmountTempString + " | " + etherPriceTempString);
                                    int etherAmountOut;
                                    int etherPriceOut;
                                    Int32.TryParse(etherAmountTempString.ToString(), out etherAmountOut);
                                    Int32.TryParse(etherPriceTempString.ToString(), out etherPriceOut);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b

                                    walletValue = etherAmountOut * etherPriceOut;
                                    earnedMoney += walletValue;
                                }
                            } catch(Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }

                            var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

                            string path = Path.GetFullPath(Path.Combine(exePath, @"..\..\hits.txt"));

<<<<<<< HEAD
                            var tempweb3 = new Web3("https://rpc.ankr.com/eth");
=======
                            var tempweb3 = new Nethereum.Web3.Web3("https://rpc.ankr.com/eth");
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                            try
                            {
                                balance = await tempweb3.Eth.GetBalance.SendRequestAsync(address);
                                etherAmount = Web3.Convert.FromWei(balance.Value);
                            } catch (Exception)
                            {
                                File.AppendAllText(path,
<<<<<<< HEAD
                                        $"[Possible Ghost Hit] Private Key: {privateKey} | Bal: {etherAmount2} {Environment.NewLine}");
=======
                                        "[Possible Ghost Hit] Private Key: " + privateKey + " | Bal: " + etherAmount2 + Environment.NewLine);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                                break;
                            }

                            hits++;

                            try
                            {
                                if (!(File.Exists(path)))
                                {
                                    using (FileStream fs = File.Create(path)) { };
                                }
                            } catch (Exception)
                            {
                                runCode = false;
                                Thread.Sleep(1000);
                                Console.WriteLine("Something went wrong with saving hits! Please save the private key down below!");
                                Console.WriteLine("If you want to support us, you can donate to this ethereum address: 0xe0f37a884658556d7577a5d34184f8054a4f752e");
                                Console.WriteLine();
<<<<<<< HEAD
                                Console.WriteLine($"Private Key: {privateKey} | Bal: {etherAmount2}");
                                Console.ReadLine();
                            }

                            Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";
=======
                                Console.WriteLine("Private Key: " + privateKey + " | Bal: " + etherAmount2);
                                Console.ReadLine();
                            }

                            Console.Title = "EquityCracker | Mining.. | Hits: " + hits + " | Earned money: $" + earnedMoney;
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                            try
                            {

                                Console.ForegroundColor= ConsoleColor.Green;
<<<<<<< HEAD
                                Console.WriteLine($"YOU GOT A HIT! | PrivateKey: {privateKey} | Bal: {etherAmount2}");
                                earnedMoney += walletValue;
                                Console.ForegroundColor = ConsoleColor.White;
                                File.AppendAllText(path,
                                        $"Private Key: {privateKey} {Environment.NewLine}");
                                Console.Title = $"EquityCracker | Mining.. | Hits: {hits} | Earned money: ${earnedMoney}";
=======
                                Console.WriteLine("YOU GOT A HIT! | PrivateKey: " + privateKey + " | Bal: " + etherAmount2);
                                earnedMoney = earnedMoney + walletValue;
                                Console.ForegroundColor = ConsoleColor.White;
                                File.AppendAllText(path,
                                        "Private Key: " + privateKey + Environment.NewLine);
                                Console.Title = "EquityCracker | Mining.. | Hits: " + hits + " | Earned money: $" + earnedMoney;
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                            } catch (Exception)
                            {
                                runCode = false;
                                Thread.Sleep(2000);
                                Console.WriteLine("Something went wrong with saving hits! Please save the private key down below!");
                                Console.WriteLine("If you want to support us, you can donate to this ethereum address: 0xe0f37a884658556d7577a5d34184f8054a4f752e");
                                Console.WriteLine();
<<<<<<< HEAD
                                Console.WriteLine($"Private Key: {privateKey} | Bal: {etherAmount2}");
=======
                                Console.WriteLine("Private Key: " + privateKey + " | Bal: " + etherAmount2);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
                                Console.ReadLine();
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

<<<<<<< HEAD
        static async Task Main()
        {
            TestCuda();
            Console.ReadLine ();
=======
        public static async Task Main()
        {
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            Initialize();
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            await Task.Run(NewMenu);
            Console.ReadLine();
            Environment.Exit(0);
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
<<<<<<< HEAD
                File.AppendAllText(logPath, $"[{DateTime.Now}] Stopped and closed miner {Environment.NewLine}");
=======
                File.AppendAllText(logPath, "[" + DateTime.Now + "] Stopped and closed miner" + Environment.NewLine);
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
            }
            return false;
        }
        static ConsoleEventDelegate handler;

        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
<<<<<<< HEAD
=======

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
>>>>>>> f3ffb39ed5873ede82c3b0528399fb1e9349dd5b
    }
}
