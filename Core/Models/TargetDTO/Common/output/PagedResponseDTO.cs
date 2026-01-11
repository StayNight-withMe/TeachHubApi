using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Common.output
{
    public class PagedResponseDTO<T>
    {
        public List<T> Data { get; set; }
        public PaginationInfo Pagination { get; set; }
    }
}
