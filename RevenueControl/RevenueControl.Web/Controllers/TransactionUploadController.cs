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
        private readonly IDataSourceManager _dataSourceManager = new DataSourceManager(new UnitOfWork());
        private readonly ITransactionManager _transactionManager = new TransactionsManager(new UnitOfWork(),
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
                    var dataSource = _dataSourceManager.GetById(dataSourceId, _context.LoggedInClient);
                    var response = _transactionManager.Insert(dataSource, path);
                    HandleResponse(response);
                }
            }
            return RedirectToAction("Index", new {id = dataSourceId});
        }

        protected override void Dispose(bool disposing)
        {
            _transactionManager.Dispose();
            _dataSourceManager.Dispose();
            base.Dispose(disposing);
        }
    }
}