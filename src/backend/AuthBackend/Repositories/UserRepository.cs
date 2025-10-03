public class UserRepository : IUserRepository {
    private readonly List<User> _users = new();

    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Username == username));

    public Task<User?> GetByIdAsync(Guid id) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task AddAsync(User user) {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user) {
        // vì in-memory nên không cần code update phức tạp
        return Task.CompletedTask;
    }
}
