using SaboreIA.Models;

namespace SaboreIA.Interfaces.Repository
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(long id);
        Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<long> ids);
        Task<Tag?> GetByNameAsync(string name);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> CreateAsync(Tag category);
        Task<Tag> UpdateAsync(Tag category);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<bool> NameExistsAsync(string name);
    }
}
