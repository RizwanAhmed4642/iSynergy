using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Areas.Procurement.Models;
using iSynergy.Areas.HR.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.HR.Controllers
{
    [AuthorizeRedirect(Roles = "Admin, Can Edit Offices")]
    public class OfficesController : CustomController
    {
        private CompanyDb db = new CompanyDb();

        // GET: Offices
        public ActionResult Index()
        {
            ViewBag.employeeIdtoNameDictonary = new EmployeeIdtoNameDictonary();
            var offices = db.Offices;
            foreach (var office in offices)
            {
                ViewBag.employeeIdtoNameDictonary.Add(office.LocalFinanceHeadId, db);
                ViewBag.employeeIdtoNameDictonary.Add(office.LocalHrManagerId, db);
                ViewBag.employeeIdtoNameDictonary.Add(office.LocalOperationsHeadId, db);
                ViewBag.employeeIdtoNameDictonary.Add(office.LocalProcurementManagerId, db);
            }
            return View(db.Offices.ToList());
        }

        // GET: Offices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Office office = db.Offices.Find(id);
            if (office == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(office);
        }

        // GET: Offices/Create
        public ActionResult Create()
        {
            ViewBag.LocalOperationsHeadId = new SelectList(db.Employees, "EmployeeId", "Name");
            ViewBag.LocalFinanceHeadId = new SelectList(db.Employees, "EmployeeId", "Name");
            ViewBag.LocalProcurementManagerId = new SelectList(db.Employees, "EmployeeId", "Name");
            ViewBag.LocalHrManagerId = new SelectList(db.Employees, "EmployeeId", "Name");
            return View();
        }

        // POST: Offices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "OfficeId,Location,LocalOperationsHeadId,LocalFinanceHeadId,LocalProcurementManagerId,LocalHrManagerId")] Office office)
        {
            if (ModelState.IsValid)
            {
                db.Offices.Add(office);
                db.SaveChanges();
                TempData["success"] = "Office added successfully!";
                return RedirectToAction("Index");
            }

            return View(office);
        }

        // GET: Offices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Office office = db.Offices.Find(id);
            if (office == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.LocalOperationsHeadId = new SelectList(db.Employees, "EmployeeId", "Name", office.LocalOperationsHeadId);
            ViewBag.LocalFinanceHeadId = new SelectList(db.Employees, "EmployeeId", "Name", office.LocalFinanceHeadId);
            ViewBag.LocalProcurementManagerId = new SelectList(db.Employees, "EmployeeId", "Name", office.LocalProcurementManagerId);
            ViewBag.LocalHrManagerId = new SelectList(db.Employees, "EmployeeId", "Name", office.LocalHrManagerId);
            return View(office);
        }

        // POST: Offices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OfficeId,Location,LocalOperationsHeadId,LocalFinanceHeadId,LocalProcurementManagerId,LocalHrManagerId")] Office office)
        {
            if (ModelState.IsValid)
            {
                db.Entry(office).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(office);
        }

        // GET: Offices/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", new { area = "" });
        //    }
        //    Office office = db.Offices.Find(id);
        //    if (office == null)
        //    {
        //        return RedirectToAction("PageNotFound", "Error", new { area = "" });
        //    }
        //    return View(office);
        //}

        // POST: Offices/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Office office = db.Offices.Find(id);
        //    db.Offices.Remove(office);
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
