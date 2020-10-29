namespace Mod_Downloader {
    public class Config {
        public bool LogToFile = true;
        public Microsoft.Extensions.Logging.LogLevel LogLevel = Microsoft.Extensions.Logging.LogLevel.Information;

        public Config() { }
    }
}