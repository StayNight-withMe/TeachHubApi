using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloomFilter;

namespace infrastructure.Utils.BloomFilter
{
    public class EmailChecker
    {
        const int exElement = 100_000_000;
        const float errorRate = 0.001f;

        private readonly IBloomFilter _BloomFilter =  FilterBuilder.Build(exElement, errorRate);
        /// <summary>
        /// Проверка email на существование в базе через bloom filter
        /// true - такой уже есть 
        /// false - такого нет
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool EmailCheck(string email)
        {
            if(!email.Contains("@"))
            {
                return false;
            }
            return _BloomFilter.Contains(email);
        }
        public void AddEmail(string email)
        {
            _BloomFilter.Add(email);
        }

        public void RebuildFilter(string email)
        {
          FilterMemorySerializerParam   
        }


    }
}
