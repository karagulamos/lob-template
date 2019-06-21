using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Business.Core.Common.Helpers;
using Business.Core.DTOs.Accounts;
using Business.Core.Entities.Identity;
using Business.Core.Services.Security;
using Business.Persistence.Repository;
using Business.Persistence.Repository.Common.Extensions;
using Microsoft.AspNet.Identity;

namespace Business.Services.Security
{
    public class UserManagerService : IUserManagerService<User>
    {
        private readonly UserManager<User, long> _userManager;

        private readonly BusinessDataContext _context;

        public UserManagerService(BusinessDataContext context)
        {
            _context = context;

            _userManager = new UserManager<User, long>(new BusinessUserStore(context))
            {
                PasswordHasher = new BusinessPasswordHasher()
            };
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _userManager.FindAsync(username, password);
            return user != null;
        }

        public async Task<IList<string>> GetUserRolesAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            return await _userManager.GetRolesAsync(user.Id);
        }

        public async Task<List<string>> GetRolesOrderedByUserIdAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var allRoles = _context.Roles.Select(r => r.Name).OrderBy(r => r);

            if (user == null)
                return await allRoles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user.Id);

            var otherRoles = await allRoles.Except(userRoles).ToListAsync();

            var roles = userRoles.ToList();

            roles.AddRange(otherRoles);

            return roles;
        }

        public Task<User> GetUserByEmailAsync(string username)
        {
            return _userManager.FindByEmailAsync(username);
        }

        public Task<User> GetUserByIdAsync(long userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public async Task<long> GetUserIdAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            return user.Id;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            return user != null;
        }

        public Task<bool> RoleExistsAsync(string role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var currentRole = role.ToLower();
            return _context.Roles.AnyAsync(r => r.Name.ToLower() == currentRole);
        }

        public async Task<bool> RolesExistAsync(List<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            foreach (var role in roles)
            {
                if (!await _context.Roles.AnyAsync(r => r.Name.ToLower() == role.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> AddUserToRoleAsync(int userId, string role)
        {
            if (!await RoleExistsAsync(role))
                return false;

            var result = await _userManager.AddToRoleAsync(userId, role);

            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(User user, string password, string role)
        {
            if (!string.IsNullOrEmpty(role) && !await RoleExistsAsync(role))
                return false;

            var result = await _userManager.CreateAsync(user, password);

            if (string.IsNullOrEmpty(role) || !result.Succeeded)
                return result.Succeeded;

            result = await _userManager.AddToRoleAsync(user.Id, role);

            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(User user, string password, List<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            var result = await _userManager.CreateAsync(user, password);

            if (roles.Count == 0 || !result.Succeeded)
                return result.Succeeded;

            result = await _userManager.AddToRolesAsync(user.Id, roles.ToArray());

            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task RemoveUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserAsync(User user, List<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            var currentRoles = await _userManager.GetRolesAsync(user.Id);

            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
            }

            await _userManager.AddToRolesAsync(user.Id, roles.ToArray());

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<LoggedOnUser> GetUserDetailsAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return new LoggedOnUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public Task<bool> ClientExistsAsync(string clientId, string clientSecret)
        {
            return _context.ApiClients.AnyAsync(c => c.ClientId == clientId && c.ClientSecret == clientSecret);
        }

        public Task<PagedList<UserDto>> GetUsersAsync(int page = 1, int size = 0)
        {
            return _context.Users.ProjectTo<UserDto>().ToPagedListAsync(page, size);
        }

        public async Task<bool> UpdateUserPasswordAsync(User user, string currentPassword, string newPassword)
        {
            if (!await _userManager.CheckPasswordAsync(user, currentPassword))
                return false;

            var result = await _userManager.ChangePasswordAsync(user.UserId, currentPassword, newPassword);

            return result.Succeeded;
        }

        public Task<bool> IsAccountActiveAsync(string username)
        {
            return _context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower() && u.IsActive);
        }
    }
}