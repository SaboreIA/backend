using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Service;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Retorna uma review específica por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetById(long id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);
                if (review == null)
                    return NotFound(new { message = "Review not found" });

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna lista paginada de todas as avaliações
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDTO<ReviewDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { message = "Page number must be greater than 0" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "Page size must be between 1 and 50" });

                var paginatedReviews = await _reviewService.GetAllAsync(pageNumber, pageSize);
                return Ok(paginatedReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna reviews paginadas de um restaurante específico
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<PaginatedResponseDTO<ReviewDto>>> GetByRestaurantId(long restaurantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { message = "Page number must be greater than 0" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "Page size must be between 1 and 50" });

                var paginatedReviews = await _reviewService.GetByRestaurantIdAsync(restaurantId, pageNumber, pageSize);
                return Ok(paginatedReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna reviews paginadas de um usuário específico
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PaginatedResponseDTO<ReviewDto>>> GetByUserId(long userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { message = "Page number must be greater than 0" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "Page size must be between 1 and 50" });

                var paginatedReviews = await _reviewService.GetByUserIdAsync(userId, pageNumber, pageSize);
                return Ok(paginatedReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna a avaliação média de um restaurante
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/average")]
        public async Task<ActionResult<object>> GetAverageRating(long restaurantId)
        {
            try
            {
                var average = await _reviewService.GetAverageRatingByRestaurantIdAsync(restaurantId);
                var count = await _reviewService.GetReviewCountByRestaurantIdAsync(restaurantId);
                
                return Ok(new { averageRating = Math.Round(average, 2), reviewCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova review
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto createReviewDto)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var review = await _reviewService.CreateAsync(createReviewDto, long.Parse(userId));
                return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma review existente
        /// Somente o autor da review ou admin podem atualizar
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> Update(long id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            var userId = User.FindFirst("userId")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var review = await _reviewService.UpdateAsync(id, updateReviewDto, long.Parse(userId), userRole);
                if (review == null)
                    return NotFound(new { message = "Review not found" });

                return Ok(review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Upload de imagem para uma review
        /// Somente o autor da review ou admin podem fazer upload
        /// </summary>
        [HttpPost("{id}/upload-image")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UploadImage(long id, IFormFile imageFile)
        {
            var userId = User.FindFirst("userId")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest(new { message = "Nenhuma imagem foi enviada" });

                // Validar tipo de arquivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { message = "Formato de imagem não suportado. Use JPG, JPEG, PNG ou GIF" });

                // Validar tamanho (5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "A imagem não pode exceder 5MB" });

                var review = await _reviewService.UpdateReviewImageAsync(id, imageFile, long.Parse(userId), userRole);
                if (review == null)
                    return NotFound(new { message = "Review not found" });

                return Ok(review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao fazer upload da imagem", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma review
        /// Somente o autor da review ou admin podem deletar
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var userId = User.FindFirst("userId")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                var result = await _reviewService.DeleteAsync(id, long.Parse(userId), userRole);
                if (!result)
                    return NotFound(new { message = "Review not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
