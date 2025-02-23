using System;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Tools;

namespace Models
{
    public class LlamaGroqToolUse : OllamaModelBase
    {
        public override async Task HandleRequestAsync(string model)
        {
            Log.Information("📡 Enviando solicitud a Ollama para el modelo {model}...", model);
            var requestBody = ConfigurationManager.GetRequestBody(model, "get_weather");
            string responseString = await SendRequestAsync(model, requestBody);

            if (string.IsNullOrEmpty(responseString))
            {
                Log.Warning("⚠️ No se recibió respuesta de Ollama.");
                return;
            }

            Log.Information("📩 Respuesta de Ollama recibida: {response}", responseString);
            var responseParts = responseString.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in responseParts)
            {
                try
                {
                    var responseJson = JsonDocument.Parse(part);
                    if (!responseJson.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        Log.Warning("⚠️ La respuesta no contiene 'message'.");
                        continue;
                    }

                    if (!messageElement.TryGetProperty("tool_calls", out var toolCallsElement))
                    {
                        Log.Warning("⚠️ No se han solicitado herramientas en la respuesta.");
                        continue;
                    }

                    Log.Information("🔧 Se detectaron herramientas en la respuesta.");
                    foreach (var toolCall in toolCallsElement.EnumerateArray())
                    {
                        var functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
                        if (string.IsNullOrEmpty(functionName))
                        {
                            Log.Warning("⚠️ Se recibió una llamada a una herramienta sin nombre válido.");
                            continue;
                        }

                        Log.Information("🛠️ Llama3-Groq solicitó la herramienta '{functionName}'", functionName);
                        var tool = ToolManager.GetToolByName(functionName);
                        if (tool == null)
                        {
                            Log.Error("❌ No se encontró la herramienta '{functionName}' en ToolManager.", functionName);
                            continue;
                        }

                        var argumentsJson = toolCall.GetProperty("function").GetProperty("arguments").ToString();
                        Log.Information("🔍 Ejecutando herramienta '{functionName}' con argumentos: {argumentsJson}", functionName, argumentsJson);
                        var result = await tool.ExecuteAsync(argumentsJson);

                        Log.Information("✅ Resultado de '{functionName}': {result}", functionName, result);
                        Console.WriteLine($"✅ Resultado de la herramienta '{functionName}': {result}");
                    }
                }
                catch (JsonException ex)
                {
                    Log.Error("❌ Error en el JSON recibido: {error}. Datos recibidos: {json}", ex.Message, part);
                }
            }
        }
    }
}
