using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class UserRolesController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/UserDocumentAccessRoles
        public ActionResult Index()
        {
            var userDocumentAccessRoles = db.UserDocumentAccessRoles.Include(u => u.DocumentAccessRole).Include(u => u.Employee);
            return View(userDocumentAccessRoles.ToList());
        }

        // GET: DMS/UserDocumentAccessRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            UserDocumentAccessRole userDocumentAccessRole = db.UserDocumentAccessRoles.Find(id);
            if (userDocumentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(userDocumentAccessRole);
        }

        // GET: DMS/UserDocumentAccessRoles/Create
        public ActionResult Create()
        {
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name");
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Name");
            return View();
        }

        // POST: DMS/UserDocumentAccessRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserDocumentRoleId,DocumentAccessRoleId,EmployeeId")] UserDocumentAccessRole userDocumentAccessRole)
        {
            if (ModelState.IsValid)
            {
                db.UserDocumentAccessRoles.Add(userDocumentAccessRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", userDocumentAccessRole.DocumentAccessRoleId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Name", userDocumentAccessRole.EmployeeId);
            return View(userDocumentAccessRole);
        }

        // GET: DMS/UserDocumentAccessRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            UserDocumentAccessRole userDocumentAccessRole = db.UserDocumentAccessRoles.Find(id);
            if (userDocumentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", userDocumentAccessRole.DocumentAccessRoleId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Name", userDocumentAccessRole.EmployeeId);
            return View(userDocumentAccessRole);
        }

        // POST: DMS/UserDocumentAccessRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserDocumentRoleId,DocumentAccessRoleId,EmployeeId")] UserDocumentAccessRole userDocumentAccessRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userDocumentAccessRole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", userDocumentAccessRole.DocumentAccessRoleId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Name", userDocumentAccessRole.EmployeeId);
            return View(userDocumentAccessRole);
        }

        // GET: DMS/UserDocumentAccessRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            UserDocumentAccessRole userDocumentAccessRole = db.UserDocumentAccessRoles.Find(id);
            if (userDocumentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(userDocumentAccessRole);
        }

        // POST: DMS/UserDocumentAccessRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserDocumentAccessRole userDocumentAccessRole = db.UserDocumentAccessRoles.Find(id);
            db.UserDocumentAccessRoles.Remove(userDocumentAccessRole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
