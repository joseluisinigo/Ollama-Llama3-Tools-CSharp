using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace Models
{
    public abstract class OllamaModelBase
    {
        protected static readonly HttpClient client = new HttpClient();

        public abstract Task HandleRequestAsync(string model);

        protected async Task<string> SendRequestAsync(string model, object requestBody)
        {
            string jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:11434/api/chat", content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Log.Error("❌ Error en la solicitud a Ollama: {error}", ex.Message);
                return string.Empty;
            }
        }
    }
}
