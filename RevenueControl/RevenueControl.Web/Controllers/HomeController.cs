using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.Web.Controllers
{
    public class HomeController : Controller
    {
        IDataSourceManager _repo = new DataSourceManager(new DataSourceRepository(), new ClientManger(new ClientRepository()).Client.Result);
        public ActionResult Index()
        {
            var model = _repo.GetDataSources().ResultList;

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