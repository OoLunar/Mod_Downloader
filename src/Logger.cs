using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Mod_Downloader {
    public class Logger : ILogger {
        public static string LogFile = Path.Combine("log/", $"{DateTime.Now.ToString("dd MMM yyyy HH.mm.ss")}.log");
        private readonly string _branchName;

        public Logger(string branchName) => _branchName = branchName;
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            switch (logLevel) {
                case LogLevel.Debug:
                    if (Mod_Downloader.Program.Config.LogLevel <= LogLevel.Debug) Debug(formatter(state, exception));
                    break;
                case LogLevel.Information:
                    if (Mod_Downloader.Program.Config.LogLevel <= LogLevel.Information) Info(formatter(state, exception));
                    break;
                case LogLevel.Warning:
                    if (Mod_Downloader.Program.Config.LogLevel <= LogLevel.Warning) Warn(formatter(state, exception));
                    break;
                case LogLevel.Error:
                    if (Mod_Downloader.Program.Config.LogLevel <= LogLevel.Error) Error(formatter(state, exception));
                    break;
                case LogLevel.Critical:
                    if (Mod_Downloader.Program.Config.LogLevel <= LogLevel.Critical) Critical($"[{_branchName}] {formatter(state, exception)}");
                    break;
                default:
                    break;
            }
        }

        public void Debug(string value) {
            Console.ResetColor();
            Console.Write($"[{getTime()}] ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[Debug]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"    {_branchName}");
            Console.ResetColor();
            Console.WriteLine($": {value}");
        }

        public void Info(string value) {
            Console.ResetColor();
            Console.Write($"[{getTime()}] [Info]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"     {_branchName}");
            Console.ResetColor();
            Console.WriteLine($": {value}");
        }

        public void Warn(string value) {
            Console.ResetColor();
            Console.Write($"[{getTime()}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[Warning]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {_branchName}");
            Console.ResetColor();
            Console.WriteLine($": {value}");
        }

        public void Error(string value) {
            Console.ResetColor();
            Console.Write($"[{getTime()}] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[Error]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"    {_branchName}");
            Console.ResetColor();
            Console.WriteLine($": {value}");
        }

        public void Critical(string value) {
            Console.ResetColor();
            Console.Write($"[{getTime()}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write($"[Critical]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($" {_branchName}");
            Console.ResetColor();
            Console.WriteLine($": {value}");
        }

        private string getTime() => DateTime.Now.ToLocalTime().ToString("ddd, dd MMM yyyy HH':'mm':'ss");
    }

    public class LoggerProvider : ILoggerFactory {
        private readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();
        public ILogger CreateLogger(string branchName) => _loggers.GetOrAdd(branchName, name => new Logger(name));
        public void Dispose() => _loggers.Clear();

        public void AddProvider(ILoggerProvider provider) { }
    }
}