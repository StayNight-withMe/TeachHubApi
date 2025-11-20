
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace infrastructure.Utils.SortBuilder
{
    public class SortBuilder<T> 
    {
        private readonly List<T> _list;

        private readonly List<string> _banValueList = new();

        private bool _downSort = false;

        private IComparer<T>? _comparer = null;

        private string[]? _thenByList = null;

        public string? _orderBy = null;

        public SortBuilder(List<T> listForSort, params string[] banValuesForSort)
        {
            _list = listForSort;
            foreach (var item in banValuesForSort)
            {
                _banValueList.Add(item);
            }
        }


        public SortBuilder(List<T> listForSort)
        {
            _list = listForSort;
        }


        public SortBuilder<T> AddComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
            return this;
        }

        public SortBuilder<T> Descending()
        {
            _downSort = true;
            return this;
        }

        public SortBuilder<T> Descending(bool param)
        {
            if(param)
            _downSort = true;

            return this;
        }

        public SortBuilder<T> ThenBY(params string[] ThenByList)
        {

            bool right = false;

            if (_thenByList != null)
            {
                right = true;
            }


            
            if(_banValueList != null && right == true)
            {
                foreach (var item in _banValueList)
                {
                    if (_thenByList.Contains(item))
                    {
                        return this;
                    }
                     
                }
            }

            _thenByList = ThenByList;

            return this;
        }

        public SortBuilder<T> OrderBy(string OrderBy)
        {
            if(!_banValueList.Contains(_orderBy))
            {
                _orderBy = OrderBy;
            }
            return this;
        }


        public List<T>? Build()
        {


            if (string.IsNullOrWhiteSpace(_orderBy))
                return null;



            if (_thenByList != null && _thenByList.Any(c => _banValueList.Contains(c))) return null;

            IQueryable<T> query = _comparer != null ? _list
                .AsQueryable()
                .OrderBy(c => c, _comparer) : _list
                .AsQueryable() ;


            if(_comparer == null)
            {

                string der = _downSort ? "descending" : "";
                query = query.
                    OrderBy($"{_orderBy} {der}".Trim());
                try
                {
                    if (_thenByList != null)
                    {
                        foreach (var item in _thenByList)
                        {
                            query = ((IOrderedQueryable<T>)query).ThenBy($"{item} {der}".Trim());
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"ошибка сортировки : {ex.Message}");
                    return null;
                }
             
            }


            return query.ToList();

        }

    }
}
