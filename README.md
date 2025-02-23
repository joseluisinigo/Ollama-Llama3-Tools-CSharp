# Ollama Function Calling with Llama 3.1 in C#

## 📝 Overview

This repository demonstrates how to use **function calling (tools)** in **Ollama** with **Llama 3.1** in a **C# project**.  
With this implementation, you can define **custom functions**, and Llama 3.1 will automatically decide when to call them.

## 🎯 Purpose

The goal of this project is to **integrate Llama 3.1 in Ollama** with **C#** and enable **function calling**,  
allowing the model to **trigger external functions** based on user queries.

---

## 🚀 Features

✅ **Uses Llama 3.1 with Ollama**  
✅ **Implements function calling (tools)**  
✅ **Written in C# for easy integration**  
✅ **Processes function calls dynamically**  
✅ **Customizable with more tools and APIs**  

---

## 📌 Requirements

Make sure you have the following installed:

- [.NET SDK (6.0 or later)](https://dotnet.microsoft.com/en-us/download)
- [Ollama](https://ollama.com/) installed and running (`ollama serve`)
- Llama 3.1 model (`ollama pull llama3.1`)

---

## 🛠️ Setup & Installation

### 1️⃣ **Clone the Repository**
```bash
git clone https://github.com/YOUR_GITHUB_USERNAME/Ollama-Llama3.1-FunctionCalling-CSharp.git
cd Ollama-Llama3.1-FunctionCalling-CSharp
```

### 2️⃣ **Install Required Packages**
In the project folder, install the OpenAI NuGet package:
```bash
dotnet add package OpenAI
```

### 3️⃣ **Start Ollama**
Ensure **Ollama is running** before executing the project:
```bash
ollama serve
```

If you haven’t downloaded **Llama 3.1**, pull it first:
```bash
ollama pull llama3.1
```

### 4️⃣ **Run the Project**
```bash
dotnet run
```

---

## 🏗️ How It Works

This project sends a **chat request** to Ollama with a **custom tool** (`get_current_weather`).  
Ollama detects when to use the function and **returns the function call in the response**.  
The C# code **detects and executes the function dynamically**.

### **🖥️ Example Flow**
1️⃣ **User asks:** _"What is the weather in Toronto?"_  
2️⃣ **Ollama detects that a tool should be used**  
3️⃣ **Ollama returns:**
```json
{
    "message": {
        "tool_calls": [
            {
                "function": {
                    "name": "get_current_weather",
                    "arguments": { "city": "Toronto" }
                }
            }
        ]
    }
}
```
4️⃣ **C# executes the function dynamically and returns a response:**  
```
🌦️ Calling get_current_weather with city: Toronto
✅ Weather in Toronto: Sunny with 25°C.
```

---

## 📝 Code Example

Here is the **C# implementation**:

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main()
    {
        string model = "llama3.1";  
        string prompt = "What is the weather in Toronto?";

        var functionDefinition = new
        {
            name = "get_current_weather",
            description = "Get the current weather for a city.",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    city = new { type = "string", description = "The name of the city" }
                },
                required = new[] { "city" }
            }
        };

        var requestBody = new
        {
            model = model,
            messages = new[] { new { role = "user", content = prompt } },
            tools = new[] { new { type = "function", function = functionDefinition } },
            stream = false
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("http://localhost:11434/api/chat"),
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine("🔹 Response from model:");
            Console.WriteLine(responseContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent, options);

            if (jsonResponse?.Message?.ToolCalls != null && jsonResponse.Message.ToolCalls.Length > 0)
            {
                foreach (var toolCall in jsonResponse.Message.ToolCalls)
                {
                    if (toolCall.Function?.Name == "get_current_weather")
                    {
                        string city = toolCall.Function.Arguments?.City ?? "unknown";
                        Console.WriteLine($"🌦️ Calling get_current_weather with city: {city}");
                        
                        string weatherInfo = GetCurrentWeather(city);
                        Console.WriteLine($"✅ Weather in {city}: {weatherInfo}");
                    }
                }
            }
            else
            {
                Console.WriteLine("⚠️ No tool calls detected.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error connecting to Ollama: {ex.Message}");
        }
    }

    static string GetCurrentWeather(string city)
    {
        return $"The weather in {city} is sunny with 25°C.";
    }
}

class OllamaResponse
{
    [JsonPropertyName("message")]
    public MessageContent? Message { get; set; }
}

class MessageContent
{
    [JsonPropertyName("tool_calls")]
    public ToolCall[]? ToolCalls { get; set; }
}

class ToolCall
{
    [JsonPropertyName("function")]
    public FunctionData? Function { get; set; }
}

class FunctionData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public ToolArguments? Arguments { get; set; }
}

class ToolArguments
{
    [JsonPropertyName("city")]
    public string? City { get; set; }
}
```

---

## 🛠️ **Testing the API (Unit Test Example)**

To ensure the function calling works as expected, we can write a simple **unit test** in **C#** using `xUnit`:

```csharp
using Xunit;

public class FunctionCallingTests
{
    [Fact]
    public void Test_GetCurrentWeather_Returns_CorrectResponse()
    {
        string city = "Toronto";
        string result = Program.GetCurrentWeather(city);
        Assert.Contains("Toronto", result);
    }
}
```

Run the test with:
```bash
dotnet test
```

---

## 🎯 Conclusion

This repository demonstrates how to use **function calling with Llama 3.1 in Ollama** using **C#**.  
It allows Llama 3.1 to **trigger external functions dynamically** based on the user's query.

---

## 🔗 Useful Links

- 📌 [Ollama Documentation](https://ollama.com/)
- 📌 [Llama 3.1 Model](https://ollama.com/models/llama3.1)
- 📌 [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- 📌 [OpenAI NuGet](https://www.nuget.org/packages/OpenAI)

