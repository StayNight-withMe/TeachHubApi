using Core.Model.TargetDTO.Common.input;

namespace infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetWithPagination<T>(this IQueryable<T> qwery, UserSortingRequest userSortingRequest)
        {
            return qwery.Skip(userSortingRequest.PageSize * (userSortingRequest.PageNumber - 1))
                .Take(userSortingRequest.PageSize);
               
        }
    }
}
