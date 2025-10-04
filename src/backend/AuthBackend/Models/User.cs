using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthBackend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
