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
        NotFound
    }

    public class ActionResponse<T>
    {
        public ActionResponseCode Status { get; set; }
        public string ActionResponseMessage { get; set; }
        public T Result { get; set; }
        public IList<T> ResultList { get; set; }
    }
}