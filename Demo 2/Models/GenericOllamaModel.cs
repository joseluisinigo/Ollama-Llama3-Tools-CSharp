using System;
using System.Threading.Tasks;
using Serilog;

namespace Models
{
    public class GenericOllamaModel : OllamaModelBase
    {
        public override async Task HandleRequestAsync(string model)
        {
            var messages = new[]
            {
                new { role = "user", content = "Dime un dato curioso sobre Madrid." }
            };

            var requestBody = new { model, messages };
            string responseString = await SendRequestAsync(model, requestBody);

            if (string.IsNullOrEmpty(responseString))
            {
                Log.Warning("⚠️ No se recibió respuesta válida de Ollama.");
                return;
            }

            Log.Information("📩 Respuesta de {model}: {response}", model, responseString);
            Console.WriteLine($"🔹 Respuesta de {model}: {responseString}");
        }
    }
}
