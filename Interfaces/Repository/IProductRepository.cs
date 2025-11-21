using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(long id);
        Task<Product?> GetByNameAsync(string productName);
        Task<bool> NameExistsAsync(string name);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(long id);
    }
}
