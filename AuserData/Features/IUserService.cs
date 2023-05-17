using AuserData.Models;

namespace AuserData.Features;

public interface IUserService
{
    public Task<User> CreateUser(User user);
}