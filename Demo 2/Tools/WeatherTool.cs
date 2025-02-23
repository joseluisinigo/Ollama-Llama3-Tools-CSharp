using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools
{
    public class WeatherTool : ITool
    {
        public string Name => "get_weather";

        public async Task<string> ExecuteAsync(string argumentsJson)
        {
            var arguments = JsonSerializer.Deserialize<WeatherArguments>(argumentsJson);
            if (arguments?.Location == null)
                return "⚠️ Error: No se proporcionó una ubicación válida.";

            return await GetWeatherAsync(arguments.Location);
        }

        private async Task<string> GetWeatherAsync(string location)
        {
            await Task.Delay(500);
            var random = new Random();
            var temperature = random.Next(-10, 40);
            var conditions = new[] { "soleado", "nublado", "lluvioso", "nevando" };
            var condition = conditions[random.Next(conditions.Length)];
            return $"El clima actual en {location} es {condition} con una temperatura de {temperature}°C.";
        }

        private class WeatherArguments
        {
            public string? Location { get; set; }
        }
    }
}
