using RevenueControl.DataAccess;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.InquiryFileReaders.Csv;
using RevenueControl.Services;
using RevenueControl.Web.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RevenueControl.Web.Controllers
{
    public class TransactionUploadController : Controller
    {
        ITransactionManager transactionManager = new TransactionsManager(new UnitOfWork(), new GenericCsvReader());
        IRevenueControlContext Context = new RevenueControlContext();
        IDataSourceManager dataSourceManager = new DataSourceManager(new UnitOfWork());

        // GET: TransactionUpload
        public ActionResult Index([Bind(Prefix = "id")] int dataSourceId)
        {
            ViewData["Id"] = dataSourceId;
            return View();
        }

        [HttpPost]
        public ActionResult Upload([Bind(Prefix = "id")] int dataSourceId)
        {
            if (Request.Files.Count > 0)
            {
              
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/UploadedContent/"), fileName);
                    file.SaveAs(path);
                    DataSource dataSource = dataSourceManager.GetById(dataSourceId, Context.LoggedInClient);
                    ParametrizedActionResponse<int> response = transactionManager.Insert(dataSource, path);
                    if(response.Status == ActionResponseCode.Success)
                    {
                        TempData["Success"] = string.Format("{0} transactions added.", response.Result);
                        
                    }
                }
            }
            return RedirectToAction("Index", new { id = dataSourceId });
        }

        protected override void Dispose(bool disposing)
        {
            transactionManager.Dispose();
            dataSourceManager.Dispose();
            base.Dispose(disposing);
        }
    }
}