using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;
using SaboreIA.Interfaces.Service;
using SaboreIA.Integrations.Cloudinary;
using SaboreIA.Helpers;

namespace SaboreIA.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            ITagRepository tagRepository,
            IReviewRepository reviewRepository,
            ICloudinaryService cloudinaryService)
        {
            _restaurantRepository = restaurantRepository;
            _tagRepository = tagRepository;
            _reviewRepository = reviewRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<RestaurantDetailDto?> GetByIdAsync(long id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null) return null;

            var dto = MapToDetailDto(restaurant);
            dto.AvgRating = Math.Round(await _reviewRepository.GetAverageRatingByRestaurantIdAsync(id), 2);
            dto.ReviewCount = await _reviewRepository.GetReviewCountByRestaurantIdAsync(id);

            return dto;
        }

        public async Task<PaginatedResponseDTO<RestaurantListDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var restaurants = await _restaurantRepository.GetAllAsync();

            // Paginação antes de processar
            var paginatedRestaurants = restaurants
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Processar cada restaurante sequencialmente para evitar concorrência
            var restaurantListDtos = new List<RestaurantListDTO>();
            foreach (var restaurant in paginatedRestaurants)
            {
                var dto = MapToListDto(restaurant);
                dto.AvgRating = Math.Round(await _reviewRepository.GetAverageRatingByRestaurantIdAsync(restaurant.Id), 2);
                dto.ReviewCount = await _reviewRepository.GetReviewCountByRestaurantIdAsync(restaurant.Id);
                restaurantListDtos.Add(dto);
            }

            return new PaginatedResponseDTO<RestaurantListDTO>
            {
                Items = restaurantListDtos,
                TotalItems = restaurants.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<RestaurantListDTO>> GetByOwnerIdAsync(long ownerId)
        {
            var restaurants = await _restaurantRepository.GetByOwnerIdAsync(ownerId);

            // Processar sequencialmente
            var restaurantDtos = new List<RestaurantListDTO>();
            foreach (var restaurant in restaurants)
            {
                var dto = MapToListDto(restaurant);
                dto.AvgRating = Math.Round(await _reviewRepository.GetAverageRatingByRestaurantIdAsync(restaurant.Id), 2);
                dto.ReviewCount = await _reviewRepository.GetReviewCountByRestaurantIdAsync(restaurant.Id);
                restaurantDtos.Add(dto);
            }

            return restaurantDtos;
        }

        public async Task<IEnumerable<RestaurantListDTO>> GetByTagIdAsync(long tagId)
        {
            var restaurantes = await _restaurantRepository.GetByTagIdAsync(tagId);

            // Processar sequencialmente
            var restaurantDtos = new List<RestaurantListDTO>();
            foreach (var restaurant in restaurantes)
            {
                var dto = MapToListDto(restaurant);
                dto.AvgRating = Math.Round(await _reviewRepository.GetAverageRatingByRestaurantIdAsync(restaurant.Id), 2);
                dto.ReviewCount = await _reviewRepository.GetReviewCountByRestaurantIdAsync(restaurant.Id);
                restaurantDtos.Add(dto);
            }

            return restaurantDtos;
        }

        public async Task<RestaurantDetailDto> CreateAsync(CreateRestaurantDto createRestaurantDto, long ownerId)
        {
            var restaurant = new Restaurant
            {
                Name = createRestaurantDto.Name,
                PhoneNumber = createRestaurantDto.PhoneNumber,
                Email = createRestaurantDto.Email,
                Description = createRestaurantDto.Description,
                Site = createRestaurantDto.Site,
                Menu = createRestaurantDto.Menu,
                CoverImageUrl = createRestaurantDto.CoverImageUrl,
                ImageUrl1 = createRestaurantDto.ImageUrl1,
                ImageUrl2 = createRestaurantDto.ImageUrl2,
                ImageUrl3 = createRestaurantDto.ImageUrl3,
                OwnerId = ownerId,
                OpenDay = createRestaurantDto.OpenDay,
                CloseDay = createRestaurantDto.CloseDay,
                OpenTime = TimeSpan.Parse(createRestaurantDto.OpenTime),
                CloseTime = TimeSpan.Parse(createRestaurantDto.CloseTime),
                Address = new Address
                {
                    ZipCode = createRestaurantDto.Address.ZipCode,
                    Street = createRestaurantDto.Address.Street,
                    Number = createRestaurantDto.Address.Number,
                    Complement = createRestaurantDto.Address.Complement,
                    City = createRestaurantDto.Address.City,
                    State = createRestaurantDto.Address.State,
                    Country = createRestaurantDto.Address.Country
                }
            };

            var createdRestaurant = await _restaurantRepository.CreateAsync(restaurant);

            // Adicionar tags DEPOIS de criar o restaurante
            if (createRestaurantDto.TagIds != null && createRestaurantDto.TagIds.Any())
            {
                // Carregar as tags existentes do banco
                var existingTags = await _tagRepository.GetByIdsAsync(createRestaurantDto.TagIds);
                createdRestaurant.Categories = existingTags.ToList();
                await _restaurantRepository.UpdateAsync(createdRestaurant);
            }

            // Recarregar com todas as relações
            return await GetByIdAsync(createdRestaurant.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created restaurant.");
        }


        public async Task<RestaurantDetailDto?> UpdateAsync(long id, UpdateRestaurantDto updateRestaurantDto)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
                return null;

            // Atualizar campos do restaurante
            if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Name))
                restaurant.Name = updateRestaurantDto.Name;

            if (updateRestaurantDto.PhoneNumber != null)
                restaurant.PhoneNumber = updateRestaurantDto.PhoneNumber;

            if (updateRestaurantDto.Email != null)
                restaurant.Email = updateRestaurantDto.Email;

            if (updateRestaurantDto.Description != null)
                restaurant.Description = updateRestaurantDto.Description;

            if (updateRestaurantDto.Site != null)
                restaurant.Site = updateRestaurantDto.Site;

            if (updateRestaurantDto.Menu != null)
                restaurant.Menu = updateRestaurantDto.Menu;

            if (updateRestaurantDto.CoverImageUrl != null)
                restaurant.CoverImageUrl = updateRestaurantDto.CoverImageUrl;

            if (updateRestaurantDto.ImageUrl1 != null)
                restaurant.ImageUrl1 = updateRestaurantDto.ImageUrl1;

            if (updateRestaurantDto.ImageUrl2 != null)
                restaurant.ImageUrl2 = updateRestaurantDto.ImageUrl2;

            if (updateRestaurantDto.ImageUrl3 != null)
                restaurant.ImageUrl3 = updateRestaurantDto.ImageUrl3;

            if (updateRestaurantDto.IsActive.HasValue)
                restaurant.IsActive = updateRestaurantDto.IsActive.Value;

            if (updateRestaurantDto.OpenDay.HasValue)
                restaurant.OpenDay = updateRestaurantDto.OpenDay.Value;

            if (updateRestaurantDto.CloseDay.HasValue)
                restaurant.CloseDay = updateRestaurantDto.CloseDay.Value;

            if (!string.IsNullOrEmpty(updateRestaurantDto.OpenTime))
                restaurant.OpenTime = TimeSpan.Parse(updateRestaurantDto.OpenTime);

            if (!string.IsNullOrEmpty(updateRestaurantDto.CloseTime))
                restaurant.CloseTime = TimeSpan.Parse(updateRestaurantDto.CloseTime);

            // Atualizar endereço
            if (updateRestaurantDto.Address != null)
            {
                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.ZipCode))
                    restaurant.Address.ZipCode = updateRestaurantDto.Address.ZipCode;

                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.Street))
                    restaurant.Address.Street = updateRestaurantDto.Address.Street;

                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.Number))
                    restaurant.Address.Number = updateRestaurantDto.Address.Number;

                if (updateRestaurantDto.Address.Complement != null)
                    restaurant.Address.Complement = updateRestaurantDto.Address.Complement;

                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.City))
                    restaurant.Address.City = updateRestaurantDto.Address.City;

                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.State))
                    restaurant.Address.State = updateRestaurantDto.Address.State;

                if (!string.IsNullOrWhiteSpace(updateRestaurantDto.Address.Country))
                    restaurant.Address.Country = updateRestaurantDto.Address.Country;

                restaurant.Address.UpdatedAt = DateTime.UtcNow;
            }

            // Atualizar tags carregando as existentes
            if (updateRestaurantDto.TagIds != null)
            {
                restaurant.Categories.Clear();
                var existingTags = await _tagRepository.GetByIdsAsync(updateRestaurantDto.TagIds);
                restaurant.Categories = existingTags.ToList();
            }

            await _restaurantRepository.UpdateAsync(restaurant);
            return await GetByIdAsync(id);
        }

        public async Task<RestaurantDetailDto?> UpdateAllImagesAsync(long id, IFormFile? coverImage, IFormFile? image1, IFormFile? image2, IFormFile? image3)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
                return null;

            // Upload de cada imagem fornecida (apenas as não nulas)
            if (coverImage != null && coverImage.Length > 0)
            {
                var coverUrl = await _cloudinaryService.UploadImageAsync(coverImage, "RestaurantImages");
                restaurant.CoverImageUrl = coverUrl;
            }

            if (image1 != null && image1.Length > 0)
            {
                var url1 = await _cloudinaryService.UploadImageAsync(image1, "RestaurantImages");
                restaurant.ImageUrl1 = url1;
            }

            if (image2 != null && image2.Length > 0)
            {
                var url2 = await _cloudinaryService.UploadImageAsync(image2, "RestaurantImages");
                restaurant.ImageUrl2 = url2;
            }

            if (image3 != null && image3.Length > 0)
            {
                var url3 = await _cloudinaryService.UploadImageAsync(image3, "RestaurantImages");
                restaurant.ImageUrl3 = url3;
            }

            restaurant.UpdatedAt = DateTime.UtcNow;
            await _restaurantRepository.UpdateAsync(restaurant);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _restaurantRepository.DeleteAsync(id);
        }

        // Métodos auxiliares para mapeamento
        private RestaurantListDTO MapToListDto(Restaurant restaurant)
        {
            return new RestaurantListDTO
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Email = restaurant.Email,
                Description = restaurant.Description,
                CoverImageUrl = restaurant.CoverImageUrl,
                ImageUrl1 = restaurant.ImageUrl1,
                ImageUrl2 = restaurant.ImageUrl2,
                ImageUrl3 = restaurant.ImageUrl3,
                IsActive = restaurant.IsActive,
                Tags = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                City = restaurant.Address?.City ?? string.Empty,
                State = restaurant.Address?.State ?? string.Empty,
                OpenDay = restaurant.OpenDay,
                CloseDay = restaurant.CloseDay,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime
            };
        }

        private RestaurantDetailDto MapToDetailDto(Restaurant restaurant)
        {
            return new RestaurantDetailDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                Description = restaurant.Description,
                Site = restaurant.Site,
                Menu = restaurant.Menu,
                CoverImageUrl = restaurant.CoverImageUrl,
                ImageUrl1 = restaurant.ImageUrl1,
                ImageUrl2 = restaurant.ImageUrl2,
                ImageUrl3 = restaurant.ImageUrl3,
                IsActive = restaurant.IsActive,
                OpenDay = restaurant.OpenDay,
                CloseDay = restaurant.CloseDay,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime,
                Address = new AddressDto
                {
                    Id = restaurant.Address.Id,
                    ZipCode = restaurant.Address.ZipCode,
                    Street = restaurant.Address.Street,
                    Number = restaurant.Address.Number,
                    Complement = restaurant.Address.Complement,
                    City = restaurant.Address.City,
                    State = restaurant.Address.State,
                    Country = restaurant.Address.Country
                },
                Tags = restaurant.Categories?.Select(c => new TagDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList() ?? new List<TagDto>(),
                OwnerId = restaurant.Owner?.Id ?? 0,
                OwnerName = restaurant.Owner?.Name ?? string.Empty,
                CreatedAt = DateTimeHelper.ToSaoPauloTime(restaurant.CreatedAt)
            };
        }
    }
}
