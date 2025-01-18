using api.BusinessLogic.DataAccess.IDataAccess;
using api.Exceptions;
using api.Helper;
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
        private readonly IMessageProvider _messageProvider;

        public CategoryController(ICategoryData categoryData, IMessageProvider messageProvider)
        {
            _categoryData = categoryData;
            _messageProvider = messageProvider;
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
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpPost]
        [Route("CreateCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            try
            {
                var res = await _categoryData.CreateCategoryAsync(categoryName).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpPatch]
        [Route("UpdateCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel model)
        {
            try
            {
                var res = await _categoryData.UpdateCategoryAsync(model).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var res = await _categoryData.DeleteCategoryAsync(id).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }
    }
}
