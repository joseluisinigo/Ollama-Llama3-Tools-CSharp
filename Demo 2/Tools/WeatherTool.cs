using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tools
{
    public class WeatherTool : ITool
    {
        public string Name => "get_weather";

        public async Task<string> ExecuteAsync(string argumentsJson)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // 🔹 Hace que "location" se reconozca aunque venga en minúsculas
                };

                var arguments = JsonSerializer.Deserialize<WeatherArguments>(argumentsJson, options);
                if (arguments == null || string.IsNullOrEmpty(arguments.Location))
                    return "⚠️ Error: No se proporcionó una ubicación válida.";

                return await GetWeatherAsync(arguments.Location);
            }
            catch (Exception ex)
            {
                return $"❌ Error al procesar los argumentos: {ex.Message}";
            }
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
            [JsonPropertyName("location")]  // 🔹 Se asegura de que "location" se mapea correctamente
            public string? Location { get; set; }
        }
    }
}
