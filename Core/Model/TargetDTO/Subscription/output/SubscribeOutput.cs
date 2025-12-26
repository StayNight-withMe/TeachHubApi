using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Subscription.output
{
    public class SubscribeOutput
    {
        public string username { get; set; }
        public Hashid id { get; set; }

    }
}
