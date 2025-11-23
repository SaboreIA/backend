using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(long id);
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}
