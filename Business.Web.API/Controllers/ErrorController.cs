using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Business.Core.Entities;
using Business.Core.Persistence;

namespace Business.Web.API.Controllers
{
    [RoutePrefix("api/errors")]
    public class ErrorController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public ErrorController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Route("{code}")]
        [ResponseType(typeof(ErrorCode))]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var error = await _uow.ErrorCodes.GetErrorByCodeAsync(code);

            if (error == null)
            {
                return BadRequest("No record with the specified code exist.");
            }

            return Ok(error);
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<ErrorCode>))]
        public async Task<IHttpActionResult> GetErrors()
        {
            var errorCodes = await _uow.ErrorCodes.FindAsync(e => true);
            return Ok(errorCodes);
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Add(ErrorCode error)
        {
            var result = await _uow.ErrorCodes.GetErrorByCodeAsync(error.Code);

            if (result != null)
            {
                return BadRequest("Duplicate record exist.");
            }

            error.Code = error.Code.Trim().ToUpper();
            _uow.ErrorCodes.Add(error);

            await _uow.CompleteAsync();

            return Ok("Record successfully added to database.");
        }

        [HttpPut]
        [Route("{code}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Update(string code, ErrorCode error)
        {
            var result = await _uow.ErrorCodes.GetErrorByCodeAsync(code);

            if (result == null)
            {
                return BadRequest("No record with the specified code exist.");
            }

            result.Code = error.Code.Trim().ToUpper();
            result.Description = error.Description;
            result.DisplayMessage = error.DisplayMessage;

            await _uow.CompleteAsync();

            return Ok("Record successfully updated.");
        }
    }
}
