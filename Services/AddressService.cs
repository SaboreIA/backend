using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;

namespace SaboreIA.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<AddressDto?> GetByIdAsync(long id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            return address != null ? MapToDto(address) : null;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync()
        {
            var addresses = await _addressRepository.GetAllAsync();
            return addresses.Select(MapToDto);
        }

        public async Task<AddressDto> CreateAsync(CreateAddressDto createAddressDto)
        {
            var address = new Address
            {
                ZipCode = createAddressDto.ZipCode,
                Street = createAddressDto.Street,
                Number = createAddressDto.Number,
                Complement = createAddressDto.Complement,
                City = createAddressDto.City,
                State = createAddressDto.State,
                Country = createAddressDto.Country
            };

            var createdAddress = await _addressRepository.CreateAsync(address);
            return MapToDto(createdAddress);
        }

        public async Task<AddressDto?> UpdateAsync(long id, UpdateAddressDto updateAddressDto)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
                return null;

            // Atualizar apenas campos não nulos
            if (!string.IsNullOrWhiteSpace(updateAddressDto.ZipCode))
                address.ZipCode = updateAddressDto.ZipCode;

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Street))
                address.Street = updateAddressDto.Street;

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Number))
                address.Number = updateAddressDto.Number;

            if (updateAddressDto.Complement != null)
                address.Complement = updateAddressDto.Complement;

            if (!string.IsNullOrWhiteSpace(updateAddressDto.City))
                address.City = updateAddressDto.City;

            if (!string.IsNullOrWhiteSpace(updateAddressDto.State))
                address.State = updateAddressDto.State;

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Country))
                address.Country = updateAddressDto.Country;

            var updatedAddress = await _addressRepository.UpdateAsync(address);
            return MapToDto(updatedAddress);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            // Verificar se o endereço está sendo usado por algum restaurante
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
                return false;

            if (address.Restaurants != null && address.Restaurants.Any())
            {
                throw new InvalidOperationException("Cannot delete address that is being used by restaurants.");
            }

            return await _addressRepository.DeleteAsync(id);
        }

        // Método auxiliar para mapear Address -> AddressDto
        private AddressDto MapToDto(Address address)
        {
            return new AddressDto
            {
                Id = address.Id,
                ZipCode = address.ZipCode,
                Street = address.Street,
                Number = address.Number,
                Complement = address.Complement,
                City = address.City,
                State = address.State,
                Country = address.Country
            };
        }
    }
}
