using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;

namespace SaboreIA.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;

        public ProductService(IProductRepository productRepository, ITagRepository tagRepository)
        {
            _productRepository = productRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => MapToDto(p));
        }

        public async Task<ProductDto?> GetByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto?> GetByNameAsync(string productName)
        {
            var product = await _productRepository.GetByNameAsync(productName);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            // Validar se TagId foi fornecido
            if (!dto.TagId.HasValue)
                throw new InvalidOperationException("TagId é obrigatório");

            // Validar se a Tag existe
            var tagExists = await _tagRepository.ExistsAsync(dto.TagId.Value);
            if (!tagExists)
                throw new InvalidOperationException("Tag não encontrada");

            // Validar se nome já existe
            var nameExists = await _productRepository.NameExistsAsync(dto.Name);
            if (nameExists)
                throw new InvalidOperationException($"Produto com nome '{dto.Name}' já existe");

            var product = new Product
            {
                Name = dto.Name,
                TagId = dto.TagId.Value
            };

            var created = await _productRepository.CreateAsync(product);
            return MapToDto(created);
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return false;

            // Validar se a Tag existe
            var tagExists = await _tagRepository.ExistsAsync(dto.TagId);
            if (!tagExists)
                throw new InvalidOperationException("Tag não encontrada");

            // Validar se o novo nome já existe em outro produto
            if (product.Name != dto.Name)
            {
                var nameExists = await _productRepository.NameExistsAsync(dto.Name);
                if (nameExists)
                    throw new InvalidOperationException($"Produto com nome '{dto.Name}' já existe");
            }

            product.Name = dto.Name;
            product.TagId = dto.TagId;

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                TagId = product.TagId,
                TagName = product.Tag?.Name ?? "Unknown"
            };
        }
    }
}