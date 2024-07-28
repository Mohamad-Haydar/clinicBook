using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Responce;
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
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryData.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception)
            {
                return BadRequest(new Response("Something whent wrong, please try again"));
            }
        }
    }
}
