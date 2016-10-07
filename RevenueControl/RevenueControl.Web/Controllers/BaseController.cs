using System.Web.Mvc;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.Resource;
using RevenueControl.Web.Context;

namespace RevenueControl.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IRevenueControlContext _context = new RevenueControlContext();

        private static bool IsWarning(ActionResponseCode responseCode)
        {
            return (responseCode == ActionResponseCode.AlreadyExists) ||
                   (responseCode == ActionResponseCode.CompletedWithWarnings) ||
                   (responseCode == ActionResponseCode.NoActionPerformed);
        }

        private static bool IsError(ActionResponseCode responseCode)
        {
            return (responseCode == ActionResponseCode.DatabaseError) ||
                   (responseCode == ActionResponseCode.InvalidInput) ||
                   (responseCode == ActionResponseCode.NotPermitted) ||
                   (responseCode == ActionResponseCode.UnspecifiedError);
        }

        protected bool HandleResponse(ActionResponse response)
        {
            if (response.Status == ActionResponseCode.Success)
                TempData["Success"] = response.ActionResponseMessage ?? Resources.GenericSuccess;
            else if (IsError(response.Status))
                TempData["Error "] = response.ActionResponseMessage ??
                                     (response.ErrorCode <= 0
                                         ? Resources.GenericError
                                         : string.Format(Resources.GenericErrorWithErrorCode, response.ErrorCode));

            return (response.Status == ActionResponseCode.Success) || IsWarning(response.Status);
        }

        protected DataSource GetDataSource(int dataSourceId)
        {
            return new DataSource
            {
                Id = dataSourceId,
                ClientName = _context.LoggedInClient
            };
        }
    }
}