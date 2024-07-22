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
    [AuthorizeRedirect(Roles = "Admin, Can Edit Departments")]
    public class DepartmentsController : CustomController
    {
        private CompanyDb db = new CompanyDb();

        // GET: Departments
        public ActionResult Index()
        {
            ViewBag.employeeIdtoNameDictonary = new EmployeeIdtoNameDictonary();
            var departments = db.Departments;
            foreach (var department in departments)
            {
                ViewBag.employeeIdtoNameDictonary.Add(department.HodId, db);
            }
            return View(db.Departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            ViewBag.employees = db.Employees;
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,Name,HodId")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                TempData["success"] = "Department created successfully!";
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.employees = db.Employees;
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,Name,HodId")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
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
