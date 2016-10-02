using System.Collections.Generic;
using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.Web.Context;
using RevenueControl.Web.Models;

namespace RevenueControl.Web.Controllers
{
    [Authorize]
    public class TransactionsController : BaseController
    {
        private readonly IRevenueControlContext Context = new RevenueControlContext();
        private readonly IDataSourceManager dataSourceManager = new DataSourceManager(new UnitOfWork());
        private readonly ITransactionManager transactionManager = new TransactionsManager(new UnitOfWork());

        // GET: Transactions
        public ActionResult Index([Bind(Prefix = "id")] int dataSourceId)
        {
            var dataSource = dataSourceManager.GetById(dataSourceId, Context.LoggedInClient);
            IEnumerable<Transaction> transactions = transactionManager.Get(dataSource);

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
            transactionManager.Dispose();
            base.Dispose(disposing);
        }
    }
}