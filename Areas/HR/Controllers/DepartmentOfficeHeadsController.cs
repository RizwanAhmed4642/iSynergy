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
    [AuthorizeRedirect(Roles = "Admin, Can Assign Regional HODs")]
    public class DepartmentOfficeHeadsController : CustomController
    {
        private CompanyDb db = new CompanyDb();

        // GET: DepartmentOfficeHeads
        public ActionResult Index()
        {
            var departmentOfficeHeads = db.DepartmentOfficeHeads.Include(d => d.Department).Include(d => d.Office);
            ViewBag.employeeIdtoNameDictonary = new EmployeeIdtoNameDictonary();
            foreach (var item in departmentOfficeHeads)
            {
                ViewBag.employeeIdtoNameDictonary.Add(item.EmployeeId,db);
            }
            return View(departmentOfficeHeads.ToList());
        }

        // GET: DepartmentOfficeHeads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DepartmentOfficeHead departmentOfficeHead = db.DepartmentOfficeHeads.Find(id);
            if (departmentOfficeHead == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(departmentOfficeHead);
        }

        // GET: DepartmentOfficeHeads/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location");
            ViewBag.EmployeeId = new SelectList(db.Employees,"EmployeeId","Name");
            return View();
        }

        // POST: DepartmentOfficeHeads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentOfficeHeadId,OfficeId,DepartmentId,EmployeeId")] DepartmentOfficeHead departmentOfficeHead)
        {
            if (ModelState.IsValid)
            {
                db.DepartmentOfficeHeads.Add(departmentOfficeHead);
                db.SaveChanges();
                TempData["success"] = "Regional HOD added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", departmentOfficeHead.DepartmentId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", departmentOfficeHead.OfficeId);
            return View(departmentOfficeHead);
        }

        // GET: DepartmentOfficeHeads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DepartmentOfficeHead departmentOfficeHead = db.DepartmentOfficeHeads.Find(id);
            if (departmentOfficeHead == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", departmentOfficeHead.DepartmentId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", departmentOfficeHead.OfficeId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Name", departmentOfficeHead.EmployeeId);
            return View(departmentOfficeHead);
        }

        // POST: DepartmentOfficeHeads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentOfficeHeadId,OfficeId,DepartmentId,EmployeeId")] DepartmentOfficeHead departmentOfficeHead)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentOfficeHead).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", departmentOfficeHead.DepartmentId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", departmentOfficeHead.OfficeId);
            return View(departmentOfficeHead);
        }

        // GET: DepartmentOfficeHeads/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", new { area = "" });
        //    }
        //    DepartmentOfficeHead departmentOfficeHead = db.DepartmentOfficeHeads.Find(id);
        //    if (departmentOfficeHead == null)
        //    {
        //        return RedirectToAction("PageNotFound", "Error", new { area = "" });
        //    }
        //    return View(departmentOfficeHead);
        //}

        //// POST: DepartmentOfficeHeads/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    DepartmentOfficeHead departmentOfficeHead = db.DepartmentOfficeHeads.Find(id);
        //    db.DepartmentOfficeHeads.Remove(departmentOfficeHead);
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