using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Utils.PageService;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Category.input;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using infrastructure.DataBase.Entitiеs;
using Microsoft.Extensions.Logging;
using Application.Abstractions.Repository.Custom;

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
            var categoryEntities = _categoryRepository.SearchCategory(
                searchText, 
                pagination, 
                ct);


            var resultDTo = resultEntities.Select(c =>
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
                await categoryEntities.CountAsync()
                );


        }


    }
}
