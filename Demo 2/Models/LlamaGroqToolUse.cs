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
            await HandleRequestAsync(model, null);
        }

        public async Task HandleRequestAsync(string model, string? toolName)
        {
            int attempts = 0;
            while (true)
            {
                attempts++;
                string responseString = await SendRequestAsync(model, ConfigurationManager.GetRequestBody(model, toolName ?? "get_weather"));

                if (string.IsNullOrEmpty(responseString))
                {
                    continue;
                }

                var responseJson = JsonDocument.Parse(responseString);
                if (!responseJson.RootElement.TryGetProperty("message", out var messageElement))
                {
                    continue;
                }

                if (!messageElement.TryGetProperty("tool_calls", out var toolCallsElement) || toolCallsElement.GetArrayLength() == 0)
                {
                    Log.Information($"🔄 Intento {attempts}: No se han solicitado herramientas. Reintentando...");
                    continue;
                }

                Log.Information($"✅ Intento {attempts}: Se detectaron herramientas en la respuesta.");

                bool executedSuccessfully = false;

                foreach (var toolCall in toolCallsElement.EnumerateArray())
                {
                    var functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(toolName) && functionName != toolName) continue;

                    var argumentsJson = toolCall.GetProperty("function").GetProperty("arguments").ToString();
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
                        Log.Warning($"❌ Intento {attempts}: Respuesta inválida de la herramienta '{functionName}'. Reintentando...");
                        continue;
                    }

                    Log.Information($"✅ Resultado de '{functionName}': {result}");
                    Console.WriteLine($"✅ Resultado de la herramienta '{functionName}': {result}");
                    executedSuccessfully = true;
                    break; // Sale del bucle cuando obtiene un resultado válido
                }

                if (executedSuccessfully) break;
            }
        }
    }
}
