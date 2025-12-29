using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Utils.PageService;
using AutoMapper;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Category.input;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Application.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<CategoriesEntities> _categoryRepository;

        private readonly ILogger<CategoryService> _logger;


        public CategoryService( 
            IBaseRepository<CategoriesEntities>  categoryRepository, 
            ILogger<CategoryService> logger
            ) 
        {
        _categoryRepository = categoryRepository;
        _logger = logger;
        }


        public async Task<TResult<PagedResponseDTO<CategoryResponseDTO>>> SearchCategory(
            string searchText, 
            PaginationDTO pagination,
            CancellationToken cancellationToken = default
            )
        {
            var qwery = _categoryRepository
                .GetAllWithoutTracking()
                .Where(c => EF.Functions.ILike(c.name, $"%{searchText}%"));

            var resultEntities =  await qwery.GetWithPagination(pagination)
                .OrderBy(c => c.parentid)
                .OrderBy(c => c.name)
                .ToListAsync();

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
                await qwery.CountAsync()
                );


        }


    }
}
