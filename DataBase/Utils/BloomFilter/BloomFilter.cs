//using Amazon.S3.Model;
using BloomFilter;
using BloomFilter.Configurations;
using Core.Interfaces.Repository;
using Core.Model.Options;
using infrastructure.Entitiеs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.BloomFilter
{
    public class EmailChecker
    {
        const int exElement = 100_000_000;
        
        const float errorRate = 0.001f;

        private  IBloomFilter _BloomFilter =  FilterBuilder.Build(exElement, errorRate);
        public string _tempfilePath { get; set; }
        public string _filePath { get ; set; }
        public string bloomDirectory { get; set; }

        /// <summary>
        /// Проверка email на существование в базе через bloom filter
        /// true - такой уже есть 
        /// false - такого нет
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// 

        public EmailChecker(BloomOptions options)
        {
            _tempfilePath = Path.Combine(options.BloomFilterDirectory, options.BloomTempFileName);
            _filePath = Path.Combine(options.BloomFilterDirectory, options.BloomDataFileName);
            bloomDirectory = options.BloomFilterDirectory;
        }

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

        public async Task RebuildFilter(
     IBaseRepository<UserEntities> repository)
        {
          
            var currentFilter = (Filter)_BloomFilter;

            IBloomFilter newFilter = FilterBuilder.Build(
                currentFilter.Capacity,
                currentFilter.Hashes
            );

            var emails = await repository
                .GetAllWithoutTracking()
                .Select(c => c.email)
                .ToListAsync();

            foreach (var mail in emails)
            {
                newFilter.Add(mail);
            }

            var newMemoryFilter = (FilterMemory)newFilter;
            
            byte[] serializedData = SerializeToBytes(newMemoryFilter);

            string finalPath = Path.Combine(bloomDirectory, "bloomFilter.dat");

            SaveAtomically(serializedData, bloomDirectory, finalPath);

            _BloomFilter = newFilter;

            
        }

        public void LoadFilter()
        {
            byte[]? data = File.ReadAllBytes(_filePath);

            if (data == null || data.Length == 0)
            {
                return ; // Нет файла или он пуст
            }

            try
            {
                using (var ms = new MemoryStream(data))
                using (var reader = new BinaryReader(ms))
                {
                    
                    long expectedElements = reader.ReadInt64();
                    double errorRate = reader.ReadDouble();
                    //HashMethod method = (HashMethod)reader.ReadInt32(); // Восстанавливаем HashMethod
                    int bucketCount = reader.ReadInt32();

                    
                    var bucketLengths = new int[bucketCount];
                    for (int i = 0; i < bucketCount; i++)
                    {
                        bucketLengths[i] = reader.ReadInt32(); // int: Длина текущего byte[]
                    }

                    
                    var restoredBuckets = new List<byte[]>(bucketCount);
                    for (int i = 0; i < bucketCount; i++)
                    {
                        int length = bucketLengths[i];
                        byte[] bucket = reader.ReadBytes(length);
                        restoredBuckets.Add(bucket);
                    }

                    var options = new FilterMemoryOptions
                    {
                        ExpectedElements = expectedElements,
                        ErrorRate = errorRate,
                        //Method = method,

                        
                        BucketBytes = restoredBuckets.ToArray()
                    };

                    
                    FilterBuilder.Build(options);
                }
            }
            catch (Exception ex)
            {
                // Ошибка десериализации (например, файл поврежден, или формат изменился)
                Console.WriteLine($"Критическая ошибка при десериализации фильтра. {ex.Message}");
                // Вернуть null, чтобы конструктор создал пустой фильтр
                return ;
            }
        }

        


        private void SaveAtomically(byte[] data, string directory, string finalPath)
        {
            string tempPath = Path.Combine(directory, "bloomTemp.tmp");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(tempPath, data);
            
            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }
            File.Move(tempPath, finalPath);
        }

        private byte[] SerializeToBytes(FilterMemory newMemoryFilter)
        {
            var bucketBytes = newMemoryFilter.ExportToBytes(); // IList<byte[]>
            var abstractFilter = (Filter)newMemoryFilter;

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(abstractFilter.ExpectedElements); // long
                writer.Write(abstractFilter.ErrorRate);       // double

                writer.Write(bucketBytes.Count);

                foreach (var bucket in bucketBytes)
                {
                    writer.Write(bucket.Length);
                }

                foreach (var bucket in bucketBytes)
                {
                    writer.Write(bucket);
                }
                return ms.ToArray();
            }
        }
    }
}
