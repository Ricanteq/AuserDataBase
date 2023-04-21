/*using UserData.Models;

namespace UserData.Features;

public class UserService : IUserService
{
    private static readonly List<User> _userList = new();

    public async Task<User> CreateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName)) throw new ArgumentException("Firstname cannot be empty");
        if (user.LastName == null) throw new ArgumentException("Lastname cannot be empty");
        if (user.Email == null) throw new ArgumentException("Email cannot be empty");
        if (user.Password == null) throw new ArgumentException("Password cannot be empty");
        user.Id = _userList.Count + 1;
        var newUser = new User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = HashPassword(user.Password)
        };
        _userList.Add(newUser);
        return await Task.FromResult(newUser);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public List<User> GetAllUsers()
    {
        return _userList;
    }

    public User GetUserById(int id)
    {
        return _userList.FirstOrDefault(u => u.Id == id);
    }

    public async Task<bool> VerifyPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }

    public async Task UpdateUser(User user)
    {
        var existingUser = _userList.FirstOrDefault(u => u.Id == user.Id);
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.Password = HashPassword(user.Password);
    }

    public void DeleteUser(int id)
    {
        var user = _userList.FirstOrDefault(u => u.Id == id);
        _userList.Remove(user);
    }

    public async Task<User?> LoginUser(string email, string password)
    {
        var user = _userList.FirstOrDefault(u => u.Email == email);
        if (user == null) return null;

        var passwordMatches = await VerifyPassword(user, password);
        return passwordMatches ? user : null;
    }
}

*/

using Microsoft.EntityFrameworkCore;
using UserData.Models;
using UserData.Persistence.Data;

namespace UserData.Features;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName)) throw new ArgumentException("Firstname cannot be empty");
        if (user.LastName == null) throw new ArgumentException("Lastname cannot be empty");
        if (user.Email == null) throw new ArgumentException("Email cannot be empty");
        if (user.Password == null) throw new ArgumentException("Password cannot be empty");

        user.Password = HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public IQueryable<User> GetAllUsers()
    {
        return _context.Users;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> VerifyPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }

    public async Task UpdateUser(User user)
    {
        user.Password = HashPassword(user.Password);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null) return;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> LoginUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return null;

        var passwordMatches = await VerifyPassword(user, password);

        return passwordMatches ? user : null;
    }
}