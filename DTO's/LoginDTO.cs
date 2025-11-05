using System.ComponentModel.DataAnnotations;

namespace JwtAuth.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(50, ErrorMessage = "User name must be less than or equal to 50 characters.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(100, ErrorMessage = "Password must be less than or equal to 100 characters.")]
        public string Password { get; set; } = null!;
    }
}