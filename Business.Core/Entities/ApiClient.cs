using System;
using Business.Core.Persistence;

namespace Business.Core.Entities
{
    public class ApiClient : IAuditable
    {
        public int ApiClientId { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}