using Application.Abstractions.Utils;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace infrastructure.Utils.PasswodHashService
{
    public class PasswordHashService : IPasswordHashService
    {


        public string PasswordHashing(string password)
        {
            var stopwatch = Stopwatch.StartNew();

            byte[] key = Encoding.UTF8.GetBytes(password);
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] userUuidBytes = BitConverter.GetBytes(password.Length);

            var argon2 = new Argon2i(key)
            {
                DegreeOfParallelism = 8,
                MemorySize = 19456,
                Iterations = 5,
                Salt = salt,
                AssociatedData = userUuidBytes,
            };


            var hashByte = argon2.GetBytes(128);

            stopwatch.Stop();

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hashByte);

        }

        public bool VerifyPassword(string password, string storedFullHash)
        {

            try
            {
                var parts = storedFullHash.Split(':');
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);


                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                var argon2 = new Argon2i(passwordBytes)
                {
                    Salt = salt,
                    DegreeOfParallelism = 8,
                    MemorySize = 19456,
                    Iterations = 5,
                    AssociatedData = BitConverter.GetBytes(password.Length)
                };
                byte[] newHash = argon2.GetBytes(storedHash.Length);
                return newHash.SequenceEqual(storedHash);
            }
            catch(Exception ex)
            {
                return false;
            }

            
          
        }

    }
}
