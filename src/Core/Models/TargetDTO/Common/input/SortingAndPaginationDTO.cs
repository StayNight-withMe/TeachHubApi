using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Common.input
{
    public class SortingAndPaginationDTO
    {
        public string? OrderBy { get; set; } = string.Empty;
        public string[]? ThenBy { get; set; } = default;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool desc { get; set; } = false;
    }
}
