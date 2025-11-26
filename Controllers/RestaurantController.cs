using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Service;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IIAService _iaService;
        private readonly IAuthorizationService _authorizationService;

        public RestaurantsController(
            IRestaurantService restaurantService, 
            IIAService iaService,
            IAuthorizationService authorizationService)
        {
            _restaurantService = restaurantService;
            _iaService = iaService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Busca restaurantes baseado na entrada textual do usuário,
        /// usando IA para identificar o tipo de restaurante e consultando o banco.
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRestaurantDto searchRestaurant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Usa IAService para extrair/cadastrar a tag do termo
            long? tagId = await _iaService.GetOrAddTagAsync(searchRestaurant.UserInput);

            if (tagId == null)
                return NotFound(new { message = "Não foi possível identificar um tipo de restaurante válido da entrada." });

            // Busca restaurantes pela tag retornada pela IA
            var restaurantes = await _restaurantService.GetByTagIdAsync(tagId.Value);

            if (restaurantes == null || !restaurantes.Any())
                return NotFound(new { message = $"Nenhum restaurante encontrado para a tag." });

            return Ok(restaurantes);
        }

        /// <summary>
        /// Retorna todos os restaurantes ativos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDTO<RestaurantListDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { message = "Page number must be greater than 0" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "Page size must be between 1 and 50" });

                var paginatedRestaurants = await _restaurantService.GetAllAsync(pageNumber, pageSize);
                return Ok(paginatedRestaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna um restaurante específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDetailDto>> GetById(long id)
        {
            try
            {
                var restaurant = await _restaurantService.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound(new { message = "Restaurant not found" });

                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna restaurantes de um proprietário específico
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<RestaurantListDTO>>> GetByOwnerId(long ownerId)
        {
            try
            {
                var restaurants = await _restaurantService.GetByOwnerIdAsync(ownerId);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo restaurante
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RestaurantDetailDto>> Create([FromBody] CreateRestaurantDto createRestaurantDto)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                return Forbid();
            }

            if (!TimeSpan.TryParse(createRestaurantDto.OpenTime, out var openTime))
                return BadRequest("OpenTime inválido. Use formato HH:MM");

            if (!TimeSpan.TryParse(createRestaurantDto.CloseTime, out var closeTime))
                return BadRequest("CloseTime inválido. Use formato HH:MM");

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var restaurant = await _restaurantService.CreateAsync(createRestaurantDto, long.Parse(userId));
                return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
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
        /// Atualiza um restaurante existente
        /// Somente o proprietário do restaurante ou admin podem atualizar
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RestaurantDetailDto>> Update(long id, [FromBody] UpdateRestaurantDto updateRestaurantDto)
        {
            // Validar apenas se os campos foram fornecidos
            if (!string.IsNullOrEmpty(updateRestaurantDto.OpenTime) &&
                !TimeSpan.TryParse(updateRestaurantDto.OpenTime, out var openTime))
                return BadRequest("OpenTime inválido. Use formato HH:MM");

            if (!string.IsNullOrEmpty(updateRestaurantDto.CloseTime) &&
                !TimeSpan.TryParse(updateRestaurantDto.CloseTime, out var closeTime))
                return BadRequest("CloseTime inválido. Use formato HH:MM");

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Buscar o restaurante para validar ownership
                var restaurant = await _restaurantService.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound(new { message = "Restaurant not found" });

                // Validar se o usuário é o owner do restaurante ou admin
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, restaurant.OwnerId, "IsOwnerOrAdmin");
                if (!authorizationResult.Succeeded)
                    return Forbid();

                var updatedRestaurant = await _restaurantService.UpdateAsync(id, updateRestaurantDto);
                return Ok(updatedRestaurant);
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
        /// Upload de múltiplas imagens do restaurante de uma só vez
        /// Todas as imagens são opcionais - envie apenas as que deseja atualizar
        /// Somente o proprietário do restaurante ou admin podem fazer upload
        /// </summary>
        [HttpPost("{id}/upload-all-images")]
        [Authorize]
        public async Task<ActionResult<RestaurantDetailDto>> UploadAllImages(
            long id,
            IFormFile? coverImage,
            IFormFile? image1,
            IFormFile? image2,
            IFormFile? image3)
        {
            try
            {
                // Verificar se pelo menos uma imagem foi enviada
                if (coverImage == null && image1 == null && image2 == null && image3 == null)
                    return BadRequest(new { message = "Nenhuma imagem foi enviada. Envie pelo menos uma imagem." });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var maxSize = 5 * 1024 * 1024; // 5MB

                // Validar cada imagem enviada
                var imagesToValidate = new[] {
                    (file: coverImage, name: "coverImage"),
                    (file: image1, name: "image1"),
                    (file: image2, name: "image2"),
                    (file: image3, name: "image3")
                };

                foreach (var (file, name) in imagesToValidate)
                {
                    if (file != null && file.Length > 0)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                            return BadRequest(new { message = $"{name}: Formato de imagem não suportado. Use JPG, JPEG, PNG ou GIF" });

                        if (file.Length > maxSize)
                            return BadRequest(new { message = $"{name}: A imagem não pode exceder 5MB" });
                    }
                }

                // Buscar o restaurante para validar ownership
                var restaurant = await _restaurantService.GetByIdAsync(id);
                if (restaurant == null) return NotFound(new { message = "Restaurant not found" });

                // Validar se o usuário é o owner do restaurante ou admin
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, restaurant.OwnerId, "IsOwnerOrAdmin");
                if (!authorizationResult.Succeeded) return Forbid();

                var updatedRestaurant = await _restaurantService.UpdateAllImagesAsync(id, coverImage, image1, image2, image3);
                return Ok(updatedRestaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao fazer upload das imagens", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um restaurante
        /// O proprietário do restaurante ou administrador podem deletar
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                // Buscar o restaurante para validar ownership
                var restaurant = await _restaurantService.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound(new { message = "Restaurant not found" });

                // Validar se o usuário é o owner do restaurante ou admin
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, restaurant.OwnerId, "IsOwnerOrAdmin");
                if (!authorizationResult.Succeeded)
                    return Forbid();

                var result = await _restaurantService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Restaurant not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
