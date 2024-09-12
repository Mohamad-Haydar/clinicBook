using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace api.BusinessLogic.DataAccess
{
    public class CategoryData : ICategoryData
    {
        private readonly ILogger<CategoryData> _logger;
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CategoryData(ApplicationDbContext appDbContext, IMemoryCache cache, MemoryCacheEntryOptions cacheOptions)
        {
            _appDbContext = appDbContext;
            _cache = cache;
            _cacheOptions = cacheOptions;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
        {
            const string cacheKey = "categories";
            try
            {
                if (!_cache.TryGetValue(cacheKey, out IEnumerable<CategoryModel> categories))
                {
                    categories = await _appDbContext.Categories.ToListAsync().ConfigureAwait(false);
                    _cache.Set(cacheKey, categories, _cacheOptions);
                }
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task UpdateCategoryAsync(CategoryModel model)
        {
            const string cacheKey = "categories";
            try
            {
                _cache.Remove(cacheKey);
                var category = await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false) ?? throw new BusinessException();
                category.CategoryName = model.CategoryName;
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            const string cacheKey = "categories";
            try
            {
                _cache.Remove(cacheKey);
                var categoryReserved = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.CategoryId == id).ConfigureAwait(false);
                if(categoryReserved != null)
                {
                    throw new BusinessException("هناك اطباء مسجلين في هذا الختصاص, لا يجب ازالتهم جميعا.");
                }
                var category = await _appDbContext.Categories.FirstAsync(x => x.Id == id).ConfigureAwait(false);
                _appDbContext.Remove(category);
                await _appDbContext.SaveChangesAsync();
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }
}
