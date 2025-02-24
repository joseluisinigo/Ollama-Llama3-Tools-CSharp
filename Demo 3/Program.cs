using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Models;
using Tools;
using Utils; // Asegura que CodeExporter esté disponible

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("\n👋 Saliendo...");
            eventArgs.Cancel = true;
            Environment.Exit(0);
        };

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
                    Console.WriteLine("\n📤 Exportando código...");
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

    static void ExportProjectCode()
    {
        Log.Information("📤 Exportando código...");

        try
        {
            CodeExporter.ExportCodeToTxt(Directory.GetCurrentDirectory());
            Console.WriteLine("\n✅ Código exportado correctamente a 'code.txt'");
        }
        catch (Exception ex)
        {
            Log.Error("❌ Error al exportar código: {error}", ex.Message);
        }
    }

    static async Task RunOllamaModel()
    {
        Log.Information("🚀 Iniciando la interacción con Ollama...");

        var modelHandlers = new Dictionary<string, OllamaModelBase>
        {
            { "qwen2.5", new QwenOllamaModel() },
            { "mistral", new MistralToolUse() },
            { "gemma", new GenericOllamaModel() },
            { "llama3.2", new Llama3ToolUse() },
            { "llama3-groq-tool-use", new LlamaGroqToolUse() }
        };

        Console.WriteLine("\nModelos disponibles en Ollama:");
        int index = 1;
        foreach (var model in modelHandlers.Keys)
        {
            Console.WriteLine($"{index}. {model} (Datos generales y conversación.)");
            index++;
        }

        Console.Write("\nElige un modelo (1-{0}) o presiona 'v' para volver: ", modelHandlers.Count);

        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.V) return;

        if (!int.TryParse(key.KeyChar.ToString(), out int choice) || choice < 1 || choice > modelHandlers.Count)
        {
            Console.WriteLine("\n❌ Opción no válida.");
            return;
        }

        string selectedModel = modelHandlers.Keys.ElementAt(choice - 1);
        Console.WriteLine($"\n✅ Modelo seleccionado: {selectedModel}");

        if (modelHandlers.ContainsKey(selectedModel))
        {
            await ShowToolMenu(selectedModel, modelHandlers[selectedModel]);
        }
        else
        {
            Console.WriteLine("❌ Error: Modelo no encontrado.");
        }
    }

    static async Task ShowToolMenu(string modelName, OllamaModelBase modelHandler)
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
            Console.WriteLine(); // Para evitar que el input quede pegado

            if (key.Key == ConsoleKey.V || key.KeyChar.ToString() == (tools.Count + 2).ToString())
            {
                Console.WriteLine("\n🔙 Volviendo al menú principal...");
                return;
            }

            if (!int.TryParse(key.KeyChar.ToString(), out int toolChoice) || toolChoice < 1 || toolChoice > tools.Count + 2)
            {
                Console.WriteLine("\n❌ Opción no válida.");
                continue;
            }

            if (toolChoice == tools.Count + 1)
            {
                await RunAllTools(modelName, modelHandler);
            }
            else if (toolChoice == tools.Count + 2)
            {
                return;
            }
            else
            {
                string selectedTool = tools[toolChoice - 1];
                Console.WriteLine($"\n🔧 Ejecutando herramienta: {selectedTool}");
                await modelHandler.HandleRequestAsync(modelName, selectedTool);
            }
        }
    }

    static async Task RunAllTools(string modelName, OllamaModelBase modelHandler)
    {
        foreach (var tool in GetToolList())
        {
            Console.WriteLine($"\n🔧 Ejecutando herramienta: {tool}");
            await modelHandler.HandleRequestAsync(modelName, tool);
        }
    }

    static List<string> GetToolList()
    {
        return ToolDefinitionManager.GetAllToolDefinitions()
            .Select(tool => tool.GetType().GetProperty("name")?.GetValue(tool)?.ToString() ?? "Desconocido")
            .ToList();
    }
}
