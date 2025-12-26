using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Category.input
{
    public class CategoryResponseDTO
    {
        public Hashid id { get; set; }
        public Hashid parentid { get; set; }
        public string name { get; set; }
    }
}
