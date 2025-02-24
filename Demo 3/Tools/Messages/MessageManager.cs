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
            return toolName switch
            {
                "get_weather" => "Usa la herramienta 'get_weather' para decirme el clima actual en Madrid.",
                "get_stock_price" => "Usa la herramienta 'get_stock_price' para decirme el precio de la acción de Apple (AAPL).",
                _ => "Usa la herramienta adecuada para responder mi pregunta."
            };
        }
    }
}
