using System;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Tools; // Importamos herramientas

namespace Models
{
    public class LlamaGroqToolUse : OllamaModelBase
    {
        public override async Task HandleRequestAsync(string model)
        {
            var tools = new[]
            {
                new
                {
                    name = "get_weather",
                    description = "Obtiene el clima actual de una ubicación dada",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            location = new
                            {
                                type = "string",
                                description = "El nombre de la ciudad o ubicación"
                            }
                        },
                        required = new[] { "location" }
                    }
                }
            };

            var messages = new[]
            {
                new { role = "system", content = "Tienes acceso a herramientas. Usa 'get_weather' si preguntan por el clima." },
                new { role = "user", content = "¿Cuál es el clima actual en Madrid?" }
            };

            var requestBody = new { model, messages, tools };
            string responseString = await SendRequestAsync(model, requestBody);

            if (string.IsNullOrEmpty(responseString))
            {
                Log.Warning("⚠️ No se recibió respuesta válida de Ollama.");
                return;
            }

            var responseParts = responseString.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in responseParts)
            {
                try
                {
                    var responseJson = JsonDocument.Parse(part);

                    if (responseJson.RootElement.TryGetProperty("message", out var messageElement) &&
                        messageElement.TryGetProperty("tool_calls", out var toolCallsElement))
                    {
                        foreach (var toolCall in toolCallsElement.EnumerateArray())
                        {
                            var functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
                            var argumentsJson = toolCall.GetProperty("function").GetProperty("arguments").ToString();

                            var tool = ToolManager.GetToolByName(functionName);
                            if (tool != null)
                            {
                                var result = await tool.ExecuteAsync(argumentsJson);
                                Log.Information("✅ {model} ejecutó '{functionName}' con resultado: {result}", model, functionName, result);
                                Console.WriteLine($"✅ Resultado de la herramienta '{functionName}': {result}");
                            }
                            else
                            {
                                Log.Warning("⚠️ No se encontró la herramienta '{functionName}'.", functionName);
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Log.Error("❌ Error en el JSON: {error}. Datos recibidos: {json}", ex.Message, part);
                }
            }
        }
    }
}
