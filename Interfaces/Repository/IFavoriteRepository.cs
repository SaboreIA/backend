using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetByIdAsync(long id);
        Task<IEnumerable<Favorite>> GetAllAsync();
        Task<IEnumerable<Favorite>> GetByUserIdAsync(long userId);
        Task<IEnumerable<Favorite>> GetByRestaurantIdAsync(long restaurantId);
        Task<Favorite?> GetByUserAndRestaurantAsync(long userId, long restaurantId);
        Task<Favorite> CreateAsync(Favorite favorite);
        Task<bool> DeleteAsync(long id);
        Task<bool> DeleteByUserAndRestaurantAsync(long userId, long restaurantId);
        Task<bool> ExistsAsync(long userId, long restaurantId);
        Task<int> GetFavoriteCountByRestaurantIdAsync(long restaurantId);
    }
}
