using AuserData.Data;
using AuserData.Models;
using Microsoft.EntityFrameworkCore;

namespace AuserData.Features;

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