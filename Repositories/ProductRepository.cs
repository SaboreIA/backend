using SaboreIA.Models;
using Microsoft.EntityFrameworkCore;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Database;

namespace SaboreIA.Repositories
{ 
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Tag)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByNameAsync(string productName)
        {
            return await _context.Products
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower());
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(product.Id) ?? product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity == null)
                return false;

            _context.Products.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}