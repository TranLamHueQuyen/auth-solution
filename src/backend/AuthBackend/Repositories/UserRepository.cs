using AuthBackend.Models;

namespace AuthBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<User?> GetByUsernameAsync(string username) =>
            Task.FromResult(_users.FirstOrDefault(u => u.Username == username));

        public Task<User?> GetByIdAsync(Guid id) =>
            Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            // với in-memory list thì không cần làm gì thêm
            return Task.CompletedTask;
        }
    }
}
