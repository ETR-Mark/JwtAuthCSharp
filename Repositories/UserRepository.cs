using JwtAuth.Data;
using JwtAuth.Models;
using JwtAuth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            var user = new User
            {
                UserName = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}
