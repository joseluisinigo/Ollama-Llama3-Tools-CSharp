Sí, **Ollama** soporta herramientas en **C#**, y puedes integrarlo con tus propios métodos para que la IA los llame según sea necesario. Además, **LM Studio** también es compatible con modelos de Ollama, pero actualmente no tiene una interfaz nativa para definir herramientas como Ollama directamente. Sin embargo, si ejecutas un modelo en LM Studio, podrías hacer llamadas a su API desde C# de manera similar.

---

## 🔹 **1. Modelo (LM) recomendado**
Para herramientas en **Ollama**, necesitas un modelo con capacidades de *function calling*. Algunos recomendados son:
- **Mistral (mistral:latest)** → Pequeño y rápido, muy eficiente para tareas generales.
- **Gemma (gemma:latest)** → Más potente en algunas tareas de razonamiento.
- **Llama 3 (cuando esté disponible en Ollama)** → Promete mejoras en el uso de herramientas.

Para instalar el modelo en **Ollama**, usa:
```bash
ollama pull mistral
```
O si prefieres otro:
```bash
ollama pull gemma
```

---

## 🔹 **2. Código en C#**
Aquí tienes un **ejemplo en C#** usando **Ollama** con herramientas y un sistema de *logging* para registrar cómo interactúa la IA con el código:

### 📌 **Ejemplo: Consultar la hora actual usando una herramienta en C#**
Este código define un método (`GetCurrentTime`) que Ollama puede llamar cuando sea necesario.

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    // Definir el cliente HTTP para comunicarnos con Ollama
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Inicializando Ollama con herramientas...");

        // Definir las herramientas disponibles
        var tools = new[]
        {
            new
            {
                name = "get_current_time",
                description = "Devuelve la hora actual en formato HH:mm:ss"
            }
        };

        // Mensaje del usuario
        var messages = new[]
        {
            new { role = "user", content = "¿Qué hora es ahora?" }
        };

        // Crear la solicitud JSON
        var requestBody = new
        {
            model = "mistral",  // Puedes cambiarlo por otro modelo que hayas instalado
            messages,
            tools
        };

        // Serializar el JSON
        string jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Enviar la solicitud a Ollama
        var response = await client.PostAsync("http://localhost:11434/api/generate", content);
        string responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine("🔹 Respuesta de Ollama:");
        Console.WriteLine(responseString);

        // Analizar la respuesta para ver si llamó a la función
        if (responseString.Contains("get_current_time"))
        {
            // Llamar a la función localmente
            string result = GetCurrentTime();
            Console.WriteLine($"✅ La IA ha solicitado la hora: {result}");
        }
    }

    // Método que Ollama puede llamar
    static string GetCurrentTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}
```

---

## 🔹 **3. Explicación del Código**
✅ Se inicializa una **herramienta** llamada `"get_current_time"`.  
✅ Se envía un **mensaje del usuario** a Ollama preguntando la hora.  
✅ Ollama analiza si necesita llamar a la herramienta y lo registra en el JSON.  
✅ Si la IA detecta que debe llamar a `"get_current_time"`, se ejecuta el método en C#.  
✅ Se **imprime la interacción** en la consola para tener un *log* detallado.

---

## 🔹 **4. Cómo Ver los Logs de Interacción**
Para asegurarte de que Ollama está llamando correctamente a las herramientas:
1. **Ejecuta Ollama en modo servidor**:  
   ```bash
   ollama serve
   ```
2. **Añade logs en el código** con `Console.WriteLine` para registrar respuestas.
3. **Opcional**: Usar **Serilog** para almacenar logs en un archivo:
   ```csharp
   Log.Information("Respuesta de Ollama: {responseString}");
   ```

---

## 🔹 **5. ¿Se puede hacer con LM Studio?**
Actualmente, **LM Studio** solo permite cargar modelos y usarlos en un endpoint **localhost:1234**.  
Pero sí puedes hacer llamadas HTTP a LM Studio de la misma manera, aunque no tiene un sistema nativo de herramientas (*function calling*). 

Si usas **LM Studio**, cambiarías la URL en el código:
```csharp
var response = await client.PostAsync("http://localhost:1234/v1/chat/completions", content);
```

---

## 🎯 **Conclusión**
- **Ollama sí permite herramientas nativas en C#**. 🚀
- **LM Studio NO tiene tools**, pero puedes usarlo para consultas normales con modelos de lenguaje.
- Puedes ver las interacciones de Ollama en la terminal o guardar logs en un archivo.

Si necesitas ayuda con otro ejemplo más complejo, dime qué quieres hacer. 😃