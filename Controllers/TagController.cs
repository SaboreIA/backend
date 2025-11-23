using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Service;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Retorna todas as tags de restaurantes (público)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            try
            {
                var tags = await _tagService.GetAllAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna uma tag específica por ID (público)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(long id)
        {
            try
            {
                var tag = await _tagService.GetByIdAsync(id);
                if (tag == null)
                    return NotFound(new { message = "Tag not found" });

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna uma tag por nome (público)
        /// </summary>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<TagDto>> GetByName(string name)
        {
            try
            {
                var tag = await _tagService.GetByNameAsync(name);
                if (tag == null)
                    return NotFound(new { message = "Tag not found" });

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova tag
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto createTagDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tag = await _tagService.CreateAsync(createTagDto);
                return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
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
        /// Atualiza uma tag existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<TagDto>> Update(long id, [FromBody] UpdateTagDto updateTagDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tag = await _tagService.UpdateAsync(id, updateTagDto);
                if (tag == null)
                    return NotFound(new { message = "Tag not found" });

                return Ok(tag);
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
        /// Deleta uma tag
        /// Apenas administradores podem deletar tags
        /// Não é possível deletar tags que estão sendo usadas por restaurantes
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var result = await _tagService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Tag not found" });

                return NoContent();
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
