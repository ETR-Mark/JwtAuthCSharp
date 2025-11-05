using JwtAuth.Models;

namespace JwtAuth.Interfaces
{
    public interface IUserRepository
    {
        Task<User> RegisterAsync(string username, string password);
        Task<User?> GetByUsernameAsync(string username);
    }
}
