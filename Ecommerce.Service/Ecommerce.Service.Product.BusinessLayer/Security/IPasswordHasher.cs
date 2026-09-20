namespace Ecommerce.Service.Product.BusinessLayer.Security
{
    public interface IPasswordHasher
    {
        void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);
        bool VerifyPasswordHash(string password, string storedHash, string storedSalt);
    }
}
