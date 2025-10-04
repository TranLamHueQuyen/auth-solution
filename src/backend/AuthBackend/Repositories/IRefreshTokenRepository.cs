using System;
using System.Threading.Tasks;
using AuthBackend.Models;

namespace AuthBackend.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<RefreshToken?> GetRevokedByHashAsync(string tokenHash);
        Task RevokeAsync(Guid id, string reason, string? replacedByTokenHash = null);
        Task RevokeAllForUserAsync(Guid userId, string reason);
    }
}
