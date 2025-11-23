namespace SaboreIA.Services.IA
{
    /// <summary>
    /// Configurações para integração com a API Perplexity AI
    /// Mapeadas da seção "IAServiceConfig" do appsettings.json
    /// </summary>
    public class IAServiceConfig
    {
        /// <summary>
        /// Chave de API para autenticação na Perplexity AI
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// URL base da API Perplexity
        /// </summary>
        public string ApiUrl { get; set; }
    }
}
