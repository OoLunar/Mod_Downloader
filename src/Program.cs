using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Mod_Downloader {
    public class Program {
        private static Logger _logger = new Logger("Main");
        private static HttpClient webClient = new HttpClient();
        private static bool useUI = true;
        public static string ProjectRoot = Path.GetFullPath("../../../../", System.AppDomain.CurrentDomain.BaseDirectory).Replace('\\', '/');
        public static Config Config = new Config();

        static void Main(string[] args) {
            try {
                for (int i = 0; i < args.Length; i++) switch (args[i].ToLower()) {
                    case "--debug":
                        Config.LogLevel = LogLevel.Debug;
                        break;
                    case "--console":
                        useUI = false;
                        break;
                    case "--install-forge":
                        _logger.Info("Installing Forge...");
                        new Downloader().GetForge();
                        _logger.Info("Forge has been installed. Thank you for using the mod installer.\nPress any key to continue...");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                        break;
                    case "--remove":
                        _logger.Info("Removing Mods...");
                        Downloader.RemoveMods();
                        _logger.Info("Mods have been removed. Sorry to see you go!\nPress any key to exit...");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                        break;
                    case "--update":
                        _logger.Info("Updating Mods...");
                        new Downloader().GetMods();
                        _logger.Info("Mods have been updated!\nPress any key to exit...");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                        break;
                    case "-h":
                    case "--help":
                        Console.WriteLine("--debug\t\t\tSpits out verbose information. Mainly used when developing the Program.");
                        Console.WriteLine("--console\t\tWishes for a console interface instead of a GUI.");
                        Console.WriteLine("--install-forge\t\tInstalls the required version of Forge.");
                        Console.WriteLine("--update\t\tUpdates the modpack.");
                        Console.WriteLine("--remove\t\tRemoves the modpack.");
                        Console.WriteLine("--download-to\t\tThe folder to download the mods too. By default it's '.minecraft/mods/'");
                        Console.WriteLine("--old-mods\t\tWhich folder to move your current mods too. By default it's '.minecraft/mods/old_mods/'");
                        Console.WriteLine("--version\t\tWhich version is the installer.");
                        Console.WriteLine("--help\t\t\tDisplays this message!");
                        Environment.Exit(0);
                        break;
                    case "-v":
                    case "--version":
                        Console.WriteLine("Mod Downloader: 3.0.0");
                        Console.WriteLine("Programmed by: https://github.com/OoLunar/ <lunar@forsaken-borders.net>");
                        Console.WriteLine("This program is open source. The git repository can be found at https://github.com/OoLunar/Mod_Downloader");
                        Environment.Exit(0);
                        break;
                    case "--download-to":
                        if (Directory.Exists(args[i + 1])) Downloader.downloadTo = args[i + 1];
                        _logger.Debug($"Set downloadTo variable as '{args[i + 1]}'");
                        break;
                    case "--old-mods":
                        if (Directory.Exists(args[i + 1])) FileSystem.OldMods = args[i + 1];
                        _logger.Debug($"Set OldMods variable as '{args[i + 1]}'");
                        break;
                    default:
                        break;
                }

                if (Environment.UserInteractive && useUI) Gui.Load();
                else {
                    _logger.Info("Preparing Mod Download...");
                    Downloader downloader = new Downloader();
                    _logger.Info("Starting Mod Download...");
                    downloader.GetMods();
                    _logger.Info("Mods have been downloaded. Downloading Forge...");
                    downloader.GetForge();
                    _logger.Info("Forge has been installed. Thank you for using the mod installer.\nPress any key to continue...");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            } catch (Exception error) {
                _logger.Critical($"An unknown error occured. Please make sure your program is up-to-date : https://github.com/OoLunar/Mod_Downloader/releases/latest");
                _logger.Critical($"{error.ToString()}\nPress any key to exit...");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
        }
    }
}