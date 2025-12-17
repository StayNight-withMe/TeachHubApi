using Core.Model.TargetDTO.Common.input;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Service;  

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/categories")]
    [Tags("Категории")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> Search(
        [FromQuery] PaginationDTO pagination,
        [FromQuery] string searchText
            )
        {
            var result = await _categoryService.SearchCategory(searchText, pagination);
            return await EntityResultExtensions.ToActionResult(result, this);
        }
    }
}
