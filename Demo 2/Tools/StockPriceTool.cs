using System;
using System.Threading.Tasks;

public class StockPriceTool
{
    public static Task<string> GetStockPriceAsync(string symbol)
    {
        // Simulación de obtención de precio de una acción
        // En un caso real, aquí se haría una llamada a una API financiera
        var random = new Random();
        var price = random.Next(100, 500) + random.NextDouble();
        return Task.FromResult($"El precio actual de {symbol} es ${price:F2}");
    }
}
