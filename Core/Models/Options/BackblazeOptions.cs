using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Storage
{
    public class BackblazeOptions
    {
        public string KeyId { get; set; } = string.Empty;
        public string ApplicationKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
    }
}
