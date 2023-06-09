﻿using AuserData.Models;
using FluentValidation.Results;

namespace AuserData.Features;

public interface IUserService
{
    Task<ValidationResult> ValidateUserAsync(User user);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<bool> CheckUserExistsAsync(int id);
    Task UpdateUserAsync(int id, User updatedUser);
    Task DeleteUserAsync(int id);
    Task<User> LoginUserAsync(string email, string password);
}