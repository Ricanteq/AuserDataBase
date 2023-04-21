using UserData.Models;

namespace UserData.Features;

public interface IUserService
{
    public Task<User> CreateUser(User user);
}