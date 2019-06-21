using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Business.Core.Common.Constants;
using Business.Core.Common.Enums;
using Business.Core.Common.Helpers;
using Business.Core.Configuration;
using Business.Core.DTOs.Accounts;
using Business.Core.Entities.Identity;
using Business.Core.Messaging;
using Business.Core.Services;
using Business.Core.Services.Security;
using Business.Services;
using Microsoft.AspNet.Identity;

namespace Business.Web.API.Controllers
{
    [Authorize(Roles = nameof(UserType.Administrator))]
    [RoutePrefix("api/accounts")]
    public class AccountController : BusinessApiControllerBase
    {
        private readonly IUserManagerService<User> _userManager;
        private readonly IServiceHelper _helper;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IMessagingFactory _messagingFactory;

        public AccountController(IUserManagerService<User> userManager, IServiceHelper helper, IPasswordGenerator passwordGenerator, IMessagingFactory messagingFactory) : base(nameof(AccountController))
        {
            _userManager = userManager;
            _helper = helper;
            _passwordGenerator = passwordGenerator;
            _messagingFactory = messagingFactory;
        }

        [HttpGet]
        [Route("")]
        public async Task<IServiceReponse<PagedList<UserDto>>> Get(int page = 1, int size = 0, string searchTerm = "", UserType userType = UserType.Administrator)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var users = await _userManager.GetUsersAsync(page, size);

                return new ServiceResponse<PagedList<UserDto>>
                {
                    Object = users.QueryBuilder
                                  .Search(user => user.UserType == userType || userType == UserType.Administrator)
                                  .Search(searchTerm)
                                  .Execute()
                };
            });
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IServiceReponse<UserDto>> Get(long userId)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var user = await _userManager.GetUserByIdAsync(userId);
                return new ServiceResponse<UserDto>(user.MapTo<UserDto>());
            });
        }

        [HttpPost]
        [Route("register")]
        [Route("register/{userType}")]
        public async Task<IServiceReponse<bool>> Register(UserDto registration, UserType userType = UserType.Employee)
        {
            return await HandleApiOperationAsync(async () =>
            {
                if (await _userManager.UserExistsAsync(registration.Email))
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountExists);
                }

                if (!await _userManager.RolesExistAsync(registration.Roles ?? new List<string>()))
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.RoleNotExist);
                }

                var user = registration.MapTo<User>();

                user.UserType = userType;
                user.IsActive = true;

                var password = _passwordGenerator.Generate();

                if (!await _userManager.CreateUserAsync(user, password, registration.Roles ?? new List<string>()))
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountRegistrationFailed);
                }

                await _messagingFactory.GetEmailManager().SendAsync(
                    "Business Mobility",
                    user.Email,
                    "Welcome to Business!",
                    $"<div> username: {user.Email } </div> <div> password: {password} </div>",
                    true
                );

                return new ServiceResponse<bool>(true);
            });
        }
        
        [HttpPut]
        [Route("update/{userId}")]
        public async Task<IServiceReponse<bool>> Update(long userId, UserDto registration)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                if (user == null)
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountNotExist);
                }

                if (!string.Equals(user.Email, registration.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (await _userManager.UserExistsAsync(registration.Email.ToLower()))
                        throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountExists);
                }

                if (!await _userManager.RolesExistAsync(registration.Roles ?? new List<string>()))
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.RoleNotExist);
                }

                user.FirstName = registration.FirstName;
                user.LastName = registration.LastName;
                user.PhoneNumber = registration.PhoneNumber;

                if (!await _userManager.UpdateUserAsync(user, registration.Roles ?? new List<string>()))
                {
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountRegistrationFailed);
                }

                return new ServiceResponse<bool>(true);
            });
        }
        
        [HttpDelete]
        [Route("{userId}")]
        public async Task<IServiceReponse<bool>> Delete(long userId)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                if (user == null)
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountNotExist);

                await _userManager.RemoveUserAsync(user);

                return new ServiceResponse<bool>(true);
            });
        }

        [HttpGet]
        [Route("toggle/{userId}")]
        public async Task<IServiceReponse<bool>> Toggle(long userId)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                if (user == null)
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountNotExist);

                user.IsActive = !user.IsActive;

                var updated = await _userManager.UpdateUserAsync(user);

                return new ServiceResponse<bool>(updated);
            });
        }

        [HttpPost, Route("update/password")]
        public async Task<IServiceReponse<bool>> UpdatePassword(string currentPassword, string newPassword)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var user = await _userManager.GetUserByIdAsync(User.Identity.GetUserId<long>());

                if (user == null)
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountNotExist);

                if (!await _userManager.UpdateUserPasswordAsync(user, currentPassword, newPassword))
                    throw await _helper.GetExceptionAsync(ErrorConstants.UserAccountPasswordInvalid);

                return new ServiceResponse<bool>(true);
            });
        }

        [HttpGet]
        [Route("roles/{userId}")]
        public Task<List<string>> GetAllRolesOrderedByUserRoles(long userId)
        {
            return _userManager.GetRolesOrderedByUserIdAsync(userId);
        }
    }
}
