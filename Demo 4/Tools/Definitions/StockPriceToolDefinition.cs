using System.Collections.Generic;

namespace Tools.Definitions
{
    public static class StockPriceToolDefinition
    {
        public static object GetDefinition()
        {
            return new
            {
                name = "get_stock_price",
                description = "Obtiene el precio actual de una acción",
                parameters = new
                {
                    type = "object",
                    properties = new Dictionary<string, object>
                    {
                        { "symbol", new { type = "string", description = "El símbolo de la acción (por ejemplo, AAPL, GOOGL)" } }
                    },
                    required = new[] { "symbol" }
                }
            };
        }
    }
}
