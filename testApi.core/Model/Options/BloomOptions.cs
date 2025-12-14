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
        public BloomOptions(
            string bloomFilterDrectory, 
            string bloomDataFileName = "bloomData.bin",
            string bloomTempFileName = "bloomTemp.tmp" ) 
        {
            BloomFilterDirectory = bloomFilterDrectory;
            BloomDataFileName = bloomDataFileName;
            BloomTempFileName = bloomTempFileName;
        }
       
    }
}
