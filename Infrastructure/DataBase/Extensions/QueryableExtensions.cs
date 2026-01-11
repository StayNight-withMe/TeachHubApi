using Core.Models.TargetDTO.Common.input;
using System.Linq.Dynamic.Core;

namespace infrastructure.DataBase.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetWithPaginationAndSorting<T>(this IQueryable<T> qwery, SortingAndPaginationDTO userSortingRequest, params string[] banvalue)
        {

            if (string.IsNullOrWhiteSpace(userSortingRequest.OrderBy))
                return qwery;

            if (userSortingRequest.ThenBy != null && userSortingRequest
                .ThenBy
                .Any(c => banvalue.Contains(c)))
            {
                return qwery;
            }

            string? der = userSortingRequest.desc ? "descending" : "";
            
            
            
            try
            {
                var query = qwery.
            OrderBy($"{userSortingRequest.OrderBy} {der}".Trim());

                if (userSortingRequest.ThenBy != null)
                {
                    foreach (var item in userSortingRequest.ThenBy)
                    {
                        query = query.ThenBy($"{item} {der}".Trim());
                    }
                }
                var finnaly = query.Skip(userSortingRequest.PageSize * (userSortingRequest.PageNumber - 1))
             .Take(userSortingRequest.PageSize);


                return finnaly;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"ошибка сортировки : {ex.Message}");
                return qwery.Order();
            }

           
            }


        public static IQueryable<T> GetWithSorting<T>(this IQueryable<T> qwery, SortingDTO userSortingRequest, params string[] banvalue)
        {

            if (string.IsNullOrWhiteSpace(userSortingRequest.OrderBy))
                return qwery;

            if (userSortingRequest.ThenBy != null && userSortingRequest
                .ThenBy
                .Any(c => banvalue.Contains(c)))
            {
                return qwery;
            }

            string? der = userSortingRequest.desc ? "descending" : "";



            try
            {
                var query = qwery.
            OrderBy($"{userSortingRequest.OrderBy} {der}".Trim());

                if (userSortingRequest.ThenBy != null)
                {
                    foreach (var item in userSortingRequest.ThenBy)
                    {
                        query = query.ThenBy($"{item} {der}".Trim());
                    }
                }

                return query;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"ошибка сортировки : {ex.Message}");
                return qwery.Order();
            }


        }




        public static IQueryable<T> GetWithPagination<T>(this IQueryable<T> qwery, PaginationDTO userSortingRequest, params string[] banvalue)
        {
            var finnaly = qwery
                .Skip(userSortingRequest.PageSize * (userSortingRequest.PageNumber - 1))
                .Take(userSortingRequest.PageSize);
            return finnaly;
        }


        public static IQueryable<T> GetWithPagination<T>(this IQueryable<T> qwery, SortingAndPaginationDTO userSortingRequest, params string[] banvalue)
        {
                var finnaly = qwery
                .Skip(userSortingRequest.PageSize * (userSortingRequest.PageNumber - 1))
                .Take(userSortingRequest.PageSize);
                return finnaly;

        }

    }



}

