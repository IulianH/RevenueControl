using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;

namespace RevenueControl.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDataSourceManager _repo = new DataSourceManager(new UnitOfWork());


        public ActionResult Index(string searchTerm = null)
        {
            var model = _repo.Get(Client, searchTerm);

            if (Request.IsAjaxRequest())
                return PartialView("_DataSources", model);

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