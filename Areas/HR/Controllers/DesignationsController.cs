using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Areas.HR.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.HR.Controllers
{
    [AuthorizeRedirect(Roles = "Admin, Can Edit Designations")]
    public class DesignationsController : CustomController
    {
        private CompanyDb db = new CompanyDb();

        // GET: Designations
        public ActionResult Index()
        {
            return View(db.Designations.ToList());
        }

        // GET: Designations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(designation);
        }

        // GET: Designations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Designations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DesignationId,Title")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Designations.Add(designation);
                db.SaveChanges();
                TempData["success"] = "Designation created successfully!";
                return RedirectToAction("Index");
            }

            return View(designation);
        }

        // GET: Designations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(designation);
        }

        // POST: Designations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DesignationId,Title")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(designation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(designation);
        }

        //// GET: Designations/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", new { area = "" });
        //    }
        //    Designation designation = db.Designations.Find(id);
        //    if (designation == null)
        //    {
        //        return RedirectToAction("PageNotFound", "Error", new { area = "" });
        //    }
        //    return View(designation);
        //}

        //// POST: Designations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Designation designation = db.Designations.Find(id);
        //    db.Designations.Remove(designation);
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
