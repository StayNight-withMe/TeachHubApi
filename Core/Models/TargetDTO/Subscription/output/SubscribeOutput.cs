using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Subscription.output
{
    public class SubscribeOutput
    {
        public string username { get; set; }
        public Hashid id { get; set; }

    }
}
