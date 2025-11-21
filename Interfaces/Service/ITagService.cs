using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface ITagService
    {
        Task<TagDto?> GetByIdAsync(long id);
        Task<TagDto?> GetByNameAsync(string name);
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto> CreateAsync(CreateTagDto createCategoryDto);
        Task<TagDto?> UpdateAsync(long id, UpdateTagDto updateCategoryDto);
        Task<bool> DeleteAsync(long id);
    }
}
