using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;

namespace SaboreIA.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _categoryRepository;

        public TagService(ITagRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<TagDto?> GetByIdAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<TagDto?> GetByNameAsync(string name)
        {
            var category = await _categoryRepository.GetByNameAsync(name);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<TagDto> CreateAsync(CreateTagDto createCategoryDto)
        {
            // Validar se categoria já existe
            if (await _categoryRepository.NameExistsAsync(createCategoryDto.Name))
            {
                throw new InvalidOperationException("Category with this name already exists.");
            }

            var category = new Tag
            {
                Name = createCategoryDto.Name
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<TagDto?> UpdateAsync(long id, UpdateTagDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            // Atualizar campos não nulos
            if (!string.IsNullOrWhiteSpace(updateCategoryDto.Name))
            {
                // Verificar se novo nome já existe em outra categoria
                if (category.Name != updateCategoryDto.Name &&
                    await _categoryRepository.NameExistsAsync(updateCategoryDto.Name))
                {
                    throw new InvalidOperationException("Category with this name already exists.");
                }
                category.Name = updateCategoryDto.Name;
            }

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return MapToDto(updatedCategory);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            // Verificar se categoria está sendo usada por restaurantes
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            if (category.Restaurants != null && category.Restaurants.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot delete category '{category.Name}' because it is being used by {category.Restaurants.Count} restaurant(s).");
            }

            return await _categoryRepository.DeleteAsync(id);
        }

        // Método auxiliar para mapear Tag -> TagDto
        private TagDto MapToDto(Tag category)
        {
            return new TagDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
