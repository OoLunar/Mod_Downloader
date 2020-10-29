using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Mod_Downloader {
    public class FileSystem {
        private static Logger _logger = new Logger("Filesystem");
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
                _logger.Critical("No mods folder was found. Are you running this on Windows, Linux or OS X?");
                Environment.Exit(1);
                return string.Empty;
            }
        }

        public static void PrepareDownload() {
            string downloadPath = GetDownloadPath();
            string oldModsFolder = Path.Join(downloadPath, "old_mods");
            string[] currentMods = Directory.GetFiles(downloadPath);
            List<string> modList = new List<string>();
            foreach (string mod in Downloader.downloadMods.Split('\n')) modList.Add(mod.Split() [0]);
            if (currentMods.Length != 0) {
                Directory.CreateDirectory(oldModsFolder);
                foreach (string file in currentMods) {
                    // Check to make sure the file isn't a directory and that it isn't already a mod.
                    if (!File.GetAttributes(file).HasFlag(FileAttributes.Directory) && !modList.Contains(Path.GetFileNameWithoutExtension(file))) {
                        File.Move(file, Path.Join(oldModsFolder, Path.GetFileName(file)), true);
                        _logger.Debug($"Moved '{file}' to '{Path.Join(oldModsFolder, file)}'");
                    }
                }
                return;
            } else {
                _logger.Info($"No files found in {downloadPath}...");
                return;
            }
        }
    }
}