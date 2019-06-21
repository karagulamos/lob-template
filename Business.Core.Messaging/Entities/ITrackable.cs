using System;

namespace Business.Core.Messaging.Entities
{
    public interface ITrackable
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}