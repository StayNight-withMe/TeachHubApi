using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.PageService
{
    public static class PageService
    {
        private static bool PageValidate(int pageSize, int pageNumber)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static TResult<PagedResponseDTO<T>> CreatePage<T>(List<T> list,UserSortingRequest userSortingRequest, int totalCount)
        {

             bool validate = PageValidate(userSortingRequest.PageSize, userSortingRequest.PageNumber);

             if(!validate)
            {
                return TResult<PagedResponseDTO<T>>.FailedOperation(errorCode.InvalidDataFormat, "ошибка загрузки страницы");
            }
            

            if (list == null)
            {
                return TResult<PagedResponseDTO<T>>.FailedOperation(errorCode.InvalidPagination, "неправильный фильтр");
            }



            return TResult<PagedResponseDTO<T>>.CompletedOperation(
             new PagedResponseDTO<T>
             {
                 Data = list,

                 Pagination = new PaginationInfo
                 {
                     PageSize = userSortingRequest.PageSize,

                     Page = userSortingRequest.PageNumber,

                     TotalCount = totalCount,

                 }
             }

             );
        }



    }
}
