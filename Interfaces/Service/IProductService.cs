using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(long id);
    Task<ProductDto?> GetByNameAsync(string product);
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<bool> UpdateAsync(ProductUpdateDto dto);
    Task<bool> DeleteAsync(long id);
}
}