using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(long id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<IEnumerable<Review>> GetByRestaurantIdAsync(long restaurantId);
        Task<IEnumerable<Review>> GetByUserIdAsync(long userId);
        Task<Review?> GetByUserAndRestaurantAsync(long userId, long restaurantId);
        Task<Review> CreateAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task<bool> DeleteAsync(long id);
        Task<double> GetAverageRatingByRestaurantIdAsync(long restaurantId);
        Task<int> GetReviewCountByRestaurantIdAsync(long restaurantId);
    }
}
