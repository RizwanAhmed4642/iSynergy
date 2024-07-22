using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class RolesController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/DocumentAccessRoles
        public ActionResult Index()
        {
            return View(db.DocumentAccessRoles.ToList());
        }

        // GET: DMS/DocumentAccessRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAccessRole documentAccessRole = db.DocumentAccessRoles.Find(id);
            if (documentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentAccessRole);
        }

        // GET: DMS/DocumentAccessRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DMS/DocumentAccessRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentAccessRoleId,Name")] DocumentAccessRole documentAccessRole)
        {
            if (ModelState.IsValid)
            {
                db.DocumentAccessRoles.Add(documentAccessRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documentAccessRole);
        }

        // GET: DMS/DocumentAccessRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAccessRole documentAccessRole = db.DocumentAccessRoles.Find(id);
            if (documentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentAccessRole);
        }

        // POST: DMS/DocumentAccessRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentAccessRoleId,Name")] DocumentAccessRole documentAccessRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentAccessRole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documentAccessRole);
        }

        // GET: DMS/DocumentAccessRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAccessRole documentAccessRole = db.DocumentAccessRoles.Find(id);
            if (documentAccessRole == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentAccessRole);
        }

        // POST: DMS/DocumentAccessRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentAccessRole documentAccessRole = db.DocumentAccessRoles.Find(id);
            db.DocumentAccessRoles.Remove(documentAccessRole);
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
