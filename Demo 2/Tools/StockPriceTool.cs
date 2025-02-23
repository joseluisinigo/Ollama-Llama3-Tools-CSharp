using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tools
{
    public class StockPriceTool : ITool
    {
        public string Name => "get_stock_price";

        public async Task<string> ExecuteAsync(string argumentsJson)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // 🔹 Asegura que "symbol" se reconozca correctamente
                };

                var arguments = JsonSerializer.Deserialize<StockArguments>(argumentsJson, options);
                if (arguments == null || string.IsNullOrEmpty(arguments.Symbol))
                    return "⚠️ Error: No se proporcionó un símbolo de acción válido.";

                return await GetStockPriceAsync(arguments.Symbol);
            }
            catch (Exception ex)
            {
                return $"❌ Error al procesar los argumentos: {ex.Message}";
            }
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
            [JsonPropertyName("symbol")]
            public string? Symbol { get; set; }
        }
    }
}
