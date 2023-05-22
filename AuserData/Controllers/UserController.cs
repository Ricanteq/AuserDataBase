/*using AuserData.Data;
using AuserData.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuserData.Controllers
{
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
        public async Task<IActionResult> Create(User user)
        {
            UserValidator validator = new UserValidator();
            ValidationResult validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Check if the email is already used
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                return BadRequest("Email is already in use");
            }

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

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty().WithMessage("Firstname cannot be empty");
            RuleFor(u => u.LastName).NotEmpty().WithMessage("Lastname cannot be empty");
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}*/


using AuserData.Features;
using AuserData.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuserData.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        var validationResult = await _userService.ValidateUserAsync(user);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var emailExists = await _userService.CheckEmailExistsAsync(user.Email);
        if (emailExists) return BadRequest("Email is already in use");

        var createdUser = await _userService.CreateUserAsync(user);

        return Ok(createdUser);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        return user;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, User updatedUser)
    {
        var userExists = await _userService.CheckUserExistsAsync(id);
        if (!userExists) return NotFound();

        await _userService.UpdateUserAsync(id, updatedUser);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var userExists = await _userService.CheckUserExistsAsync(id);
        if (!userExists) return NotFound();

        await _userService.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(LoginRequest request)
    {
        var user = await _userService.LoginUserAsync(request.Email, request.Password);
        if (user == null) return Unauthorized();

        return user;
    }
}