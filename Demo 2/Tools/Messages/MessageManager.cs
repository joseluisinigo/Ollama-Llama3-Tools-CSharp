using System.Collections.Generic;

namespace Tools.Messages
{
    public static class MessageManager
    {
        private static readonly Dictionary<string, string> messages = new Dictionary<string, string>
        {
            { "get_weather", WeatherMessage.GetMessage() },
            { "get_stock_price", StockPriceMessage.GetMessage() }
        };

        public static string GetUserMessage(string toolName)
        {
            return messages.ContainsKey(toolName) 
                ? messages[toolName] 
                : "Dime la información que necesito usando la herramienta adecuada.";
        }
    }
}
