using System.Collections.Generic;

namespace Tools.Definitions
{
    public static class WeatherToolDefinition
    {
        public static object GetDefinition()
        {
            return new
            {
                name = "get_weather",
                description = "Obtiene el clima actual de una ubicación dada",
                parameters = new
                {
                    type = "object",
                    properties = new Dictionary<string, object>
                    {
                        { "location", new { type = "string", description = "El nombre de la ciudad o ubicación" } }
                    },
                    required = new[] { "location" }
                }
            };
        }
    }
}
