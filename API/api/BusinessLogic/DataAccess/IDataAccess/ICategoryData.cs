using api.Models;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ICategoryData
    {
        Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync();
        Task<Result> CreateCategoryAsync(string categoryName);
        Task<Result> UpdateCategoryAsync(CategoryModel model);
        Task<Result> DeleteCategoryAsync(int model);
    }
}