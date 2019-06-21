using System.Collections.Generic;

namespace Business.Core.Services
{
    public interface IServiceReponse<TReponse>
    {
        string Code { get; set; }
        string ShortDescription { get; set; }
        TReponse Object { get; set; }
        Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}