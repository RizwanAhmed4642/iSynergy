using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class CategoryRolesController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/CategoryRights
        public ActionResult Index()
        {
            var categoryRights = db.CategoryRights.Include(c => c.DocumentAccessRole).Include(c => c.DocumentCategory);
            return View(categoryRights.ToList());
        }

        // GET: DMS/CategoryRights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            CategoryRight categoryRight = db.CategoryRights.Find(id);
            if (categoryRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(categoryRight);
        }

        // GET: DMS/CategoryRights/Create
        public ActionResult Create()
        {
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name");
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name");
            return View();
        }

        // POST: DMS/CategoryRights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryRightsId,hasWriteAccess,DocumentAccessRoleId,DocumentCategoryId")] CategoryRight categoryRight)
        {
            if (ModelState.IsValid)
            {
                db.CategoryRights.Add(categoryRight);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", categoryRight.DocumentAccessRoleId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", categoryRight.DocumentCategoryId);
            return View(categoryRight);
        }

        // GET: DMS/CategoryRights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            CategoryRight categoryRight = db.CategoryRights.Find(id);
            if (categoryRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", categoryRight.DocumentAccessRoleId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", categoryRight.DocumentCategoryId);
            return View(categoryRight);
        }

        // POST: DMS/CategoryRights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryRightsId,hasWriteAccess,DocumentAccessRoleId,DocumentCategoryId")] CategoryRight categoryRight)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryRight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", categoryRight.DocumentAccessRoleId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", categoryRight.DocumentCategoryId);
            return View(categoryRight);
        }

        // GET: DMS/CategoryRights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            CategoryRight categoryRight = db.CategoryRights.Find(id);
            if (categoryRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(categoryRight);
        }

        // POST: DMS/CategoryRights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryRight categoryRight = db.CategoryRights.Find(id);
            db.CategoryRights.Remove(categoryRight);
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
