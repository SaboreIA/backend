using Microsoft.EntityFrameworkCore;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;

namespace SaboreIA.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(long id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByRestaurantIdAsync(long restaurantId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(long userId)
        {
            return await _context.Reviews
                .Include(r => r.Restaurant)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndRestaurantAsync(long userId, long restaurantId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RestaurantId == restaurantId);
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(review.Id) ?? review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            review.UpdatedAt = DateTime.UtcNow;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageRatingByRestaurantIdAsync(long restaurantId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.RestaurantId == restaurantId)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.AvgRating) : 0;
        }

        public async Task<int> GetReviewCountByRestaurantIdAsync(long restaurantId)
        {
            return await _context.Reviews
                .CountAsync(r => r.RestaurantId == restaurantId);
        }
    }
}
