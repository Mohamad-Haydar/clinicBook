using api.Models;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ICategoryData
    {
        Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync();
        Task CreateCategoryAsync(string categoryName);
        Task UpdateCategoryAsync(CategoryModel model);
        Task DeleteCategoryAsync(int model);
    }
}