using System;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Tools;

namespace Models
{
    public class Llama3ToolUse : OllamaModelBase
    {
        public override async Task HandleRequestAsync(string model)
        {
            await HandleRequestAsync(model, null);
        }

        public override async Task HandleRequestAsync(string model, string? toolName)
        {
            Log.Information($"🚀 Ejecutando modelo {model} con herramienta: {toolName ?? "ninguna"}");

            string responseString = await SendRequestAsync(model, ConfigurationManager.GetRequestBody(model, toolName ?? "get_weather"));

            if (string.IsNullOrEmpty(responseString))
            {
                Log.Warning("⚠️ No se recibió respuesta válida de Ollama.");
                return;
            }

            var responseJson = JsonDocument.Parse(responseString);
            if (!responseJson.RootElement.TryGetProperty("message", out var messageElement))
            {
                Log.Warning("⚠️ No se encontró la clave 'message' en la respuesta de Ollama.");
                return;
            }

            // Si hay tool_calls, ejecutamos la herramienta correspondiente
            if (messageElement.TryGetProperty("tool_calls", out var toolCallsElement) && toolCallsElement.GetArrayLength() > 0)
            {
                Log.Information($"✅ Se detectaron herramientas en la respuesta de {model}.");
                
                foreach (var toolCall in toolCallsElement.EnumerateArray())
                {
                    string functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
                    string argumentsJson = toolCall.GetProperty("function").GetProperty("arguments").ToString();

                    var tool = ToolManager.GetToolByName(functionName);
                    if (tool == null)
                    {
                        Log.Error($"❌ No se encontró la herramienta '{functionName}' en ToolManager.");
                        continue;
                    }

                    Log.Information($"🔍 Ejecutando herramienta '{functionName}' con argumentos: {argumentsJson}");
                    var result = await tool.ExecuteAsync(argumentsJson);

                    if (result.Contains("⚠️ Error"))
                    {
                        Log.Warning($"❌ Respuesta inválida de la herramienta '{functionName}'. Reintentando...");
                        continue;
                    }

                    Log.Information($"✅ Resultado de '{functionName}': {result}");
                    Console.WriteLine($"✅ Resultado de la herramienta '{functionName}': {result}");
                }
            }
            else
            {
                // Si no hay tool_calls, imprimimos el contenido del mensaje
                string content = messageElement.GetProperty("content").GetString() ?? "(sin contenido)";
                Log.Information("📩 Respuesta de {model}: {response}", model, content);
                Console.WriteLine($"🔹 Respuesta de {model}: {content}");
            }
        }
    }
}
