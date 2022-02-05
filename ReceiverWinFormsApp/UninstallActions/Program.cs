using DataContracts;
using Domain;
using System;
using System.IO;
using System.Text.Json;

namespace UninstallActions
{
    class Program
    {
        static void Main(string[] args)
        {
            var dto = JsonSerializer.Deserialize<LocationPaths>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\LocationPaths.json")));
            var locations = new Locations(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dto.ApplicationDataFolderName),
                dto.LogFileName,
                dto.ShutdownHistoryFileName);

            if (File.Exists(locations.LogFilePath))
            {
                File.Delete(locations.LogFilePath);
            }

            if (File.Exists(locations.ShutdownHistoryFilePath))
            {
                File.Delete(locations.ShutdownHistoryFilePath);
            }

            if (Directory.Exists(locations.ApplicationDataFolder))
            {
                Directory.Delete(locations.ApplicationDataFolder);
            }
        }
    }
}
