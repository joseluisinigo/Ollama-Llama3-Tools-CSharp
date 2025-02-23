using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Models; // Importamos la carpeta Models

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("🚀 Iniciando la interacción con Ollama...");

        var modelHandlers = new Dictionary<string, OllamaModelBase>
        {
            { "qwen2.5", new GenericOllamaModel() },
            { "mistral", new GenericOllamaModel() },
            { "gemma", new GenericOllamaModel() },
            { "llama3.2", new GenericOllamaModel() },
            { "llama3-groq-tool-use", new LlamaGroqToolUse() }
        };

        Console.WriteLine("Modelos disponibles en Ollama:");
        int index = 1;
        foreach (var model in modelHandlers.Keys)
        {
            Console.WriteLine($"{index}. {model}");
            index++;
        }

        Console.Write("Elige un modelo (1-5): ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > modelHandlers.Count)
        {
            Console.Write("Selección no válida. Elige un número entre 1 y 5: ");
        }

        string selectedModel = new List<string>(modelHandlers.Keys)[choice - 1];
        Console.WriteLine($"✅ Modelo seleccionado: {selectedModel}");

        await modelHandlers[selectedModel].HandleRequestAsync(selectedModel);
    }
}
