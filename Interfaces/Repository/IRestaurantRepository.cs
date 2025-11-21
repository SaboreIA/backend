using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface IRestaurantRepository
    {
        Task<Restaurant?> GetByIdAsync(long id);
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(long ownerId);
        Task<IEnumerable<Restaurant>> GetByTagIdAsync(long tagId);
        Task<Restaurant> CreateAsync(Restaurant restaurant);
        Task<Restaurant> UpdateAsync(Restaurant restaurant);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}
