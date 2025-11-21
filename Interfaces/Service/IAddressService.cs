using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface IAddressService
    {
        Task<AddressDto?> GetByIdAsync(long id);
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto> CreateAsync(CreateAddressDto createAddressDto);
        Task<AddressDto?> UpdateAsync(long id, UpdateAddressDto updateAddressDto);
        Task<bool> DeleteAsync(long id);
    }
}
