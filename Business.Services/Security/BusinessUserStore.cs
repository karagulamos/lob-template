using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Business.Core.Entities.Identity;
using Business.Persistence.Repository;
using Microsoft.AspNet.Identity;

namespace Business.Services.Security
{
    internal class BusinessUserStore : IUserEmailStore<User, long>, IUserPasswordStore<User, long>, IUserRoleStore<User, long>
    {
        private readonly BusinessDataContext _context;

        public BusinessUserStore(BusinessDataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public Task<User> FindByIdAsync(long userId)
        {
            return _context.Users.FindAsync(userId);
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return _context.Users.FirstOrDefaultAsync(apu => apu.UserName == userName);
        }

        public async Task UpdateAsync(User user)
        {
            var entry = _context.Entry(user);

            if (entry.State == EntityState.Detached)
                _context.Users.Attach(user);

            entry.State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult(true);
        }

        public async Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Value cannot be empty", nameof(roleName));

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
                throw new InvalidOperationException("Role does not exist");

            role.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Value cannot be empty", nameof(roleName));

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
                throw new InvalidOperationException("Role does not exist");

            var entry = _context.Entry(user);

            if (entry.State == EntityState.Detached)
                _context.Users.Attach(user);

            role.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var entry = _context.Entry(user);

            if (entry.State == EntityState.Detached)
                _context.Users.Attach(user);

            return await Task.FromResult(user.Roles.Select(r => r.Name).ToList());
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Value cannot be empty", nameof(roleName));

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
                throw new InvalidOperationException("Role does not exist");

            var entry = _context.Entry(user);

            if (entry.State == EntityState.Detached)
                _context.Users.Attach(user);

            return role.Users.Any(u => u.Id == user.Id);
        }

        public Task SetEmailAsync(User user, string email)
        {
            user.Email = email;
            return Task.FromResult(true);
        }

        public Task<string> GetEmailAsync(User user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.Email));
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            return Task.FromResult(true);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}