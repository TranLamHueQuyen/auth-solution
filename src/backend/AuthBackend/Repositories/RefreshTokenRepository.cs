using AuthBackend.Data;
using AuthBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AuthBackend.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByHashAsync(string hash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == hash && !rt.IsRevoked);
        }

        public async Task<RefreshToken?> GetRevokedByHashAsync(string hash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == hash && rt.IsRevoked);
        }

        public async Task RevokeAsync(Guid tokenId, string reason, string? replacedByHash = null)
        {
            var token = await _context.RefreshTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedReason = reason;
                token.ReplacedByTokenHash = replacedByHash;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllForUserAsync(Guid userId, string reason)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.IsRevoked = true;
                t.RevokedReason = reason;
            }

            await _context.SaveChangesAsync();
        }
    }
}