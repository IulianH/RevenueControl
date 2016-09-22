using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.Web.Context;

namespace RevenueControl.Web.Controllers
{
    public class HomeController : Controller
    {

        IDataSourceManager _repo = new DataSourceManager(new UnitOfWork());
        IRevenueControlContext Context = new RevenueControlContext();

        public ActionResult Index(string searchTerm = null)
        {
            var model = _repo.Get(Context.LoggedInClient, searchTerm);

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