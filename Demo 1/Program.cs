using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main()
    {
        string model = "llama3.1";  
        string prompt = "¿Cuál es el clima en Toronto?";

        var functionDefinition = new
        {
            name = "get_current_weather",
            description = "Obtiene el clima actual de una ciudad.",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    city = new { type = "string", description = "El nombre de la ciudad" }
                },
                required = new[] { "city" }
            }
        };

        var requestBody = new
        {
            model = model,
            messages = new[] { new { role = "user", content = prompt } },
            tools = new[] { new { type = "function", function = functionDefinition } },
            stream = false // Desactivamos streaming para recibir el JSON completo
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("http://localhost:11434/api/chat"),
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine("🔹 Respuesta del modelo:");
            Console.WriteLine(responseContent);

            // Configurar opciones de deserialización para que ignore mayúsculas y minúsculas
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserializar la respuesta
            var jsonResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent, options);

            if (jsonResponse?.Message?.ToolCalls != null && jsonResponse.Message.ToolCalls.Length > 0)
            {
                foreach (var toolCall in jsonResponse.Message.ToolCalls)
                {
                    if (toolCall.Function?.Name == "get_current_weather")
                    {
                        string city = toolCall.Function.Arguments?.City ?? "desconocida";
                        Console.WriteLine($"🌦️ Llamando a get_current_weather con ciudad: {city}");
                        
                        // Simulación de llamada a API del clima
                        string weatherInfo = GetCurrentWeather(city);
                        Console.WriteLine($"✅ Clima en {city}: {weatherInfo}");
                    }
                }
            }
            else
            {
                Console.WriteLine("⚠️ No se detectaron tool_calls en la respuesta.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al conectar con Ollama: {ex.Message}");
        }
    }

    // Simula una función para obtener el clima
    static string GetCurrentWeather(string city)
    {
        return $"El clima en {city} es soleado con 25°C.";
    }
}

// 🔥 **Corrección en las clases para reflejar el JSON de Ollama**
class OllamaResponse
{
    [JsonPropertyName("message")]
    public MessageContent? Message { get; set; }
}

class MessageContent
{
    [JsonPropertyName("tool_calls")]
    public ToolCall[]? ToolCalls { get; set; }
}

class ToolCall
{
    [JsonPropertyName("function")]
    public FunctionData? Function { get; set; }
}

class FunctionData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public ToolArguments? Arguments { get; set; }
}

class ToolArguments
{
    [JsonPropertyName("city")]
    public string? City { get; set; }
}
