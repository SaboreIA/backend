using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;
using SaboreIA.Integrations.Cloudinary;
using SaboreIA.Helpers;

namespace SaboreIA.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ReviewService(
            IReviewRepository reviewRepository,
            IRestaurantRepository restaurantRepository,
            IUserRepository userRepository,
            ICloudinaryService cloudinaryService)
        {
            _reviewRepository = reviewRepository;
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ReviewDto?> GetByIdAsync(long id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review != null ? MapToDto(review) : null;
        }

        // Método atualizado com paginação
        public async Task<PaginatedResponseDTO<ReviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var reviews = await _reviewRepository.GetAllAsync();

            // Paginar os resultados
            var paginatedReviews = reviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            // Criar a resposta paginada
            return new PaginatedResponseDTO<ReviewDto>
            {
                Items = paginatedReviews,
                TotalItems = reviews.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Método por restaurante com paginação e contagem total de restaurantes
        public async Task<PaginatedResponseDTO<ReviewDto>> GetByRestaurantIdAsync(long restaurantId, int pageNumber = 1, int pageSize = 10)
        {
            var reviews = await _reviewRepository.GetByRestaurantIdAsync(restaurantId);;

            // Paginar os resultados
            var paginatedReviews = reviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            // Criar a resposta paginada com total de restaurantes
            var response = new PaginatedResponseDTO<ReviewDto>
            {
                Items = paginatedReviews,
                TotalItems = reviews.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return response;
        }

        // Método por usuário com paginação e contagem total de restaurantes  
        public async Task<PaginatedResponseDTO<ReviewDto>> GetByUserIdAsync(long userId, int pageNumber = 1, int pageSize = 10)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);

            // Paginar os resultados
            var paginatedReviews = reviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            // Criar a resposta paginada com total de restaurantes
            var response = new PaginatedResponseDTO<ReviewDto>
            {
                Items = paginatedReviews,
                TotalItems = reviews.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return response;
        }

        public async Task<ReviewDto> CreateAsync(CreateReviewDto createReviewDto, long userId)
        {
            // Verificar se o usuário existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Verificar se o restaurante existe
            var restaurant = await _restaurantRepository.GetByIdAsync(createReviewDto.RestaurantId);
            if (restaurant == null)
                throw new InvalidOperationException("Restaurant not found");

            // Verificar se o usuário já fez review deste restaurante
            var existingReview = await _reviewRepository.GetByUserAndRestaurantAsync(userId, createReviewDto.RestaurantId);
            if (existingReview != null)
                throw new InvalidOperationException("User already reviewed this restaurant");

            double AvgRating = (createReviewDto.Rating1 + createReviewDto.Rating2 + createReviewDto.Rating3 + createReviewDto.Rating4) / 4;

            var review = new Review
            {
                Title = createReviewDto.Title,
                Comment = createReviewDto.Comment,
                ImageUrl = createReviewDto.ImageUrl,
                Rating1 = createReviewDto.Rating1,
                Rating2 = createReviewDto.Rating2,
                Rating3 = createReviewDto.Rating3,
                Rating4 = createReviewDto.Rating4,
                AvgRating = AvgRating,
                UserId = userId,
                RestaurantId = createReviewDto.RestaurantId
            };

            var createdReview = await _reviewRepository.CreateAsync(review);
            return MapToDto(createdReview);
        }

        public async Task<ReviewDto?> UpdateAsync(long id, UpdateReviewDto updateReviewDto, long userId, string userRole)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return null;

            // Verificar se o usuário é o dono do review ou ADMIN
            if (review.UserId != userId && !string.Equals(userRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("User can only update their own reviews");

            // Atualizar campos se fornecidos
            if (!string.IsNullOrWhiteSpace(updateReviewDto.Title))
                review.Title = updateReviewDto.Title;

            if (!string.IsNullOrWhiteSpace(updateReviewDto.Comment))
                review.Comment = updateReviewDto.Comment;

            if (!string.IsNullOrWhiteSpace(updateReviewDto.ImageUrl))
                review.ImageUrl = updateReviewDto.ImageUrl;

            if (updateReviewDto.Rating1.HasValue)
                review.Rating1 = updateReviewDto.Rating1.Value;

            if (updateReviewDto.Rating2.HasValue)
                review.Rating2 = updateReviewDto.Rating2.Value;

            if (updateReviewDto.Rating3.HasValue)
                review.Rating3 = updateReviewDto.Rating3.Value;

            if (updateReviewDto.Rating4.HasValue)
                review.Rating4 = updateReviewDto.Rating4.Value;

            // Recalcular AvgRating se algum rating foi atualizado
            if (updateReviewDto.Rating1.HasValue || updateReviewDto.Rating2.HasValue ||
                updateReviewDto.Rating3.HasValue || updateReviewDto.Rating4.HasValue)
            {
                review.AvgRating = (review.Rating1 + review.Rating2 + review.Rating3 + review.Rating4) / 4;
            }

            var updatedReview = await _reviewRepository.UpdateAsync(review);
            return MapToDto(updatedReview);
        }

        public async Task<ReviewDto?> UpdateReviewImageAsync(long id, IFormFile imageFile, long userId, string userRole)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return null;

            // Verificar se o usuário é o dono da review ou ADMIN
            if (review.UserId != userId && !string.Equals(userRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("User can only update their own reviews");

            // Fazer upload da nova imagem para o Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "ReviewImages");

            // Atualizar a URL da imagem na review
            review.ImageUrl = imageUrl;
            review.UpdatedAt = DateTime.UtcNow;

            var updatedReview = await _reviewRepository.UpdateAsync(review);
            return MapToDto(updatedReview);
        }

        public async Task<bool> DeleteAsync(long id, long userId, string userRole)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return false;

            // Verificar se o usuário é o dono do review ou ADMIN
            if (review.UserId != userId && !string.Equals(userRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("User can only delete their own reviews");

            return await _reviewRepository.DeleteAsync(id);
        }

        public async Task<double> GetAverageRatingByRestaurantIdAsync(long restaurantId)
        {
            return await _reviewRepository.GetAverageRatingByRestaurantIdAsync(restaurantId);
        }

        public async Task<int> GetReviewCountByRestaurantIdAsync(long restaurantId)
        {
            return await _reviewRepository.GetReviewCountByRestaurantIdAsync(restaurantId);
        }

        private static ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Title = review.Title,
                Comment = review.Comment,
                ImageUrl = review.ImageUrl,
                Rating1 = review.Rating1,
                Rating2 = review.Rating2,
                Rating3 = review.Rating3,
                Rating4 = review.Rating4,
                AvgRating = review.AvgRating,
                UserId = review.UserId,
                RestaurantId = review.RestaurantId,
                CreatedAt = DateTimeHelper.ToSaoPauloTime(review.CreatedAt),
                UpdatedAt = DateTimeHelper.ToSaoPauloTime(review.UpdatedAt),
                UserName = review.User?.Name ?? "Unknown"
            };
        }
    }
}