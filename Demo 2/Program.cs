using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Models;
using Utils;
using Tools;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Console.TreatControlCAsInput = true;

        while (true)
        {
            Console.WriteLine("\n📌 Opciones disponibles:");
            Console.WriteLine("1. Elegir y ejecutar un modelo de Ollama");
            Console.WriteLine("2. Exportar código del proyecto a 'code.txt'");
            Console.WriteLine("3. Salir (o presiona 'q' para salir inmediatamente)");

            Console.Write("\nSelecciona una opción: ");

            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Q)
            {
                Console.WriteLine("\n👋 Saliendo...");
                return;
            }

            string menuChoice = key.KeyChar.ToString();

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
                    Console.WriteLine("\n❌ Opción no válida, intenta de nuevo.");
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
            Console.WriteLine($"{index}. {model} (Datos generales y conversación.)");
            index++;
        }

        Console.Write("\nElige un modelo (1-5) o presiona 'v' para volver: ");

        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.V) return;

        int choice;
        if (!int.TryParse(key.KeyChar.ToString(), out choice) || choice < 1 || choice > modelHandlers.Count)
        {
            Console.WriteLine("\n❌ Opción no válida.");
            return;
        }

        string selectedModel = modelHandlers.Keys.ElementAt(choice - 1);
        Console.WriteLine($"\n✅ Modelo seleccionado: {selectedModel}");

        if (selectedModel == "llama3-groq-tool-use")
        {
            await ShowToolMenu();
        }
        else
        {
            await modelHandlers[selectedModel].HandleRequestAsync(selectedModel);
        }
    }

    static async Task ShowToolMenu()
    {
        var tools = GetToolList();
        if (tools.Count == 0)
        {
            Console.WriteLine("⚠️ No hay herramientas disponibles.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n🔧 Herramientas disponibles:");
            for (int i = 0; i < tools.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tools[i]}");
            }
            Console.WriteLine($"{tools.Count + 1}. 🛠️ Ejecutar todas");
            Console.WriteLine($"{tools.Count + 2}. 🔙 Volver al menú anterior");

            Console.Write("\nElige una herramienta (1-{0}) o presiona 'v' para volver: ", tools.Count + 2);

            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.V) return;

            int toolChoice;
            if (!int.TryParse(key.KeyChar.ToString(), out toolChoice) || toolChoice < 1 || toolChoice > tools.Count + 2)
            {
                Console.WriteLine("\n❌ Opción no válida.");
                continue;
            }

            if (toolChoice == tools.Count + 1)
            {
                Console.WriteLine("\n🛠️ Ejecutando todas las herramientas disponibles...");
                await RunAllTools();
            }
            else if (toolChoice == tools.Count + 2)
            {
                return;
            }
            else
            {
                string selectedTool = tools[toolChoice - 1];
                Console.WriteLine($"\n🔧 Ejecutando herramienta: {selectedTool}");
                var modelHandler = new LlamaGroqToolUse();
                await modelHandler.HandleRequestAsync("llama3-groq-tool-use", selectedTool);
            }
        }
    }

    static async Task RunAllTools()
    {
        foreach (var tool in GetToolList())
        {
            Console.WriteLine($"\n🔧 Ejecutando herramienta: {tool}");
            var modelHandler = new LlamaGroqToolUse();
            await modelHandler.HandleRequestAsync("llama3-groq-tool-use", tool);
        }
    }

    static void ExportProjectCode()
    {
        Console.WriteLine("\n📤 Exportando código...");
        CodeExporter.ExportCodeToTxt(Directory.GetCurrentDirectory());
    }

    static List<string> GetToolList()
    {
        return ToolDefinitionManager.GetAllToolDefinitions()
            .Select(tool => tool.GetType().GetProperty("name")?.GetValue(tool)?.ToString() ?? "Desconocido")
            .ToList();
    }
}
