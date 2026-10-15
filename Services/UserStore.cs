using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public sealed class UserStore
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId;

    public IReadOnlyCollection<User> GetAll() => _users.Values.ToArray();

    public User? GetById(int id) => _users.GetValueOrDefault(id);

    public User Create(UserWriteRequest request)
    {
        var id = Interlocked.Increment(ref _nextId);
        var user = ToUser(id, request);
        _users[id] = user;
        return user;
    }

    public User? Update(int id, UserWriteRequest request)
    {
        while (_users.TryGetValue(id, out var current))
        {
            var updated = ToUser(id, request);
            if (_users.TryUpdate(id, updated, current))
            {
                return updated;
            }
        }

        return null;
    }

    public bool Delete(int id) => _users.TryRemove(id, out _);

    private static User ToUser(int id, UserWriteRequest request) =>
        new(id, request.Name.Trim(), request.Email.Trim(), request.Department.Trim());
}
