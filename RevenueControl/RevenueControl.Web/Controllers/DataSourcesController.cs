using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;

namespace RevenueControl.Web.Controllers
{
    public class DataSourcesController : Controller
    {
       
        private IDataSourceManager manager = new DataSourceManager(new UnitOfWork());
        private Client Client
        {
            get
            {
                return new Client {Name = "DefaultClient" };
            }
        }


        // GET: DataSources
        public ActionResult Index()
        {
            return View(manager.Get(Client));
        }

        // GET: DataSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = manager.GetById(id.Value);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
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
        public ActionResult Create([Bind(Include = "Id,BankAccount,Name,Culture,ClientName")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                manager.Insert(dataSource);
                return RedirectToAction("Index");
            }

            return View(dataSource);
        }

        // GET: DataSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = manager.GetById(id.Value);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // POST: DataSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccount,Name,Culture,ClientName")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                manager.Update(dataSource);
                return RedirectToAction("Index");
            }
            return View(dataSource);
        }

        // GET: DataSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = manager.GetById(id.Value);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // POST: DataSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSource dataSource = manager.GetById(id);
            manager.Delete(dataSource); 
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
