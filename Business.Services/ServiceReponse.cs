using System.Collections.Generic;
using Business.Core.Services;

namespace Business.Services
{
    public class ServiceResponse<TResponse> : IServiceReponse<TResponse>
    {
        public ServiceResponse(TResponse response) : this()
        {
            Object = response;
        }

        public ServiceResponse()
        {
            ValidationErrors = new Dictionary<string, IEnumerable<string>>();
        }

        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public TResponse Object { get; set; }

        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}
