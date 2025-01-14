using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryData _categoryData;
        private readonly Response _response;

        public CategoryController(ICategoryData categoryData, Response response)
        {
            _categoryData = categoryData;
            _response = response;
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
                // return BadRequest(new Response(ex.Message));
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost]
        [Route("CreateCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            try
            {
                await _categoryData.CreateCategoryAsync(categoryName).ConfigureAwait(false);
                return Ok(new Response("تم انشاء الاختصاص بنجاح."));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }

        [HttpPatch]
        [Route("UpdateCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel model)
        {
            try
            {
                await _categoryData.UpdateCategoryAsync(model).ConfigureAwait(false);
                return Ok(new Response("تم تحديث الاختصاص بنجاح."));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryData.DeleteCategoryAsync(id).ConfigureAwait(false);
                return Ok(new Response("تم حذف الاختصاص بنجاح."));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }
    }
}
