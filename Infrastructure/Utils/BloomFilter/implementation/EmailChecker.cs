//using Amazon.S3.Model;
using Application.Abstractions.Repository.Base;
using BloomFilter;
using BloomFilter.Configurations;
using Core.Models.Entitiеs;
using Core.Models.Options;

//using infrastructure.Repository.Base;
using infrastructure.Utils.BloomFilter.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.BloomFilter.implementation
{
    public class EmailChecker : IEmailChecker
    {
        private int exElement { get; set; }
        private double errorRate { get; set; }

        private IBloomFilter _BloomFilter;
        public string _tempfilePath { get; set; }
        public string _filePath { get ; set; }
        public string bloomDirectory { get; set; }

        private readonly ILogger<EmailChecker> _logger;

        public EmailChecker(
            IOptions<BloomOptions> options,
            ILogger<EmailChecker> logger
            )
        {
            _tempfilePath = Path.Combine(options.Value.BloomFilterDirectory, options.Value.BloomTempFileName + ".tmp");
            _filePath = Path.Combine(options.Value.BloomFilterDirectory, options.Value.BloomDataFileName + ".dat");
            bloomDirectory = options.Value.BloomFilterDirectory;
            exElement = options.Value.expectedElements;
            errorRate = options.Value.errorRate;
            _logger = logger;
            FilterBuilder.Build(exElement, errorRate);
            LoadFilter();
        }
        /// <summary>
        /// Проверка email на существование в базе через bloom filter
        /// true - такой уже есть 
        /// false - такого нет
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// 
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
                IBaseRepository<UserEntity> repo, 
                CancellationToken ct = default)
        {
          
            var currentFilter = (Filter)_BloomFilter;

            IBloomFilter newFilter = FilterBuilder.Build(
                exElement,
                errorRate
            );

            var emails = repo.GetAllWithoutTracking().Select(c => c.email).AsAsyncEnumerable();
            await foreach (var email in emails)
            {
                newFilter.Add(email);
            }

            var newMemoryFilter = (FilterMemory)newFilter;
            
            byte[] serializedData = SerializeToBytes(newMemoryFilter);

            string finalPath = Path.Combine(bloomDirectory, _filePath);

            SaveAtomically(serializedData, bloomDirectory, finalPath);

            _BloomFilter = newFilter;

            
        }

        public void LoadFilter()
        {
            try
            {
                byte[]? data = File.ReadAllBytes(_filePath);

                if (data == null || data.Length == 0)
                {
                    _logger.LogWarning("Файл фильтра пуст или не существует. Загружается пустой фильтр.");
                    return;
                }


                try
                {
                    using (var ms = new MemoryStream(data))
                    using (var reader = new BinaryReader(ms))
                    {

                        long expectedElements = reader.ReadInt64();
                        double errorRate = reader.ReadDouble();
                        //HashMethod method = (HashMethod)reader.ReadInt32(); 
                        int bucketCount = reader.ReadInt32();


                        var bucketLengths = new int[bucketCount];
                        for (int i = 0; i < bucketCount; i++)
                        {
                            bucketLengths[i] = reader.ReadInt32(); 
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
                    _logger.LogInformation("Фильтр успешно загружен из файла.");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Критическая ошибка при десериализации фильтра. {ex.Message}");
                    return;
                }

            }
            catch(Exception ex)
            {
                
                return; 
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
