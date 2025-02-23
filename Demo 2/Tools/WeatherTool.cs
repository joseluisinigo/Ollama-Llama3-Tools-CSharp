using System;
using System.Threading.Tasks;

namespace Tools
{
    public static class WeatherTool
    {
        public static async Task<string> GetWeatherAsync(string location)
        {
            await Task.Delay(500); // Simula una API de clima
            var random = new Random();
            var temperature = random.Next(-10, 40);
            var conditions = new[] { "soleado", "nublado", "lluvioso", "nevando" };
            var condition = conditions[random.Next(conditions.Length)];
            return $"El clima actual en {location} es {condition} con una temperatura de {temperature}°C.";
        }
    }
}
