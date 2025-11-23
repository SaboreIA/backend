using SaboreIA.Services.IA;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SaboreIA.Integrations
{
    /// <summary>
    /// Cliente HTTP para integração com a API Perplexity AI
    /// Gerencia autenticação e comunicação com o serviço de IA
    /// </summary>
    public class PerplexityClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        /// <summary>
        /// Inicializa uma nova instância do cliente Perplexity
        /// </summary>
        /// <param name="httpClient">HttpClient configurado pela factory</param>
        /// <param name="config">Configurações da API Perplexity</param>
        public PerplexityClient(HttpClient httpClient, IOptions<IAServiceConfig> config)
        {
            _httpClient = httpClient;
            _apiKey = config.Value.ApiKey;
            _apiUrl = config.Value.ApiUrl;

            // Configura o cabeçalho de autorização uma vez
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        /// <summary>
        /// Envia uma requisição para a Perplexity AI com contexto de sistema e prompt do usuário
        /// </summary>
        /// <param name="systemPrompt">Instrução de sistema que define o comportamento da IA</param>
        /// <param name="userPrompt">Entrada/pergunta do usuário</param>
        /// <returns>Resposta da IA ou string vazia em caso de falha</returns>
        /// <exception cref="HttpRequestException">Erro na comunicação com a API</exception>
        /// <exception cref="InvalidOperationException">Erro ao processar resposta da IA</exception>
        public async Task<string> SendAsync(string systemPrompt, string userPrompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "sonar",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = systemPrompt
                        },
                        new
                        {
                            role = "user",
                            content = userPrompt
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                // Tratamento de erro HTTP mais detalhado
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Perplexity API error: {response.StatusCode} - {errorContent}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();

                // Parse seguro da resposta JSON
                var doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message");
                    if (message.TryGetProperty("content", out var contentElement))
                    {
                        return contentElement.GetString() ?? string.Empty;
                    }
                }

                return string.Empty;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    "Falha na comunicação com Perplexity AI", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "Resposta inválida da Perplexity AI", ex);
            }
        }
    }
}
