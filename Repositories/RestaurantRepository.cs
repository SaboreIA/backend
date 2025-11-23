using Microsoft.EntityFrameworkCore;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;

namespace SaboreIA.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurant?> GetByIdAsync(long id)
        {
            return await _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Owner)
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Categories)
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(long ownerId)
        {
            return await _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Categories)
                .Where(r => r.OwnerId == ownerId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetByTagIdAsync(long tagId)
        {
            return await _context.Restaurants
                .Include(r => r.Categories)
                .Include(r => r.Address)
                .Where(r => r.Categories.Any(c => c.Id == tagId))
                .ToListAsync();
        }


        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<Restaurant> UpdateAsync(Restaurant restaurant)
        {
            restaurant.UpdatedAt = DateTime.UtcNow;
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return false;

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Restaurants.AnyAsync(r => r.Id == id);
        }
    }
}
