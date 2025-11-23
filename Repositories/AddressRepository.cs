using Microsoft.EntityFrameworkCore;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;

namespace SaboreIA.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetByIdAsync(long id)
        {
            return await _context.Addresses
                .Include(a => a.Restaurants)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Addresses
                .OrderBy(a => a.City)
                .ThenBy(a => a.Street)
                .ToListAsync();
        }

        public async Task<Address> CreateAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            address.UpdatedAt = DateTime.Now;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Addresses.AnyAsync(a => a.Id == id);
        }
    }
}
