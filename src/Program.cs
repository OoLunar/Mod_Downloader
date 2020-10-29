using System;
using System.IO;
using System.Net.Http;

namespace Mod_Downloader {
    public class Program {
        public static string ProjectRoot = Path.GetFullPath("../../../../", System.AppDomain.CurrentDomain.BaseDirectory).Replace('\\', '/');
        public static Config Config = new Config();
        private static HttpClient webClient = new HttpClient();
        private static Logger _logger = new Logger("Main");

        static void Main(string[] args) {
            Downloader downloader = new Downloader();
            _logger.Info("Starting Mod Download...");
            downloader.GetMods();
            _logger.Info("Mods have been downloaded. Downloading Forge...");
            downloader.GetForge();
            _logger.Info("Forge has been installed. Thank you for using the mod installer.\nPress any key to continue...");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
    }
}