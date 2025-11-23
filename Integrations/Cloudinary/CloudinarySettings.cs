namespace SaboreIA.Integrations.Cloudinary
{
    /// <summary>
    /// Configurações para integração com Cloudinary
    /// Mapeadas da seção "Cloudinary" do appsettings.json
    /// </summary>
    public class CloudinarySettings
    {
        /// <summary>
        /// Nome da conta/cloud no Cloudinary
        /// </summary>
        public string CloudName { get; set; } = string.Empty;

        /// <summary>
        /// Chave de API para autenticação
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Secret da API para autenticação segura
        /// </summary>
        public string ApiSecret { get; set; } = string.Empty;
    }
}
