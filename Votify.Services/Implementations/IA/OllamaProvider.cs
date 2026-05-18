using System.Net.Http.Json;
using System.Text.Json;
using Votify.Core.Interfaces;

namespace Votify.Services.Implementations.IA
{
    public class OllamaProvider : IIAProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelo;

        public OllamaProvider(HttpClient httpClient, string modelo = "llama3")
        {
            _httpClient = httpClient;
            _modelo = modelo;
        }

        public async Task<string> AnalizarAsync(string prompt)
        {
            var body = new
            {
                model = _modelo,
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/generate", body);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("response").GetString() ?? string.Empty;
        }
    }
}
