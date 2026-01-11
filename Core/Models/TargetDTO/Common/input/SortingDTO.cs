using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Common.input
{
    public class SortingDTO
    {
        public string OrderBy { get; set; }
        public string[]? ThenBy { get; set; }
        public bool desc { get; set; } = false;
    }
}
