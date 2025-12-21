using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Options
{
    public class BloomOptions
    {
        public  string BloomFilterDirectory { get; set; }
        public string BloomDataFileName { get; set; }
        public string BloomTempFileName { get; set; }
        public int expectedElements { get; set; }
        public double errorRate { get; set; }
    }
}
