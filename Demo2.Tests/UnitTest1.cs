using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Tools;
using System.Text.Json;
using Models;  // <- Importa correctamente el namespace
using Xunit;
using System.Threading.Tasks;

namespace Demo2.Tests
{
    public class ToolTests
    {
        [Fact]
        public async Task WeatherTool_Returns_Valid_Weather()
        {
            // Arrange
            var tool = new WeatherTool();
            var arguments = JsonSerializer.Serialize(new { location = "Madrid" });
            
            // Act
            var result = await tool.ExecuteAsync(arguments);
            Console.WriteLine($"Resultado: {result}");
            
            // Assert
            Assert.Contains("El clima actual en Madrid", result);
        }

        [Fact]
        public async Task WeatherTool_Returns_Error_When_Location_Is_Missing()
        {
            // Arrange
            var tool = new WeatherTool();
            var arguments = JsonSerializer.Serialize(new { location = "" });
            
            // Act
            var result = await tool.ExecuteAsync(arguments);
            Console.WriteLine($"Resultado: {result}");
            
            // Assert
            Assert.Contains("⚠️ Error", result);
        }

        [Fact]
        public async Task StockPriceTool_Returns_Valid_Stock_Price()
        {
            // Arrange
            var tool = new StockPriceTool();
            var arguments = JsonSerializer.Serialize(new { symbol = "AAPL" });
            
            // Act
            var result = await tool.ExecuteAsync(arguments);
            Console.WriteLine($"Resultado: {result}");
            
            // Assert
            Assert.Contains("El precio actual de AAPL", result);
        }

        [Fact]
        public async Task StockPriceTool_Returns_Error_When_Symbol_Is_Missing()
        {
            // Arrange
            var tool = new StockPriceTool();
            var arguments = JsonSerializer.Serialize(new { symbol = "" });
            
            // Act
            var result = await tool.ExecuteAsync(arguments);
            Console.WriteLine($"Resultado: {result}");
            
            // Assert
            Assert.Contains("⚠️ Error", result);
        }
    }

    public class LlamaGroqToolUseTests
    {
        [Fact]
        public async Task HandleRequestAsync_WeatherTool_Executes_Successfully()
        {
            // Arrange
            var modelHandler = new LlamaGroqToolUse();
            
            // Act
            var exception = await Record.ExceptionAsync(async () => 
                await modelHandler.HandleRequestAsync("llama3-groq-tool-use", "get_weather"));
            Console.WriteLine("Prueba WeatherTool ejecutada correctamente");
            
            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task HandleRequestAsync_StockPriceTool_Executes_Successfully()
        {
            // Arrange
            var modelHandler = new LlamaGroqToolUse();
            
            // Act
            var exception = await Record.ExceptionAsync(async () => 
                await modelHandler.HandleRequestAsync("llama3-groq-tool-use", "get_stock_price"));
            Console.WriteLine("Prueba StockPriceTool ejecutada correctamente");
            
            // Assert
            Assert.Null(exception);
        }
    }
}
