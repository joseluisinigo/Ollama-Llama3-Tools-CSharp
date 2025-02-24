using System;
using Tools.Prompts;
using Tools.Messages;
using Serilog;

namespace Tools
{
    public static class ConfigurationManager
    {
      public static object GetRequestBody(string model, string toolName)
{
    Log.Information($"⚙️ Creando RequestBody para modelo: {model}, herramienta: {toolName}");

    var tools = ToolDefinitionManager.GetAllToolDefinitions();

    return new
    {
        model = model,  // 🔹 ASEGÚRATE DE QUE ESTO NO SEA FIJO COMO "mistral"
        messages = new[]
        {
            new { role = "system", content = PromptManager.GetSystemPrompt() },
            new { role = "user", content = MessageManager.GetUserMessage(toolName) }
        },
        tools = tools.Length > 0 ? tools : null,  
        tool_choice = "required",  
        stream = false
    };
}

    }
}
