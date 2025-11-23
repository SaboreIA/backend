using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetByIdAsync(long id);
        Task<PaginatedResponseDTO<ReviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<PaginatedResponseDTO<ReviewDto>> GetByRestaurantIdAsync(long restaurantId, int pageNumber = 1, int pageSize = 10);
        Task<PaginatedResponseDTO<ReviewDto>> GetByUserIdAsync(long userId, int pageNumber = 1, int pageSize = 10);
        Task<ReviewDto> CreateAsync(CreateReviewDto createReviewDto, long userId);
        Task<ReviewDto?> UpdateAsync(long id, UpdateReviewDto updateReviewDto, long userId, string userRole);
        Task<ReviewDto?> UpdateReviewImageAsync(long id, IFormFile imageFile, long userId, string userRole);
        Task<bool> DeleteAsync(long id, long userId, string userRole);
        Task<double> GetAverageRatingByRestaurantIdAsync(long restaurantId);
        Task<int> GetReviewCountByRestaurantIdAsync(long restaurantId);
    }
}