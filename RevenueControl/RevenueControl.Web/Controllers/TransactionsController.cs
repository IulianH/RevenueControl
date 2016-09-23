using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.Web.Context;
using RevenueControl.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RevenueControl.Web.Controllers
{
    public class TransactionsController : Controller
    {
        IDataSourceManager dataSourceManager = new DataSourceManager(new UnitOfWork());
        ITransactionManager transactionManager = new TransactionsManager(new UnitOfWork());
        private IRevenueControlContext Context = new RevenueControlContext();

        // GET: Transactions
        public ActionResult Index([Bind(Prefix = "id")] int dataSourceId)
        {
            DataSource dataSource = dataSourceManager.GetById(dataSourceId, Context.LoggedInClient);
            IEnumerable<Transaction> transactions = transactionManager.Get(dataSource);

            var model = new TransactionsViewModel
            {
                DataSource = string.IsNullOrWhiteSpace(dataSource.ClientName) ? dataSource.BankAccount : dataSource.ClientName + " - " + dataSource.BankAccount,
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
