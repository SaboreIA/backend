using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Service;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        // Injeção das dependências: IUserService e IAuthorizationService
        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Retorna todos os usuários - somente admin pode acessar
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna um usuário especificado pelo ID
        /// Só o próprio usuário ou admin podem acessar
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(long id)
        {
            try
            {
                var currentUserIdStr = User.FindFirst("userId")?.Value;
                if (!long.TryParse(currentUserIdStr, out var currentUserId))
                    return Unauthorized();

                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

                if (currentUserId != id && !string.Equals(currentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized();
                }

                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// Somente o próprio usuário ou admin podem atualizar
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(long id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "IsOwnerOrAdmin");
                if (!authorizationResult.Succeeded)
                    return Forbid();

                var user = await _userService.UpdateAsync(id, updateUserDto);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
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
        /// Upload de imagem de perfil do usuário
        /// Somente o próprio usuário ou admin podem fazer upload
        /// </summary>
        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<UserDto>> UploadImage(long id, IFormFile imageFile)
        {
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

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "IsOwnerOrAdmin");
                if (!authorizationResult.Succeeded)
                    return Forbid();

                var user = await _userService.UpdateUserImageAsync(id, imageFile);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao fazer upload da imagem", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um usuário pelo ID - apenas admins podem deletar
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "User not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
