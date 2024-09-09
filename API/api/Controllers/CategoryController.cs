using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryData _categoryData;

        public CategoryController(ICategoryData categoryData)
        {
            _categoryData = categoryData;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryData.GetAllCategoriesAsync().ConfigureAwait(false);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }
    }
}
