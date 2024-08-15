using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic.DataAccess
{
    public class CategoryData : ICategoryData
    {
        private readonly ApplicationDbContext _appDbContext;

        public CategoryData(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
        {
            try
            {
                var result = await _appDbContext.Categories.ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw new BusinessException();
            }
        }
    }
}
