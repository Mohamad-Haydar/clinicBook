using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Models;
using api.Models.Responce;
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
        private readonly IMessageProvider _messageProvider;

        public CategoryData(ApplicationDbContext appDbContext, IMemoryCache cache, MemoryCacheEntryOptions cacheOptions, IMessageProvider messageProvider)
        {
            _appDbContext = appDbContext;
            _cache = cache;
            _cacheOptions = cacheOptions;
            _messageProvider = messageProvider;
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

        public async Task<Result> CreateCategoryAsync(string categoryName)
        {
            const string cacheKey = "categories";
            try
            {
                var res = await _appDbContext.Categories.AddAsync(new CategoryModel() { CategoryName = categoryName});
                if(res == null) 
                    return Result.Failure(_messageProvider.GetMessage("error"));
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
                await _appDbContext.SaveChangesAsync();
                return Result.Success(_messageProvider.GetMessage("createCategorySuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }


        public async Task<Result> UpdateCategoryAsync(CategoryModel model)
        {
            const string cacheKey = "categories";
            try
            {
                _cache.Remove(cacheKey);
                var category = await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
                if(category == null) return Result.Failure(_messageProvider.GetMessage("error"));
                category.CategoryName = model.CategoryName;
                await _appDbContext.SaveChangesAsync();
                return Result.Success(_messageProvider.GetMessage("updateCategorySuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {
            const string cacheKey = "categories";
            try
            {
                _cache.Remove(cacheKey);
                var categoryReserved = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.CategoryId == id).ConfigureAwait(false);
                if(categoryReserved != null) return Result.Failure(_messageProvider.GetMessage("failedToRemoveCategory"));
                var category = await _appDbContext.Categories.FirstAsync(x => x.Id == id).ConfigureAwait(false);
                _appDbContext.Remove(category);
                await _appDbContext.SaveChangesAsync();
                return Result.Success(_messageProvider.GetMessage("deleteCategorySuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }
}
