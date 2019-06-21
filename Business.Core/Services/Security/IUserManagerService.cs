using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Core.Common.Helpers;
using Business.Core.DTOs.Accounts;
using Business.Core.Entities.Identity;

namespace Business.Core.Services.Security
{
    public interface IUserManagerService<in TUser> : IServiceDependencyMarker
    {
        Task<bool> ValidateUserCredentialsAsync(string username, string password);

        Task<IList<string>> GetUserRolesAsync(string username);

        Task<long> GetUserIdAsync(string username);

        Task<PagedList<UserDto>> GetUsersAsync(int page = 1, int size = 0);

        Task<bool> UserExistsAsync(string username);

        Task<bool> AddUserToRoleAsync(int userId, string role);
        Task<bool> RoleExistsAsync(string role);
        Task<bool> RolesExistAsync(List<string> roles);

        Task<bool> CreateUserAsync(TUser user, string password, string role = "");
        Task<bool> CreateUserAsync(User user, string password, List<string> roles);

        Task<LoggedOnUser> GetUserDetailsAsync(long userId);
        Task<User> GetUserByEmailAsync(string username);
        Task<User> GetUserByIdAsync(long userId);

        Task<bool> ClientExistsAsync(string clientId, string clientSecret);

        Task<bool> UpdateUserAsync(User user, List<string> roles);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateUserPasswordAsync(User user, string currentPassword, string newPassword);

        Task RemoveUserAsync(User user);

        Task<List<string>> GetRolesOrderedByUserIdAsync(long userId);

        Task<bool> IsAccountActiveAsync(string username);
    }
}