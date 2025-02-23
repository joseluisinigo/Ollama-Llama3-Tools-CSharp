using System;
using Tools.Prompts;
using Tools.Messages;

namespace Tools
{
    public static class ConfigurationManager
    {
        public static object GetRequestBody(string model, string toolName)
        {
            var tools = ToolDefinitionManager.GetAllToolDefinitions();

            return new
            {
                model,
                messages = new[]
                {
                    new { role = "system", content = PromptManager.GetSystemPrompt() },
                    new { role = "user", content = MessageManager.GetUserMessage(toolName) }
                },
                tools = tools.Length > 0 ? tools : null,  // Evita enviar un array vacío
                stream = false
            };
        }
    }
}
