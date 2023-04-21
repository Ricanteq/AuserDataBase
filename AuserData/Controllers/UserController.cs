/*using Microsoft.AspNetCore.Mvc;
using UserData.Features;
using UserData.Models;
using UserData.Persistence.Data;

namespace AuserData.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContext _context;
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    public UserController(DataContext context)
    {
        _context = context;
    }


    [HttpPost]
    public ActionResult Create(User user)
    {
        return Ok(_userService.CreateUser(user));
    }


    [HttpGet]
    public ActionResult<List<User>> GetAllUsers()
    {
        return _userService.GetAllUsers();
    }


    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null) return NotFound();

        return user;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, User updatedUser)
    {
        var userToUpdate = _userService.GetUserById(id);
        if (userToUpdate == null) return NotFound();

        userToUpdate.FirstName = updatedUser.FirstName ?? userToUpdate.FirstName;
        userToUpdate.LastName = updatedUser.LastName;
        userToUpdate.Email = updatedUser.Email;
        userToUpdate.Password = updatedUser.Password;

        return Ok(userToUpdate);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        _userService.DeleteUser(id);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(LoginRequest request)
    {
        var user = await _userService.LoginUser(request.Email, request.Password);
        if (user == null) return Unauthorized();

        return user;
    }
}
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserData.Models;
using UserData.Persistence.Data;

namespace AuserData.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContext _context;

    public UserController(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName))
            throw new ArgumentException("Firstname cannot be empty");
        if (string.IsNullOrWhiteSpace(user.LastName))
            throw new ArgumentException("Lastname cannot be empty");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentException("Password cannot be empty");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return user;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, User updatedUser)
    {
        var userToUpdate = await _context.Users.FindAsync(id);
        if (userToUpdate == null)
            return NotFound();

        userToUpdate.FirstName = updatedUser.FirstName ?? userToUpdate.FirstName;
        userToUpdate.LastName = updatedUser.LastName ?? userToUpdate.LastName;
        userToUpdate.Email = updatedUser.Email ?? userToUpdate.Email;

        if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

        await _context.SaveChangesAsync();

        return Ok(userToUpdate);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var userToDelete = await _context.Users.FindAsync(id);
        if (userToDelete == null)
            return NotFound();

        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized();

        var passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!passwordMatches)
            return Unauthorized();

        return user;
    }
}