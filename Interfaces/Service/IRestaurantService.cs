using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface IRestaurantService
    {
        Task<RestaurantDetailDto?> GetByIdAsync(long id);
        Task<PaginatedResponseDTO<RestaurantListDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<RestaurantListDTO>> GetByOwnerIdAsync(long ownerId);
        Task<IEnumerable<RestaurantListDTO>> GetByTagIdAsync(long tagId);
        Task<RestaurantDetailDto> CreateAsync(CreateRestaurantDto createRestaurantDto, long ownerId);
        Task<RestaurantDetailDto?> UpdateAsync(long id, UpdateRestaurantDto updateRestaurantDto);
        Task<RestaurantDetailDto?> UpdateAllImagesAsync(long id, IFormFile? coverImage, IFormFile? image1, IFormFile? image2, IFormFile? image3);
        Task<bool> DeleteAsync(long id);
    }
}
