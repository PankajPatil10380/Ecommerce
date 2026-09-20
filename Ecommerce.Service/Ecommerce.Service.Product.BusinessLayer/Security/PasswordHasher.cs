using System;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Service.Product.BusinessLayer.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

            using var hmac = new HMACSHA512();
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
                return false;

            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] storedHashBytes = Convert.FromBase64String(storedHash);

                using var hmac = new HMACSHA512(saltBytes);
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
            }
            catch
            {
                return false;
            }
        }
    }
}
