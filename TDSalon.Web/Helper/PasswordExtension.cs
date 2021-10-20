using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TDSalon.Web.Helper
{
    public static class PasswordExtension
    {
        public static byte[] GenerateSalt(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string pass = Convert.ToBase64String(salt);

            return salt;
        }
        public static string GenerateHash(byte[] salt, string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;
        }
        public static string GetHashedPassword(string password,string salt)
        {
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: System.Convert.FromBase64String(salt),///Encoding.ASCII.GetBytes(dbPasswordSalt),
               prf: KeyDerivationPrf.HMACSHA1,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));
            return hashedPassword;
        }
    }
}
