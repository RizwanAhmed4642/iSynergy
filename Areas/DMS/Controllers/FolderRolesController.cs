using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class FolderRolesController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/FolderRights
        public ActionResult Index()
        {
            var folderRights = db.FolderRights.Include(f => f.DocumentAccessRole).Include(f => f.DocumentFolder);
            return View(folderRights.ToList());
        }

        // GET: DMS/FolderRights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            FolderRight folderRight = db.FolderRights.Find(id);
            if (folderRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(folderRight);
        }

        // GET: DMS/FolderRights/Create
        public ActionResult Create()
        {
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name");
            ViewBag.DocumentFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name");
            return View();
        }

        // POST: DMS/FolderRights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FolderRightsId,hasWriteAccess,DocumentAccessRoleId,DocumentFolderId")] FolderRight folderRight)
        {
            if (ModelState.IsValid)
            {
                db.FolderRights.Add(folderRight);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", folderRight.DocumentAccessRoleId);
            ViewBag.DocumentFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", folderRight.DocumentFolderId);
            return View(folderRight);
        }

        // GET: DMS/FolderRights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            FolderRight folderRight = db.FolderRights.Find(id);
            if (folderRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", folderRight.DocumentAccessRoleId);
            ViewBag.DocumentFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", folderRight.DocumentFolderId);
            return View(folderRight);
        }

        // POST: DMS/FolderRights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FolderRightsId,hasWriteAccess,DocumentAccessRoleId,DocumentFolderId")] FolderRight folderRight)
        {
            if (ModelState.IsValid)
            {
                db.Entry(folderRight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentAccessRoleId = new SelectList(db.DocumentAccessRoles, "DocumentAccessRoleId", "Name", folderRight.DocumentAccessRoleId);
            ViewBag.DocumentFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", folderRight.DocumentFolderId);
            return View(folderRight);
        }

        // GET: DMS/FolderRights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            FolderRight folderRight = db.FolderRights.Find(id);
            if (folderRight == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(folderRight);
        }

        // POST: DMS/FolderRights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FolderRight folderRight = db.FolderRights.Find(id);
            db.FolderRights.Remove(folderRight);
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
