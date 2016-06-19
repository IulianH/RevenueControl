using System.Collections.Generic;

namespace RevenueControl.DomainObjects
{
    public enum ActionResponseCode
    {
        Success,
        OkWithWarning,
        InvalidInput,
        AlreadyExists
    }

    public class ActionResponse<T>
    {
        public ActionResponseCode Status { get; set; }
        public string ActionResponseMessage { get; set; }
        public T Result { get; set; }
        public ICollection<T> ResultList { get; set; }
    }
}