using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace Tools
{
    public class SystemInfoTool : ITool
    {
        public string Name => "get_system_info";

        public Task<string> ExecuteAsync(string argumentsJson)
        {
            string osName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Windows"
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "Linux"
                    : "Desconocido";

            if (osName == "Windows")
            {
                string hostName = Environment.MachineName;
                var drives = DriveInfo.GetDrives();
                string driveLetters = string.Join(", ", Array.ConvertAll(drives, d => d.Name.TrimEnd('\\')));

                return Task.FromResult($"El sistema operativo actual es: {osName}\nNombre del equipo: {hostName}\nUnidades disponibles: {driveLetters}");
            }

            return Task.FromResult($"El sistema operativo actual es: {osName}");
        }
    }
}