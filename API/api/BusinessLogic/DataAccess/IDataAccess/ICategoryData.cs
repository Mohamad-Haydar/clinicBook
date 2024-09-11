using api.Models;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ICategoryData
    {
        Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync();
        Task UpdateCategoryAsync(CategoryModel model);
        Task DeleteCategoryAsync(CategoryModel model);
    }
}