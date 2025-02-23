using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools
{
    public class StockPriceTool : ITool
    {
        public string Name => "get_stock_price";

        public async Task<string> ExecuteAsync(string argumentsJson)
        {
            var arguments = JsonSerializer.Deserialize<StockArguments>(argumentsJson);
            if (arguments?.Symbol == null)
                return "⚠️ Error: No se proporcionó un símbolo de acción válido.";

            return await GetStockPriceAsync(arguments.Symbol);
        }

        private async Task<string> GetStockPriceAsync(string symbol)
        {
            await Task.Delay(500);
            var random = new Random();
            var price = random.Next(100, 500) + random.NextDouble();
            return $"El precio actual de {symbol} es ${price:F2}";
        }

        private class StockArguments
        {
            public string? Symbol { get; set; }
        }
    }
}
