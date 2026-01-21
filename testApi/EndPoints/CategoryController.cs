using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Models.TargetDTO.Common.input;
using Microsoft.AspNetCore.Mvc;
using testApi.WebUtils.EntityResultExtensions;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/categories")]
    [Tags("Категории")]
    [ApiVersion("1.0")]
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
