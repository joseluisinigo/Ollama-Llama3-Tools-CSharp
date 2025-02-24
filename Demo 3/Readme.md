
# 🦙 **Ollama Tools in C#**
## 🚀 **Integrating Ollama with Function Calling in C#**

Yes, **Ollama** supports tools in **C#**, allowing the AI to call your own methods when needed.  
Additionally, **LM Studio** is compatible with Ollama models, but it currently lacks a native interface for defining tools.  
However, you can still interact with LM Studio’s API from C# in a similar way.

---

## 🔹 **1. Recommended (LM) Models**
For tools in **Ollama**, you need a model with *function calling* capabilities. Recommended models:
- **Mistral (`mistral:latest`)** → Small and fast, efficient for general tasks.
- **Gemma (`gemma:latest`)** → More powerful for reasoning tasks.
- **Qwen (`qwen2.5`)** → Optimized for function calling.
- **Llama 3 (when available in Ollama)** → Expected to improve tool usage.

To install a model in **Ollama**, use:
```bash
ollama pull mistral
```
Or another:
```bash
ollama pull qwen2.5
```

---

## 🔹 **2. C# Integration with Ollama Tools**
Here is an **example in C#** using **Ollama tools** with a structured logging system.  
This allows **Ollama to call functions dynamically** based on user queries.

### 📌 **Example: Getting Current Time with a Tool in C#**
This code defines a method (`GetCurrentTime`) that Ollama can invoke when required.

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing Ollama with tools...");

        var tools = new[]
        {
            new
            {
                name = "get_current_time",
                description = "Returns the current time in HH:mm:ss format"
            }
        };

        var messages = new[]
        {
            new { role = "user", content = "What time is it now?" }
        };

        var requestBody = new
        {
            model = "mistral",
            messages,
            tools
        };

        string jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:11434/api/generate", content);
        string responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine("🔹 Ollama Response:");
        Console.WriteLine(responseString);

        if (responseString.Contains("get_current_time"))
        {
            string result = GetCurrentTime();
            Console.WriteLine($"✅ The AI requested the time: {result}");
        }
    }

    static string GetCurrentTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}
```

---

## 🔹 **3. Improvements and Enhancements**
### ✅ **Major Refactoring & Modularization**
- 🏗 **Separated tools into individual files in `Tools/Definitions/`.**
- 🏗 **Created `PromptManager.cs` and `MessageManager.cs` to manage prompts dynamically.**
- 🏗 **Added `ConfigurationManager.cs` to handle request structuring.**
- 🏗 **Introduced a clean and extendable way to integrate new tools.**
- 🏗 **Added a feature to export all project code to `code.txt`.**

### 📌 **New Feature: Export Code**
To export all source code into `code.txt`, run:
```sh
dotnet run
```
And select **option 2**.

---

## 🔹 **4. Viewing Logs & Debugging Interactions**
1. **Run Ollama in server mode**:
   ```bash
   ollama serve
   ```
2. **Enable logging in C#** using `Serilog`:
   ```csharp
   Log.Information("Ollama Response: {responseString}");
   ```
3. **Check generated logs in `logs.txt`**.

---

## 🔹 **5. Using LM Studio Instead of Ollama**
LM Studio supports **running models locally**, but it **does not support function calling**.  
If using LM Studio, change the API endpoint in your code:
```csharp
var response = await client.PostAsync("http://localhost:1234/v1/chat/completions", content);
```

---

## 🎯 **Conclusion**
- **Ollama supports function calling natively in C#.** 🚀
- **LM Studio does not, but can still process requests.**
- **The project is now modular, extendable, and fully structured!** 🔥

---

# 🦙 **Herramientas de Ollama en C#**
## 🚀 **Integración de Ollama con Function Calling en C#**

Sí, **Ollama** admite herramientas en **C#**, lo que permite que la IA llame a tus propios métodos según sea necesario.  
Además, **LM Studio** es compatible con los modelos de Ollama, aunque actualmente no tiene una interfaz nativa para definir herramientas.  
Sin embargo, puedes interactuar con la API de LM Studio desde C# de manera similar.

---

## 🔹 **1. Modelos (LM) recomendados**
Para herramientas en **Ollama**, necesitas un modelo con capacidades de *function calling*.  
Algunos recomendados son:
- **Mistral (`mistral:latest`)** → Pequeño y rápido, eficiente en tareas generales.
- **Gemma (`gemma:latest`)** → Más potente para tareas de razonamiento.
- **Qwen (`qwen2.5`)** → Optimizado para *function calling*.
- **Llama 3 (cuando esté disponible en Ollama)** → Mejoras esperadas en el uso de herramientas.

Para instalar un modelo en **Ollama**, usa:
```bash
ollama pull mistral
```
O si prefieres otro:
```bash
ollama pull qwen2.5
```

---

## 🔹 **2. Integración en C# con Ollama Tools**
Aquí tienes un **ejemplo en C#** usando **Ollama Tools** con un sistema de logs estructurado.  
Esto permite que **Ollama llame funciones dinámicamente** según las consultas del usuario.

### 📌 **Ejemplo: Obtener la Hora con una Herramienta en C#**
```csharp
// (Mismo código de ejemplo que en la versión en inglés)
```

---

## 🔹 **3. Mejoras y Optimización**
### ✅ **Refactorización Completa y Modularización**
- 🏗 **Las herramientas ahora están separadas en `Tools/Definitions/`.**
- 🏗 **`PromptManager.cs` y `MessageManager.cs` gestionan los mensajes dinámicamente.**
- 🏗 **`ConfigurationManager.cs` maneja la estructura de las solicitudes.**
- 🏗 **Nueva función para exportar todo el código del proyecto a `code.txt`.**

### 📌 **Nueva Función: Exportar Código**
Para exportar todo el código fuente en `code.txt`, ejecuta:
```sh
dotnet run
```
Y selecciona **la opción 2**.

---

## 🔹 **4. Ver Logs y Depurar Interacciones**
1. **Ejecuta Ollama en modo servidor**:
   ```bash
   ollama serve
   ```
2. **Habilita logs en C#** con `Serilog`:
   ```csharp
   Log.Information("Respuesta de Ollama: {responseString}");
   ```
3. **Revisa los logs generados en `logs.txt`**.

---

## 🔹 **5. Usar LM Studio en Lugar de Ollama**
LM Studio permite **ejecutar modelos localmente**, pero **NO admite `function calling`**.  
Si usas LM Studio, cambia la API en el código:
```csharp
var response = await client.PostAsync("http://localhost:1234/v1/chat/completions", content);
```

---

## 🎯 **Conclusión**
- **Ollama permite `function calling` en C# de forma nativa.** 🚀
- **LM Studio no, pero puede usarse para consultas normales.**
- **El proyecto ahora es modular, escalable y bien estructurado.** 🔥
```
