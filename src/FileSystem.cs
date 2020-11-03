using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Mod_Downloader {
    public class FileSystem {
        private static Logger _logger = new Logger("Filesystem");
        public static string OldMods = Path.Join(Downloader.downloadTo, "old_mods");

        public static string GetDownloadPath() {
            string downloadPath = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                downloadPath = Path.Join(Environment.GetEnvironmentVariable("HOME"), "/Library/Application Support/minecraft/mods");
                Directory.CreateDirectory(downloadPath);
                return downloadPath;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                downloadPath = Path.Join(Environment.GetEnvironmentVariable("HOME"), ".minecraft/mods");
                Directory.CreateDirectory(downloadPath);
                return downloadPath;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                downloadPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft/mods");
                Directory.CreateDirectory(downloadPath);
                return downloadPath;
            } else {
                _logger.Critical("No mods folder was found. Are you running this on Windows, Linux or Mac?");
                Environment.Exit(1);
                return null;
            }
        }

        public static void PrepareDownload() {
            string[] currentMods = Directory.GetFiles(Downloader.downloadTo);
            List<string> hashList = new List<string>();
            //Sort hash list alphabetically to be nice to the user.
            Array.Sort(currentMods, (x, y) => String.Compare(x, y));
            foreach (string mod in Downloader.downloadMods.Trim().Split('\n')) hashList.Add(mod.Split(' ') [2]);

            if (currentMods.Length != 0) {
                Directory.CreateDirectory(OldMods);
                foreach (string file in currentMods) {
                    if (string.IsNullOrEmpty(file)) break;
                    // Check to make sure the file isn't a directory and that it isn't already a mod.
                    if (Path.GetExtension(file) == ".jar" && !hashList.Contains(CalcHash(file))) {
                        File.Move(file, Path.Join(OldMods, Path.GetFileName(file)), true);
                        _logger.Debug($"Moved '{file}' to '{Path.Join(OldMods, file)}'");
                    }
                }
                return;
            } else {
                _logger.Info($"Nothing to prepare in {Downloader.downloadTo}...");
                return;
            }
        }

        public static string CalcHash(string filePath) => BitConverter.ToString(MD5.Create().ComputeHash(File.OpenRead(filePath)));
    }
}