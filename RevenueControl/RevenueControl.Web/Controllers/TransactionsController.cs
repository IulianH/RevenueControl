using System.Collections.Generic;
using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.Web.Models;

namespace RevenueControl.Web.Controllers
{
    [Authorize]
    public class TransactionsController : BaseController
    {
        private readonly IDataSourceManager _dataSourceManager = new DataSourceManager(new UnitOfWork());
        private readonly ITransactionManager _transactionManager = new TransactionsManager(new UnitOfWork());

        // GET: Transactions
        public ActionResult Index([Bind(Prefix = "id")] int dataSourceId)
        {
            var dataSource = _dataSourceManager.GetById(dataSourceId, Client);
            IEnumerable<Transaction> transactions = _transactionManager.Get(dataSource);

            var model = new TransactionsViewModel
            {
                DataSource =
                    string.IsNullOrWhiteSpace(dataSource.ClientName)
                        ? dataSource.BankAccount
                        : dataSource.ClientName + " - " + dataSource.BankAccount,
                Transactions = transactions,
                DataSourceId = dataSource.Id
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            _transactionManager.Dispose();
            base.Dispose(disposing);
        }
    }
}