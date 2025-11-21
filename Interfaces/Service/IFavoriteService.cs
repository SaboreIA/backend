using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface IFavoriteService
    {
        Task<FavoriteDto?> GetByIdAsync(long id);
        Task<IEnumerable<FavoriteDto>> GetAllAsync();
        Task<IEnumerable<FavoriteDto>> GetByUserIdAsync(long userId);
        Task<IEnumerable<FavoriteDto>> GetByRestaurantIdAsync(long restaurantId);
        Task<FavoriteStatusDto> GetFavoriteStatusAsync(long userId, long restaurantId);
        Task<FavoriteDto> AddFavoriteAsync(CreateFavoriteDto createFavoriteDto, long userId);
        Task<bool> RemoveFavoriteAsync(long userId, long restaurantId);
        Task<bool> RemoveFavoriteByIdAsync(long id, long userId);
        Task<int> GetFavoriteCountByRestaurantIdAsync(long restaurantId);
    }
}
