using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Mod_Downloader {
    public class Downloader {
        private static WebClient webClient = new WebClient();
        private static Uri downloadUrl = new Uri("https://mc.forsaken-borders.net/mods/list.ini");
        private static Uri forgeUrl = new Uri("https://files.minecraftforge.net/maven/net/minecraftforge/forge/1.16.3-34.1.25/forge-1.16.3-34.1.25-installer.jar");
        private static string downloadTo = Mod_Downloader.FileSystem.GetDownloadPath();
        public static string downloadMods = getList(downloadUrl);
        private static Logger _logger = new Logger("Downloader");

        public Downloader() {
            _logger.Info("Preparing Mods folder.");
            FileSystem.PrepareDownload();
        }

        private static string getList(Uri downloadLink) {
            string list = string.Empty;
            try {
                list = webClient.DownloadString(downloadUrl);
            } catch (WebException error) when((error.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound) {
                _logger.Error("Seems like the mods are no longer available. This likely means that the SMP Modded server has been taken down, or you're using an outdated version of the mod installer.\nPress any key to Exit...");
                Console.ReadKey(true);
                Environment.Exit(1);
            } catch (WebException error) when(error.Message.StartsWith("Connection refused")) {
                _logger.Error("Seems like the server has been taken offline. This could be temporary, or permanent. Either way, we can't download the mods since the server is offline. Sorry.\nPress any key to exit...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
            return list;
        }

        public void GetMods() {
            List<string> currentMods = new List<string>();
            Directory.GetFiles(FileSystem.GetDownloadPath()).ToList().ForEach(file => currentMods.Add(Path.GetFileNameWithoutExtension(file)));
            foreach (string detail in downloadMods.Split('\n')) {
                if (detail == string.Empty) break;
                string title = detail.Split(' ') [0];
                System.Uri link = new System.Uri(detail.Split(' ') [1]);
                if (!currentMods.ToList().Contains(title)) {
                    _logger.Info($"Downloading '{title}'...");
                    webClient.DownloadFile(link, Path.Join(downloadTo, $"{title}.jar"));
                }
            }
            _logger.Info($"Downloaded mods to '{downloadTo}'");
        }

        public void GetForge() {
            string forgePath = Path.Join(Path.GetTempPath(), "forge-1.16.3.jar");
            webClient.DownloadFile(forgeUrl, forgePath);
            _logger.Warn("Starting to install Forge. If Forge has already been installed, just press \"Cancel\"");
            Process java = new Process();
            string javaVar = "java";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) javaVar = "java.exe";
            java.StartInfo.FileName = javaVar;
            java.StartInfo.Arguments = $"-jar {forgePath}";
            java.StartInfo.UseShellExecute = false;
            java.StartInfo.RedirectStandardOutput = true;
            java.Start();
            java.WaitForExit();
            try { } catch (Exception) {
                _logger.Error($"Java isn't installed. Please go install it: https://www.oracle.com/java/technologies/javase-jre8-downloads.html\nYou'll be looking for something like {Environment.OSVersion.Platform}-{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}\nPress any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
        }
    }
}