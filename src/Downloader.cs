using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Mod_Downloader {
    public class Downloader {
        private static Logger _logger = new Logger("Downloader");
        private static WebClient webClient = new WebClient();
        private static Uri downloadUrl = new Uri("https://mc.forsaken-borders.net/mods/list.ini");
        private static Uri forgeUrl = new Uri("https://mc.forsaken-borders.net/mods/forge");
        public static string downloadTo = Mod_Downloader.FileSystem.GetDownloadPath();
        public static string downloadMods = getList(downloadUrl);

        public Downloader() => FileSystem.PrepareDownload();

        private static string getList(Uri downloadLink) {
            string list = string.Empty;
            try {
                list = webClient.DownloadString(downloadUrl);
            } catch (WebException error) when((error.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound) {
                _logger.Error("Seems like the mods are no longer available. This likely means that the SMP Modded server has been taken down, or you're using an outdated version of the mod installer.\nPress any key to Exit...");
                Console.ReadKey(true);
                Environment.Exit(1);
            } catch (WebException error) when((error.InnerException.InnerException as SocketException).SocketErrorCode == SocketError.ConnectionRefused) {
                _logger.Error("Seems like the server is currently offline. This could be temporary, or permanent. Either way, we can't download the mods. Try again in an hour or two.\nPress any key to exit...");
                Console.ReadKey(true);
                Environment.Exit(1);
            } catch (WebException error) when(error.Status == WebExceptionStatus.Timeout) {
                _logger.Error("The HTTP request to get the mods has timed out. Make sure you have internet. Trying again...");
                list = getList(downloadLink);
            } catch (WebException error) when((error.InnerException.InnerException as SocketException).ErrorCode == 11) {
                _logger.Error("Unable to reach the server. Do you have internet?\nPress any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(1);
            } catch (WebException error) when((error.Response as HttpWebResponse).StatusCode == HttpStatusCode.Forbidden) {
                _logger.Error("Access to the mod list has been denied by the server. You can't do anything about this. Try again in an hour or two.\nPress any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
            return list;
        }

        public void GetMods() {
            List<string> fileHashes = new List<string>();
            Directory.GetFiles(downloadTo).ToList().ForEach(file => fileHashes.Add(FileSystem.CalcHash(file)));
            foreach (string detail in downloadMods.Trim().Split('\n')) {
                string[] splitDetails = detail.Split(' ');
                string title = splitDetails[0];
                Uri link = new Uri(splitDetails[1]);
                string hash = splitDetails[2];
                if (!fileHashes.Contains(hash)) {
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
            string javaVar = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java";
            Process javaProcess = Process.Start(javaVar, $"-jar {forgePath}");
            if (!javaProcess.WaitForExit((int) TimeSpan.FromSeconds(30).TotalMilliseconds)) {
                javaProcess.Kill();
                _logger.Error("Forge took longer than 30 seconds to install. This shouldn't happen.");
            }
            try { } catch (Exception) {
                _logger.Error($"Java isn't installed. Please go install it: https://www.oracle.com/java/technologies/javase-jre8-downloads.html\nYou'll be looking for something like {Environment.OSVersion.Platform}-{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}\nPress any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
        }

        public static void RemoveMods() {
            string[] currentMods = Directory.GetFiles(downloadTo);
            List<string> hashList = new List<string>();
            //Sort hash list alphabetically to be nice to the user.
            Array.Sort(currentMods, (x, y) => String.Compare(x, y));
            foreach (string mod in Downloader.downloadMods.Trim().Split('\n')) hashList.Add(mod.Split(' ') [2]);
            if (currentMods.Length != 0) {
                foreach (string file in currentMods) {
                    if (string.IsNullOrEmpty(file)) break;
                    // Check to make sure the file isn't a directory and that it isn't already a mod.
                    if (Path.GetExtension(file) == ".jar" && hashList.Contains(FileSystem.CalcHash(file))) {
                        File.Delete(file);
                        _logger.Debug($"Deleted '{file}'...");
                    }
                }
                return;
            } else {
                _logger.Info($"No mods to delete in {downloadTo}...");

            }
        }
    }
}