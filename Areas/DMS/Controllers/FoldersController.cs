using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class FoldersController : CustomController
    {
        private ArchivingDb db = new ArchivingDb();

        // GET: DMS/Folders
        public ActionResult Index()
        {
            return View(db.DocumentFolders.ToList());
        }

        // GET: DMS/Folders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentFolder documentFolder = db.DocumentFolders.Find(id);
            if (documentFolder == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentFolder);
        }

        // GET: DMS/Folders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DMS/Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentFolderId,Name")] DocumentFolder documentFolder)
        {
            if (ModelState.IsValid)
            {
                db.DocumentFolders.Add(documentFolder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documentFolder);
        }

        // GET: DMS/Folders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentFolder documentFolder = db.DocumentFolders.Find(id);
            if (documentFolder == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentFolder);
        }

        // POST: DMS/Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentFolderId,Name")] DocumentFolder documentFolder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentFolder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documentFolder);
        }

        // GET: DMS/Folders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var folder = db.DocumentFolders.Find(id);
            if (folder == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            return View(folder);
        }

        // POST: DMS/Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentFolder folder = db.DocumentFolders.Find(id);
            var categories = from category in db.DocumentCategories
                             where category.DocuementFolderId == folder.DocumentFolderId
                             select category;

            if (categories.Count() > 0)
            {
                TempData["error"] = string.Format("Cannot delete folder '{0}' because it has {1} categories under it.", folder.Name, categories.Count());
            }
            else
            {
                db.DocumentFolders.Remove(folder);
                db.SaveChanges();
                TempData["success"] = string.Format("'{0}' folder deleted.", folder.Name);
            }
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
