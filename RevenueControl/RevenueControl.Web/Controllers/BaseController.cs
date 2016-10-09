using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.Resource;
using RevenueControl.Web.Models;

namespace RevenueControl.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string Client
        {
            get
            {
                var userName = User.Identity.GetUserName();
                using (var context = new ApplicationDbContext())
                {
                    var query = from u in context.Users
                        join p in context.UserClientPermissions
                        on u.Id equals p.UserId
                        where u.UserName == userName
                        select p.ClientName;

                    return query.SingleOrDefault();
                }
            }
        }


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
                ClientName = Client
            };
        }
    }
}