using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;
using SaboreIA.Helpers;

namespace SaboreIA.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IRestaurantRepository restaurantRepository,
            IUserRepository userRepository)
        {
            _favoriteRepository = favoriteRepository;
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
        }

        public async Task<FavoriteDto?> GetByIdAsync(long id)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(id);
            return favorite != null ? MapToDto(favorite) : null;
        }

        public async Task<IEnumerable<FavoriteDto>> GetAllAsync()
        {
            var favorites = await _favoriteRepository.GetAllAsync();
            return favorites.Select(MapToDto);
        }

        public async Task<IEnumerable<FavoriteDto>> GetByUserIdAsync(long userId)
        {
            var favorites = await _favoriteRepository.GetByUserIdAsync(userId);
            return favorites.Select(MapToDto);
        }

        public async Task<IEnumerable<FavoriteDto>> GetByRestaurantIdAsync(long restaurantId)
        {
            var favorites = await _favoriteRepository.GetByRestaurantIdAsync(restaurantId);
            return favorites.Select(MapToDto);
        }

        public async Task<FavoriteStatusDto> GetFavoriteStatusAsync(long userId, long restaurantId)
        {
            var favorite = await _favoriteRepository.GetByUserAndRestaurantAsync(userId, restaurantId);
            return new FavoriteStatusDto
            {
                IsFavorite = favorite != null,
                FavoriteId = favorite?.Id
            };
        }

        public async Task<FavoriteDto> AddFavoriteAsync(CreateFavoriteDto createFavoriteDto, long userId)
        {
            // Verificar se o usuário existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Verificar se o restaurante existe
            var restaurant = await _restaurantRepository.GetByIdAsync(createFavoriteDto.RestaurantId);
            if (restaurant == null)
                throw new InvalidOperationException("Restaurant not found");

            // Verificar se já não está favoritado
            var existingFavorite = await _favoriteRepository.GetByUserAndRestaurantAsync(userId, createFavoriteDto.RestaurantId);
            if (existingFavorite != null)
                throw new InvalidOperationException("Restaurant is already favorited by this user");

            var favorite = new Favorite
            {
                UserId = userId,
                RestaurantId = createFavoriteDto.RestaurantId
            };

            var createdFavorite = await _favoriteRepository.CreateAsync(favorite);
            return MapToDto(createdFavorite);
        }

        public async Task<bool> RemoveFavoriteAsync(long userId, long restaurantId)
        {
            // Verificar se o favorito existe
            var favorite = await _favoriteRepository.GetByUserAndRestaurantAsync(userId, restaurantId);
            if (favorite == null)
                return false;

            return await _favoriteRepository.DeleteByUserAndRestaurantAsync(userId, restaurantId);
        }

        public async Task<bool> RemoveFavoriteByIdAsync(long id, long userId)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(id);
            if (favorite == null)
                return false;

            // Verificar se o usuário é o dono do favorito
            if (favorite.UserId != userId)
                throw new UnauthorizedAccessException("User can only remove their own favorites");

            return await _favoriteRepository.DeleteAsync(id);
        }

        public async Task<int> GetFavoriteCountByRestaurantIdAsync(long restaurantId)
        {
            return await _favoriteRepository.GetFavoriteCountByRestaurantIdAsync(restaurantId);
        }

        private static FavoriteDto MapToDto(Favorite favorite)
        {
            return new FavoriteDto
            {
                Id = favorite.Id,
                UserId = favorite.UserId,
                RestaurantId = favorite.RestaurantId,
                CreatedAt = DateTimeHelper.ToSaoPauloTime(favorite.CreatedAt),
                RestaurantName = favorite.Restaurant?.Name ?? "Unknown",
                RestaurantCoverImageUrl = favorite.Restaurant?.CoverImageUrl,
                RestaurantDescription = favorite.Restaurant?.Description
            };
        }
    }
}
