using JwtAuth.Models;

namespace JwtAuth.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterAsync(string username, string password);
        public Task<string?> LoginAsync(string username, string password);
        public Task<User?> GetByUsernameAsync(string username);
        public string GenerateJwtToken(User user);
    }
}