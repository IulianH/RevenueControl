using System.Collections.Generic;

namespace RevenueControl.DomainObjects
{

    public enum ActionResponseCode
    {
        None = 0,
        Success,
        NoActionPerformed,
        InvalidInput,
        AlreadyExists,
        NotPermitted,
        DatabaseError,
        UnspecifiedError,
        NotFound
    }

    public class ActionResponse
    {
        public ActionResponseCode Status { get; set; }
        public string ActionResponseMessage { get; set; }
    }

    public class ParametrizedActionResponse<T> : ActionResponse
    {
        public T Result { get; set; }
    }

}