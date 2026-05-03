using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoClientService : IVotoClientService
    {
        private readonly HttpClient _httpClient;

        public VotoClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> EmitirVotoMulticriterioAsync(EmitirVotoMulticriterioRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/votos/multicriterio", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EmitirVotoPuntuacionAsync(EmitirVotoPuntuacionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/votos/puntuacion", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EmitirVotoPopularAsync(EmitirVotoPopularRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/votos/popular", request);
            return response.IsSuccessStatusCode;
        }
    }
}
