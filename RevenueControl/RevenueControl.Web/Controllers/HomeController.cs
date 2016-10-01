using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.Web.Context;

namespace RevenueControl.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataSourceManager _repo = new DataSourceManager(new UnitOfWork());
        private readonly IRevenueControlContext Context = new RevenueControlContext();

        public ActionResult Index(string searchTerm = null)
        {
            var model = _repo.Get(Context.LoggedInClient, searchTerm);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DataSources", model);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}