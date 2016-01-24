namespace codingfreaks.WadLogTail.Ui.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using codingfreaks.cfUtils.Logic.Azure;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;

    class Program
    {
        #region constants

        private const int MinConsoleHeight = 7;

        private const int MinConsoleWidth = 80;

        private static int _currentConsoleHeight;
        private static int _currentConsoleWidth;

        private static readonly TableHelper<WadLogEntity> Helper = new TableHelper<WadLogEntity>();

        private static bool _isBusy;

        #endregion

        #region methods

        static void Main(string[] args)
        {
            var cloudStorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            int secondsInPast;
            if (!int.TryParse(ConfigurationManager.AppSettings["TimeSpanSeconds"] ?? "3600", out secondsInPast))
            {
                PrintProgramInfo("Invalid settings value SecondsInPast.");
                Console.ReadKey();
                return;
            }
            if (args.Any())
            {
                if (args[0].Equals("?"))
                {
                    PrintProgramInfo();
                    Console.ReadKey();
                    return;
                }
                cloudStorageConnectionString = args[0];
            }
            if (args.Length >= 2 && !int.TryParse(args[1], out secondsInPast))
            {
                PrintProgramInfo("Invalid arguments. Seconds argument must be a number.");
                Console.ReadKey();
                return;
            }
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
            }
            catch
            {
                PrintProgramInfo($"Invalid storage connection string: {cloudStorageConnectionString}");
                Console.ReadKey();
                return;
            }
            var allEntries = new List<WadLogEntity>();
            WriteConsoleHeader(storageAccount.BlobStorageUri);
            WriteConsoleFooter(allEntries.Count);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WADLogsTable");
            Helper.MonitoringReceivedNewEntries += (s, e) =>
            {
                var entries = e.Entries;
                var lastTicks = long.Parse(allEntries.LastOrDefault()?.PartitionKey ?? "0");
                allEntries.AddRange(entries);
                WriteEntries(storageAccount, allEntries, lastTicks);
            };
            Helper.QueryStarted += (s, e) =>
            {
                _isBusy = true;
            };
            Helper.QueryFinished += (s, e) =>
            {
                _isBusy = false;
            };
            // start a task for the busy indicator and console-resize-eventhandler
            Task.Run(
                () =>
                {
                    var busyElements = new List<string>
                    {
                        "|",
                        "/",
                        "-",
                        "\\",
                        "|",
                        "/",
                        "-",
                        "\\"
                    };
                    var lastOffset = 0;
                    while (true)
                    {
                        Console.SetCursorPosition(Console.WindowWidth - 1, 0);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        if (_isBusy)
                        {
                            // show busy
                            Console.Write(busyElements[lastOffset]);
                            Console.ResetColor();
                            lastOffset++;
                            if (lastOffset >= busyElements.Count)
                            {
                                lastOffset = 0;
                            }
                        }
                        else
                        {
                            // hide busy
                            Console.Write(" ");
                        }
                        Console.ResetColor();
                        Task.Delay(500).Wait();
                        // react on console window size change
                        if (Console.WindowWidth < MinConsoleWidth || Console.WindowHeight < MinConsoleHeight)
                        {
                            Console.Clear();
                            Console.WriteLine("Console too small. Increase size of the window please!");
                        }
                        else
                        {
                            if (Console.WindowWidth != _currentConsoleWidth || Console.WindowHeight != _currentConsoleHeight)
                            {
                                WriteEntries(storageAccount, allEntries, long.Parse(allEntries.LastOrDefault()?.PartitionKey ?? "0"));
                                _currentConsoleWidth = Console.WindowWidth;
                                _currentConsoleHeight = Console.WindowHeight;
                            }
                        }
                    }
                });
            var tokenSource = new CancellationTokenSource();
            Task.Run(async () => await Helper.MonitorTableAsync(table, tokenSource.Token, 5, secondsInPast), tokenSource.Token);
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Cancelling process...");
            tokenSource.Cancel();
        }

        /// <summary>
        /// Prints the header and help to the console.
        /// </summary>
        /// <param name="error">An optional error-text.</param>
        private static void PrintProgramInfo(string error = null)
        {
            Console.Clear();
            Console.WriteLine("codingfreaks WadLogTail\n");
            if (!string.IsNullOrEmpty(error))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
            }
            Console.WriteLine("\nParameter-Call: WadLogTail.exe {Azure Cloud Storage Connectionstring} {seconds in past}");
            Console.WriteLine("\nHelp\n");
            Console.WriteLine("There ar 2 ways to configure this app. Either you edit the appSettings in WadLogTail.exe.config");
            Console.WriteLine("or you call the exe with arguments. If you use arguments you must provide the connection string");
            Console.WriteLine("as the first one. The first cmd-argument will always be interpreted as the connection string.\n");
            Console.WriteLine("{Azure Cloud Storage Connectionstring}: Replace this with your secret connection string from the");
            Console.WriteLine("                                        Azure Portal. Alternatively you can change the setting");
            Console.WriteLine("                                        StorageConnectionString in the WadLogTail.exe.config.\n");
            Console.WriteLine("{seconds in past}: The amount of seconds the you want to look back in the Azure WADLogsTable.");
            Console.WriteLine("                   This corresponds to TimeSpanSeconds in the appSettings in WadLogTail.exe.config.");
        }

        /// <summary>
        /// Writes the footer to the bottom of the console.
        /// </summary>
        /// <param name="count">The amount of items collected so far.</param>
        /// <param name="lastQueryTime">The time the last query took.</param>
        private static void WriteConsoleFooter(int count, DateTime? lastQueryTime = null)
        {
            if (Console.WindowWidth < MinConsoleWidth || Console.WindowHeight < MinConsoleHeight)
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            var line = $"{count} entries loaded | last query at {lastQueryTime} | last query time {Helper.LastQueryTime}";
            line += new string(' ', Console.WindowWidth - line.Length);
            Console.Write(line);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes the header to the top of the console.
        /// </summary>
        /// <param name="uri">The storage url.</param>
        private static void WriteConsoleHeader(StorageUri uri)
        {
            if (Console.WindowWidth < MinConsoleWidth || Console.WindowHeight < MinConsoleHeight)
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(0, 0);
            var line = $"codingfreaks WadLogTail | Account: {uri.PrimaryUri.Host.Split('.')[0]} |";
            line += new string(' ', Console.WindowWidth - line.Length);
            Console.Write(line);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes all <see cref="WadLogEntity"/> entries collected to the window.
        /// </summary>
        /// <param name="storageAccount">The storage account for the connection.</param>
        /// <param name="allEntries">The entries collected so far.</param>
        /// <param name="lastTicks">The timestamp of the last top-item.</param>
        private static void WriteEntries(CloudStorageAccount storageAccount, IReadOnlyCollection<WadLogEntity> allEntries, long lastTicks)
        {
            if (Console.WindowWidth < MinConsoleWidth || Console.WindowHeight < MinConsoleHeight)
            {
                return;
            }
            Console.Clear();
            WriteConsoleHeader(storageAccount.BlobStorageUri);
            WriteConsoleFooter(allEntries.Count, DateTime.Now);
            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            var roleInstanceLength = 20;
            if (allEntries.Any())
            {
                roleInstanceLength = allEntries.Max(e => e.RoleInstance.Length);
            }
            var headerFormat = "{0, -19} | {1, -6} | {2, " + roleInstanceLength * -1 + "} | {3}";
            var format = "{0, 19} | {1, 6} | {2, " + roleInstanceLength + "} | {3}";
            var headerLine = string.Format(headerFormat, "Timetamp ^", "PID", "Role Instance", "Message");
            headerLine += new string(' ', Console.WindowWidth - headerLine.Length);
            Console.Write(headerLine);
            Console.ResetColor();
            // 
            var line = 2;
            allEntries.OrderByDescending(entry => entry.PartitionKey).ThenByDescending(entry => entry.RowIndexValue).Take(Console.WindowHeight - 3).ToList().ForEach(
                entry =>
                {
                    var currentTicks = long.Parse(entry.PartitionKey);
                    if (currentTicks > lastTicks)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.SetCursorPosition(0, line++);
                    var message = entry.MessageCleaned;

                    Console.Write(format, entry.Timestamp.ToLocalTime().DateTime, entry.Pid, entry.RoleInstance, message.Length <= 300 ? message : message.Substring(0, 299));
                });
            Console.ResetColor();
        }

        #endregion
    }
}