using Application.Abstractions.Service;
using Application.Utils.PageService;
using Microsoft.Extensions.Logging;
using Application.Abstractions.Repository.Custom;
using Core.Models.TargetDTO.Category.input;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Common.input;

namespace Application.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly ILogger<CategoryService> _logger;


        public CategoryService(
            ICategoryRepository categoryRepository, 
            ILogger<CategoryService> logger
            ) 
        {
        _categoryRepository = categoryRepository;
        _logger = logger;
        }


        public async Task<TResult<PagedResponseDTO<CategoryResponseDTO>>> SearchCategory(
            string searchText, 
            PaginationDTO pagination,
            CancellationToken ct = default
            )
        {
            var categoryEntities =  await _categoryRepository.SearchCategory(
                searchText, 
                pagination, 
                ct);


            var resultDTo = categoryEntities.Select(c =>
            new CategoryResponseDTO
            {
                id = c.id,
                name = c.name,
                parentid = c.parentid
            })
            .ToList();


            return PageService.CreatePage(
                resultDTo,
                pagination,
                await _categoryRepository.CountBySearch(searchText)
                );


        }


    }
}
