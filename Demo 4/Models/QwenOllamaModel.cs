using System;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Tools;
using Tools.Prompts;
using Tools.Messages;

namespace Models
{
    public class QwenOllamaModel : OllamaModelBase
    {
        public override async Task HandleRequestAsync(string model)
        {
            await HandleRequestAsync(model, null);
        }

        public override async Task HandleRequestAsync(string model, string? toolName)
        {
            int attempts = 0;
            while (true)
            {
                attempts++;

                var requestBody = new
                {
                    model,
                    messages = new[]
                    {
                        new { role = "system", content = PromptManager.GetSystemPrompt() },
                        new { role = "user", content = $"Usa la herramienta '{toolName ?? "get_weather"}' para responder." }
                    },
                    tools = ToolDefinitionManager.GetAllToolDefinitions(),
                    tool_choice = "required",
                    stream = false
                };

                string responseString = await SendRequestAsync(model, requestBody);

                if (string.IsNullOrEmpty(responseString))
                {
                    Log.Warning($"⚠️ Intento {attempts}: No se recibió respuesta válida de Qwen.");
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
                    
                    // Si se seleccionó una herramienta específica, ignoramos cualquier otra
                    if (!string.IsNullOrEmpty(toolName) && functionName != toolName) 
                    {
                        Log.Warning($"⚠️ El modelo intentó ejecutar '{functionName}', pero se esperaba '{toolName}'. Reintentando...");
                        continue;
                    }

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
                    break;
                }

                if (executedSuccessfully) break;
            }
        }
    }
}
