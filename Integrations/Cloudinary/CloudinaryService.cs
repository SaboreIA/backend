using Microsoft.Extensions.Options;

namespace SaboreIA.Integrations.Cloudinary
{
    /// <summary>
    /// Serviço para gerenciamento de imagens via Cloudinary
    /// Responsável por upload, transformação e exclusão de imagens
    /// </summary>
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        /// <summary>
        /// Inicializa uma nova instância do serviço Cloudinary
        /// </summary>
        /// <param name="config">Configurações do Cloudinary (CloudName, ApiKey, ApiSecret)</param>
        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new CloudinaryDotNet.Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        /// <summary>
        /// Faz upload de uma imagem para o Cloudinary com transformações automáticas
        /// </summary>
        /// <param name="file">Arquivo de imagem a ser enviado</param>
        /// <param name="folder">Pasta de destino no Cloudinary (use CloudinaryFolders)</param>
        /// <returns>URL segura (HTTPS) da imagem hospedada</returns>
        /// <exception cref="ArgumentException">Arquivo inválido ou vazio</exception>
        /// <exception cref="Exception">Erro no upload para Cloudinary</exception>
        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            // Validação de entrada
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido ou vazio", nameof(file));

            // Validação de tipo de arquivo (defesa em profundidade)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException(
                    "Formato de imagem não suportado. Use JPG, JPEG, PNG ou GIF", 
                    nameof(file));

            // Validação de tamanho (5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException(
                    "A imagem não pode exceder 5MB", 
                    nameof(file));

            using var stream = file.OpenReadStream();
            
            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                Folder = folder,
                // Transformação: limita dimensões a 500x500 mantendo proporção
                Transformation = new CloudinaryDotNet.Transformation()
                    .Width(500).Height(500).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception(
                    $"Erro ao fazer upload para Cloudinary: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl.ToString();
        }

        /// <summary>
        /// Deleta uma imagem do Cloudinary pelo seu public ID
        /// </summary>
        /// <param name="publicId">ID público da imagem no Cloudinary</param>
        /// <returns>True se deletado com sucesso, False se não encontrado ou erro</returns>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
    }
}
