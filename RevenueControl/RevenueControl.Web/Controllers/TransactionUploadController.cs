using System.IO;
using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.InquiryFileReaders.Csv;
using RevenueControl.Services;
using RevenueControl.Web.Context;

namespace RevenueControl.Web.Controllers
{
    public class TransactionUploadController : BaseController
    {
        private readonly IRevenueControlContext Context = new RevenueControlContext();
        private readonly IDataSourceManager dataSourceManager = new DataSourceManager(new UnitOfWork());

        private readonly ITransactionManager transactionManager = new TransactionsManager(new UnitOfWork(),
            new GenericCsvReader());

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

                if ((file != null) && (file.ContentLength > 0))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/UploadedContent/"), fileName);
                    file.SaveAs(path);
                    var dataSource = dataSourceManager.GetById(dataSourceId, Context.LoggedInClient);
                    var response = transactionManager.Insert(dataSource, path);
                    HandleResponse(response);
                }
            }
            return RedirectToAction("Index", new {id = dataSourceId});
        }

        protected override void Dispose(bool disposing)
        {
            transactionManager.Dispose();
            dataSourceManager.Dispose();
            base.Dispose(disposing);
        }
    }
}