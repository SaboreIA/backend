using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Service;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Retorna todos os favoritos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetAll()
        {
            try
            {
                var favorites = await _favoriteService.GetAllAsync();
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna um favorito específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteDto>> GetById(long id)
        {
            try
            {
                var favorite = await _favoriteService.GetByIdAsync(id);
                if (favorite == null)
                    return NotFound(new { message = "Favorite not found" });

                return Ok(favorite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna os favoritos do usuário autenticado
        /// </summary>
        [HttpGet("my-favorites")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetMyFavorites()
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                var favorites = await _favoriteService.GetByUserIdAsync(long.Parse(userId));
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna favoritos de um usuário específico
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetByUserId(long userId)
        {
            try
            {
                var favorites = await _favoriteService.GetByUserIdAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna favoritos de um restaurante específico
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetByRestaurantId(long restaurantId)
        {
            try
            {
                var favorites = await _favoriteService.GetByRestaurantIdAsync(restaurantId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se um restaurante está favoritado pelo usuário autenticado
        /// </summary>
        [HttpGet("status/{restaurantId}")]
        [Authorize]
        public async Task<ActionResult<FavoriteStatusDto>> GetFavoriteStatus(long restaurantId)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                var status = await _favoriteService.GetFavoriteStatusAsync(long.Parse(userId), restaurantId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna a quantidade de favoritos de um restaurante
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/count")]
        public async Task<ActionResult<object>> GetFavoriteCount(long restaurantId)
        {
            try
            {
                var count = await _favoriteService.GetFavoriteCountByRestaurantIdAsync(restaurantId);
                return Ok(new { favoriteCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Adiciona um restaurante aos favoritos
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FavoriteDto>> AddFavorite([FromBody] CreateFavoriteDto createFavoriteDto)
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

                var favorite = await _favoriteService.AddFavoriteAsync(createFavoriteDto, long.Parse(userId));
                return CreatedAtAction(nameof(GetById), new { id = favorite.Id }, favorite);
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
        /// Remove um restaurante dos favoritos
        /// </summary>
        [HttpDelete("restaurant/{restaurantId}")]
        [Authorize]
        public async Task<ActionResult> RemoveFavorite(long restaurantId)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                var result = await _favoriteService.RemoveFavoriteAsync(long.Parse(userId), restaurantId);
                if (!result)
                    return NotFound(new { message = "Favorite not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove um favorito específico por ID
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveFavoriteById(long id)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                var result = await _favoriteService.RemoveFavoriteByIdAsync(id, long.Parse(userId));
                if (!result)
                    return NotFound(new { message = "Favorite not found" });

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

        /// <summary>
        /// Toggle favorite - adiciona se não existe, remove se existe
        /// </summary>
        [HttpPost("toggle")]
        [Authorize]
        public async Task<ActionResult<FavoriteStatusDto>> ToggleFavorite([FromBody] CreateFavoriteDto createFavoriteDto)
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

                var currentStatus = await _favoriteService.GetFavoriteStatusAsync(long.Parse(userId), createFavoriteDto.RestaurantId);

                if (currentStatus.IsFavorite)
                {
                    // Remove dos favoritos
                    await _favoriteService.RemoveFavoriteAsync(long.Parse(userId), createFavoriteDto.RestaurantId);
                    return Ok(new FavoriteStatusDto { IsFavorite = false, FavoriteId = null });
                }
                else
                {
                    // Adiciona aos favoritos
                    var favorite = await _favoriteService.AddFavoriteAsync(createFavoriteDto, long.Parse(userId));
                    return Ok(new FavoriteStatusDto { IsFavorite = true, FavoriteId = favorite.Id });
                }
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
    }
}
