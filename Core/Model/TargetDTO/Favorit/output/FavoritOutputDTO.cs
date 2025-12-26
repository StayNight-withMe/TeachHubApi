using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Favorit.output
{
    public class FavoritOutputDTO
    {
        public Hashid courseid { get; set; }
        public string coursename { get; set; }
        public string creatorname { get; set; }
        public string field {  get; set; }
    }
}
