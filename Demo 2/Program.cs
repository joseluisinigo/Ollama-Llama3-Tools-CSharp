using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using Models;
using Utils;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        while (true)
        {
            Console.WriteLine("\n📌 Opciones disponibles:");
            Console.WriteLine("1. Elegir y ejecutar un modelo de Ollama");
            Console.WriteLine("2. Exportar código del proyecto a 'code.txt'");
            Console.WriteLine("3. Salir");
            Console.Write("\nSelecciona una opción: ");

            string menuChoice = Console.ReadLine();

            switch (menuChoice)
            {
                case "1":
                    await RunOllamaModel();
                    break;
                case "2":
                    ExportProjectCode();
                    break;
                case "3":
                    Console.WriteLine("👋 Saliendo...");
                    return;
                default:
                    Console.WriteLine("❌ Opción no válida, intenta de nuevo.");
                    break;
            }
        }
    }

    static async Task RunOllamaModel()
    {
        Log.Information("🚀 Iniciando la interacción con Ollama...");

        var modelHandlers = new Dictionary<string, OllamaModelBase>
        {
            { "qwen2.5", new GenericOllamaModel() },
            { "mistral", new GenericOllamaModel() },
            { "gemma", new GenericOllamaModel() },
            { "llama3.2", new GenericOllamaModel() },
            { "llama3-groq-tool-use", new LlamaGroqToolUse() }
        };

        Console.WriteLine("\nModelos disponibles en Ollama:");
        int index = 1;
        foreach (var model in modelHandlers.Keys)
        {
            Console.WriteLine($"{index}. {model}");
            index++;
        }

        Console.Write("\nElige un modelo (1-5): ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > modelHandlers.Count)
        {
            Console.Write("Selección no válida. Elige un número entre 1 y 5: ");
        }

        string selectedModel = new List<string>(modelHandlers.Keys)[choice - 1];
        Console.WriteLine($"✅ Modelo seleccionado: {selectedModel}");

        await modelHandlers[selectedModel].HandleRequestAsync(selectedModel);
    }

    static void ExportProjectCode()
    {
        Console.WriteLine("📤 Exportando código...");
        CodeExporter.ExportCodeToTxt(Directory.GetCurrentDirectory());
    }
}
