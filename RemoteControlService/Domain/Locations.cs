using System;
using System.IO;

namespace Domain
{
    public class Locations
    {
        private readonly string logFileName;
        private readonly string shutdownHistoryFileName;

        public string ApplicationDataFolder { get; }

        public string LogFilePath => Path.Combine(ApplicationDataFolder, logFileName);

        public string ShutdownHistoryFilePath => Path.Combine(ApplicationDataFolder, shutdownHistoryFileName);

        public Locations(string applicationDataFolderName, string logFileName, string shutdownHistoryFileName)
        {
            ApplicationDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationDataFolderName);
            this.logFileName = logFileName;
            this.shutdownHistoryFileName = shutdownHistoryFileName;
        }
    }
}
