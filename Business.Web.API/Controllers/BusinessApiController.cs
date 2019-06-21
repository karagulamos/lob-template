using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using Business.Core.Common.Exceptions;
using Business.Core.Services;
using Business.Services;
using Business.Web.API.Helpers.Extensions;
using log4net;
using Microsoft.AspNet.Identity;

namespace Business.Web.API.Controllers
{
    public abstract class BusinessApiControllerBase : ApiController
    {
        private readonly string _controllerName;
        private string _databaseForeignKeyErrorMessage;

        protected string CurrentUserId => $"[{User.Identity.GetUserId() ?? "0"}]({(!string.IsNullOrEmpty(User.Identity.Name) ? User.Identity.Name : "Anonymous")})";

        protected ILog Logger;

        protected BusinessApiControllerBase(string controllerName)
        {
            _controllerName = controllerName;
            Logger = LogManager.GetLogger(controllerName);
        }

        protected async Task<IServiceReponse<T>> HandleApiOperationAsync<T>(
            Func<Task<ServiceResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
        {
            var apiResponse = new ServiceResponse<T> { Code = $"{(int)HttpStatusCode.OK}", ShortDescription = "SUCCESS" };

            var logger = LogManager.GetLogger($"{_controllerName} / {method} - {CurrentUserId}");

            logger.Info($">>>=============== ENTERS ({method}) ===============>>> ");

            try
            {
                if (!ModelState.IsValid)
                    throw new BusinessGenericException("There were errors in your input, please correct them and try again.", $"{(int)HttpStatusCode.BadRequest}");

                var methodResponse = await action.Invoke();

                apiResponse.Object = methodResponse.Object;
                apiResponse.ShortDescription = string.IsNullOrEmpty(methodResponse.ShortDescription)
                    ? apiResponse.ShortDescription
                    : methodResponse.ShortDescription;

            }
            catch (BusinessGenericException bgex)
            {
                logger.Warn($"L{lineNo} - {bgex.ErrorCode}: {bgex.Message}");

                apiResponse.ShortDescription = bgex.Message;
                apiResponse.Code = bgex.ErrorCode;

                if (!ModelState.IsValid)
                {
                    apiResponse.ValidationErrors = ModelState.ToDictionary(
                        m =>
                        {
                            var tokens = m.Key.Split('.');
                            return tokens.Length > 0 ? tokens[tokens.Length - 1] : tokens[0];
                        },
                        m => m.Value.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)
                    );
                }
            }
            catch (DbUpdateException duex) when (duex.IsDatabaseFkDeleteException(out _databaseForeignKeyErrorMessage))
            {
                logger.Warn($"L{lineNo} - DFK001: {_databaseForeignKeyErrorMessage}");

                apiResponse.ShortDescription = "You cannot delete this record because it's currently in use.";
                apiResponse.Code = "DFK001";
            }
            catch (DbEntityValidationException devex) // Shouldn't happen but is useful for catching & fixing DB validation errors
            {
                logger.Warn($"L{lineNo} - DBV001: {devex.Message}");

                apiResponse.ShortDescription = "A data validation error occurred. Please contact admin for assistance.";
                apiResponse.Code = "DBV001";
            }
            catch (Exception ex)
            {
                logger.Error($"L{lineNo} {ex}");

                apiResponse.ShortDescription = "Sorry, we are unable process your request. Please try again or contact support for assistance.";
                apiResponse.Code = $"{(int)HttpStatusCode.InternalServerError}";
            }

            logger.Info($"<<<=============== EXITS ({method}) ===============<<< ");

            return apiResponse;
        }
    }
}
