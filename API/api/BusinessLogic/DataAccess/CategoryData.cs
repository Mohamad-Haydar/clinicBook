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
    }
}
