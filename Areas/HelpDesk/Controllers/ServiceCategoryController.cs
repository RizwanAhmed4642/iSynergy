using iSynergy.Areas.HelpDesk.Models;
using iSynergy.Areas.HR.Models;
using iSynergy.Areas.Procurement.Models;
using iSynergy.DataContexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSynergy.Areas.HelpDesk.Controllers
{
    public class ServiceCategoryController : Controller
    {
        private FullDb db = new FullDb();

        // GET:  ServiceCategory
        public ActionResult Index()
        {
           
           
            return View(db.ServiceCategories.ToList());
        }

        // GET:  ServiceCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            ServiceCategory _serviceCategory = db.ServiceCategories.Find(id);
            if (_serviceCategory == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(_serviceCategory);
        }

        // GET:  ServiceCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST:  ServiceCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                db.ServiceCategories.Add(serviceCategory);
                db.SaveChanges();
                TempData["success"] = "Service Category created successfully!";
                return RedirectToAction("Index");
            }

            return View(serviceCategory);
        }

        // GET:  ServiceCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            ServiceCategory ServiceCategory = db.ServiceCategories.Find(id);
            if (ServiceCategory == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
      
            return View(ServiceCategory);
        }

        // POST:  ServiceCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceCategory);
        }

        //// GET: Departments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", new { area = "" });
        //    }
        //    Department department = db.Departments.Find(id);
        //    if (department == null)
        //    {
        //        return RedirectToAction("PageNotFound", "Error", new { area = "" });
        //    }
        //    return View(department);
        //}

        //// POST: Departments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Department department = db.Departments.Find(id);
        //    db.Departments.Remove(department);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}