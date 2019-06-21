using System;

namespace Business.Core.Persistence
{
    public interface IAuditable
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        bool IsDeleted { get; set; }
    }
}