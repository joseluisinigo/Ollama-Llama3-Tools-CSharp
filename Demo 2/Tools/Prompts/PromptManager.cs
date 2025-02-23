using System.Collections.Generic;

namespace Tools.Prompts
{
    public static class PromptManager
    {
        private static readonly Dictionary<string, string> prompts = new Dictionary<string, string>
        {
            { "get_weather", WeatherPrompt.GetPrompt() },
            { "get_stock_price", StockPricePrompt.GetPrompt() }
        };

        public static string GetSystemPrompt()
        {
            return "Eres un asistente con acceso a herramientas. Siempre que puedas responder usando una herramienta, debes usarla. No respondas directamente si existe una herramienta que pueda proporcionar la información.";
        }

        public static string GetPromptForTool(string toolName)
        {
            return prompts.ContainsKey(toolName) ? prompts[toolName] : GetSystemPrompt();
        }
    }
}
