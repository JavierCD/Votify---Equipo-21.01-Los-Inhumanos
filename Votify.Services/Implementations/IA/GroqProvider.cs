using System.Net.Http.Json;
using System.Text.Json;
using Votify.Core.Interfaces;

namespace Votify.Services.Implementations.IA
{
    public class GroqProvider : IIAProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelo;

        public GroqProvider(HttpClient httpClient, string modelo = "llama-3.1-8b-instant")
        {
            _httpClient = httpClient;
            _modelo = modelo;
        }

        public async Task<string> AnalizarAsync(string prompt)
        {
            var body = new
            {
                model = _modelo,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 4096
            };

            var response = await _httpClient.PostAsJsonAsync("/openai/v1/chat/completions", body);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? string.Empty;
        }
    }
}
