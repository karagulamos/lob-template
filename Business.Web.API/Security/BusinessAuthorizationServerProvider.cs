using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Business.Core.Entities.Identity;
using Business.Core.Services;
using Business.Core.Services.Security;
using Business.Persistence.Repository;
using Business.Services.Security;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Business.Web.API.Security
{
    public class BusinessAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserManagerService<User> _userManager;
        private readonly IServiceHelper _helper;

        public BusinessAuthorizationServerProvider() : this(new UserManagerService(new BusinessDataContext()), NinjectIoc.Get<IServiceHelper>())
        {
        }

        public BusinessAuthorizationServerProvider(IUserManagerService<User> userManager, IServiceHelper helper)
        {
            _userManager = userManager;
            _helper = helper;
        }

        private const string BusinessExclusionClientId = "BUSINESS-ADMIN";

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.TryGetFormCredentials(out var clientId, out var clientSecret);

            try
            {
                if (clientId != BusinessExclusionClientId && (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)))
                {
                    context.SetError("invalid_grant", "Non existent or invalid client.");
                    return;
                }

                if (clientId != BusinessExclusionClientId && !await _userManager.ClientExistsAsync(clientId, clientSecret))
                {
                    context.SetError("invalid_grant", "Non existent or invalid client.");
                    return;
                }

                context.Validated(clientId);

                await base.ValidateClientAuthentication(context);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", $"Internal server error validating client: {ex.Message}");
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var cacheKey = $"LOGIN-REQ-{context.UserName}";
                var attempt = _helper.GetOrUpdateCacheItem(cacheKey, () => new LoginAttempt { Count = 0 });

                var accountLockTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["AccountLockTimeout"]);
                var maxLoginAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLoginAttempts"]);

                var isLockOutActive = DateTime.Now - attempt.DateTime < TimeSpan.FromMinutes(accountLockTimeout);

                if (isLockOutActive && attempt.Count == maxLoginAttempts)
                {
                    context.SetError("invalid_grant", $"You have exceeded the maximum number of login attempts. Please try again in {accountLockTimeout} minutes.");
                    return;
                }

                if (isLockOutActive && attempt.Count < maxLoginAttempts)
                    ++attempt.Count;

                if (!await _userManager.ValidateUserCredentialsAsync(context.UserName, context.Password))
                {
                    context.SetError("invalid_grant", "The username or password is incorrect.");
                    return;
                }

                _helper.RemoveCachedItem(cacheKey);

                if (!await _userManager.IsAccountActiveAsync(context.UserName))
                {
                    context.SetError("invalid_grant", "Your account has been blocked. Please contact admin for assistance.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                var userId = await _userManager.GetUserIdAsync(context.UserName);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

                var userRoles = await _userManager.GetUserRolesAsync(context.UserName);

                foreach (var role in userRoles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                context.Validated(new AuthenticationTicket(identity, new AuthenticationProperties
                {
                    Dictionary = {
                        { "roles", userRoles.Count <= 1 ? userRoles.FirstOrDefault() : userRoles.Aggregate((previous, next) => $"{previous},{next}") },
                        { "user_id", userId.ToString() },
                        { "username", context.UserName }
                    }
                }));
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", $"Internal server error validating user: {ex.Message}");
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var pair in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(pair.Key, pair.Value);
            }

            return Task.FromResult(true);
        }

        private class LoginAttempt
        {
            public int Count { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now;
        }
    }
}