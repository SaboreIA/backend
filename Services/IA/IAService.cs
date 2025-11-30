using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Integrations;
using SaboreIA.Models;

namespace SaboreIA.Services.IA
{
    /// <summary>
    /// Serviço de inteligência artificial para processamento de preferências alimentares
    /// Integra com Perplexity AI para identificar produtos e categorias
    /// </summary>
    public class IAService : IIAService
    {
        private readonly PerplexityClient _perplexityClient;
        private readonly ITagRepository _tagRepository;
        private readonly IProductRepository _productRepository;

        public IAService(
            PerplexityClient perplexityClient, 
            ITagRepository tagRepository, 
            IProductRepository productRepository)
        {
            _perplexityClient = perplexityClient;
            _tagRepository = tagRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Processa um termo e retorna o ID da tag correspondente.
        /// Fluxo: 
        /// 1. Verifica se o termo já é uma tag existente
        /// 2. Consulta IA para identificar o produto
        /// 3. Verifica se o produto já existe no banco
        /// 4. Consulta IA para obter a categoria do produto
        /// 5. Cria produto e/ou tag se necessário
        /// </summary>
        public async Task<long?> GetOrAddTagAsync(string term)
        {
            // 1. Verifica se o termo já é uma tag existente
            Tag? tag = await _tagRepository.GetByNameAsync(term);
            if (tag != null)
                return tag.Id;

            // 2. Consulta IA para identificar o produto
            string product = await _perplexityClient.SendAsync(
                IAHelpers.getProductByUserInput(), 
                term
            );

            if (product == "nulo")
                return null;

            if (product == "sugestao")
            {
                // Busca todas as tags disponíveis
                var allTags = (await _tagRepository.GetAllAsync()).ToList();
                
                if (allTags.Count == 0)
                    return null;

                // Retorna uma tag aleatória
                var random = new Random();
                var randomTag = allTags[random.Next(allTags.Count)];
                return randomTag.Id;
            }

            // 3. Verifica se o produto já existe no banco
            Product? findedProduct = await _productRepository.GetByNameAsync(product);
            if (findedProduct != null)
                return findedProduct.Tag.Id;

            try
            {
                // 4. Consulta IA para obter a categoria do produto
                string IAnswer = await _perplexityClient.SendAsync(
                    IAHelpers.getTagByProduct(), 
                    product
                );

                // Limpa a resposta para obter apenas o nome da tag
                IAnswer = IAnswer.Trim()
                    .Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (string.IsNullOrWhiteSpace(IAnswer))
                    throw new InvalidOperationException("IA retornou resposta vazia");

                // 5. Verifica novamente se a tag existe (pode ter sido criada entre as consultas)
                Tag? findedTag = await _tagRepository.GetByNameAsync(IAnswer);

                Product newProduct = new Product
                {
                    Name = product,
                    // Se a tag já existe, usa ela. Caso contrário, cria uma nova.
                    Tag = findedTag ?? new Tag { Name = IAnswer }
                };

                // Cria o novo produto (e possivelmente a nova tag)
                Product createdProduct = await _productRepository.CreateAsync(newProduct);

                // Retorna o ID da tag associada ao produto criado
                return createdProduct.TagId;
            }
            catch (Exception ex)
            {
                // Propaga erro com contexto
                throw new InvalidOperationException(
                    $"Erro ao processar termo '{term}' com IA", ex);
            }
        }
    }
}
