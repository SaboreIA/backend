using Microsoft.EntityFrameworkCore;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;

namespace SaboreIA.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByIdAsync(long id)
        {
            return await _context.Tag
                .Include(c => c.Restaurants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Tag
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tag
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tag
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag category)
        {
            _context.Tag.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Tag> UpdateAsync(Tag category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Tag.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var category = await _context.Tag.FindAsync(id);
            if (category == null)
                return false;

            _context.Tag.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Tag.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Tag
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}
