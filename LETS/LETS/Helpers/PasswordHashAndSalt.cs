using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace LETS.Helpers
{
    public class PasswordHashAndSalt
    {
        public string getHashedPassword(string plainPassword)
        {
            byte[] salt = new byte[128 / 8];

            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(salt);
            //}

            string hashedAndSaltedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: plainPassword, salt: salt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
            return hashedAndSaltedPassword;
        }
    }
}