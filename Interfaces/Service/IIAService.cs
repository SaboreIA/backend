namespace SaboreIA.Interfaces.Service
{
    /// <summary>
    /// Interface para serviços de inteligência artificial
    /// Responsável por processar entradas do usuário e gerar recomendações de restaurantes
    /// </summary>
    public interface IIAService
    {
        /// <summary>
        /// Processa um termo fornecido pelo usuário e retorna o ID da tag correspondente.
        /// Se o termo não for reconhecido, consulta a IA para identificar o produto e sua categoria.
        /// </summary>
        /// <param name="term">Termo de entrada do usuário (ex: "pizza", "sushi", "quero comer hamburguer")</param>
        /// <returns>ID da tag/categoria encontrada ou criada, ou null se o termo não for reconhecido como comida</returns>
        Task<long?> GetOrAddTagAsync(string term);
    }
}
