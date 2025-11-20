using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Common.input
{
    public class UserSortingRequest
    {
        public string? OrderBy { get; set; }
        public string[]? ThenBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool desc { get; set; } = false;
    }
}
