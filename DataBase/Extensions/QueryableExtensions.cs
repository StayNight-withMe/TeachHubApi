using Core.Model.TargetDTO.Common.input;
using System.Linq.Dynamic.Core;

namespace infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetWithPaginationAndSorting<T>(this IQueryable<T> qwery, UserSortingRequest userSortingRequest, params string[] banvalue)
        {

            if (string.IsNullOrWhiteSpace(userSortingRequest.OrderBy))
                return qwery;

            if (userSortingRequest.ThenBy != null && userSortingRequest.ThenBy.Any(c => banvalue.Contains(c))) return qwery;

            string der = userSortingRequest.desc ? "descending" : "";
            var query = qwery.
               OrderBy($"{userSortingRequest.OrderBy} {der}".Trim());
            try
            {
                if (userSortingRequest.ThenBy != null)
                {
                    foreach (var item in userSortingRequest.ThenBy)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy($"{item} {der}".Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ошибка сортировки : {ex.Message}");
                return query;
            }

             var finnaly = query.Skip(userSortingRequest.PageSize * (userSortingRequest.PageNumber - 1))
      .Take(userSortingRequest.PageSize);


            return finnaly;
            }

        }



    }

