using Microsoft.EntityFrameworkCore;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;

namespace SaboreIA.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Favorite?> GetByIdAsync(long id)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Restaurant)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Favorite>> GetAllAsync()
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Restaurant)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> GetByUserIdAsync(long userId)
        {
            return await _context.Favorites
                .Include(f => f.Restaurant)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> GetByRestaurantIdAsync(long restaurantId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Where(f => f.RestaurantId == restaurantId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Favorite?> GetByUserAndRestaurantAsync(long userId, long restaurantId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Restaurant)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RestaurantId == restaurantId);
        }

        public async Task<Favorite> CreateAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(favorite.Id) ?? favorite;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByUserAndRestaurantAsync(long userId, long restaurantId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RestaurantId == restaurantId);
            
            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long userId, long restaurantId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.RestaurantId == restaurantId);
        }

        public async Task<int> GetFavoriteCountByRestaurantIdAsync(long restaurantId)
        {
            return await _context.Favorites
                .CountAsync(f => f.RestaurantId == restaurantId);
        }
    }
}
