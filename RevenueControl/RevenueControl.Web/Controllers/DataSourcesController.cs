using System.Net;
using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.Web.Context;

namespace RevenueControl.Web.Controllers
{
    public class DataSourcesController : Controller
    {
        private readonly IRevenueControlContext Context = new RevenueControlContext();

        private readonly IDataSourceManager manager = new DataSourceManager(new UnitOfWork());

        // GET: DataSources
        public ActionResult Index()
        {
            return View(manager.Get(Context.LoggedInClient));
        }

        // GET: DataSources/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankAccount,Name,Culture")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                dataSource.ClientName = Context.LoggedInClient;
                manager.Insert(dataSource);
                return RedirectToAction("Index");
            }

            return View(dataSource);
        }

        // GET: DataSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var dataSource = manager.GetById(id.Value, Context.LoggedInClient);
            if (dataSource == null)
                return HttpNotFound();
            return View(dataSource);
        }

        // POST: DataSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccount,Name,Culture")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                dataSource.ClientName = Context.LoggedInClient;
                manager.Update(dataSource);
                return RedirectToAction("Index");
            }
            return View(dataSource);
        }

        // GET: DataSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var dataSource = manager.GetById(id.Value, Context.LoggedInClient);
            if (dataSource == null)
                return HttpNotFound();
            return View(dataSource);
        }

        // POST: DataSources/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var dataSource = manager.GetById(id, Context.LoggedInClient);
            manager.Delete(dataSource);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                manager.Dispose();
            base.Dispose(disposing);
        }
    }
}