using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class CategoriesController : CustomController
    {
        private ArchivingDb db = new ArchivingDb();

        // GET: DMS/Categories
        public ActionResult Index()
        {
            var documentCategories = db.DocumentCategories.Include(d => d.DocumentFolder);
            return View(documentCategories.ToList());
        }

        public ActionResult Follow(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentCategory category = db.DocumentCategories.Find(id);
            if (category == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            var followRequest = new CategorySubscription
            {
                DocumentCategoryId = category.DocumentCategoryId,
                EmployeeId = this.thisGuy.EmployeeId
            };

            db.CategorySubscriptions.Add(followRequest);
            db.SaveChanges();

            TempData["success"] = "You will get notification on future updates in " + category.Name;
            return RedirectToAction("Index", "Home", new { id = category.DocumentCategoryId});
        }

        public ActionResult Unfollow(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentCategory category = db.DocumentCategories.Find(id);
            if (category == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            var followRequest = db.CategorySubscriptions
                                .Where(x => x.DocumentCategoryId == category.DocumentCategoryId && x.EmployeeId == this.thisGuy.EmployeeId)
                                .FirstOrDefault();

            if (followRequest != null)
            {
                db.CategorySubscriptions.Remove(followRequest);
                db.SaveChanges();
            }

            TempData["success"] = "You will not get notifications about " + category.Name;
            return RedirectToAction("Index", "Home", new { id = category.DocumentCategoryId });
        }

        // GET: DMS/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentCategory documentCategory = db.DocumentCategories.Find(id);
            if (documentCategory == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentCategory);
        }

        // GET: DMS/Categories/Create
        public ActionResult Create()
        {
            ViewBag.DocuementFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name");
            return View();
        }

        // POST: DMS/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentCategoryId,Name,DocuementFolderId")] DocumentCategory documentCategory)
        {
            if (ModelState.IsValid)
            {
                db.DocumentCategories.Add(documentCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocuementFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", documentCategory.DocuementFolderId);
            return View(documentCategory);
        }

        // GET: DMS/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentCategory documentCategory = db.DocumentCategories.Find(id);
            if (documentCategory == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DocuementFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", documentCategory.DocuementFolderId);
            return View(documentCategory);
        }

        // POST: DMS/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentCategoryId,Name,DocuementFolderId")] DocumentCategory documentCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocuementFolderId = new SelectList(db.DocumentFolders, "DocumentFolderId", "Name", documentCategory.DocuementFolderId);
            return View(documentCategory);
        }

        // GET: DMS/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentCategory documentCategory = db.DocumentCategories.Find(id);
            if (documentCategory == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentCategory);
        }

        // POST: DMS/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentCategory category = db.DocumentCategories.Find(id);
            var documents = from doc in db.Documents
                           where doc.DocumentCategoryId == category.DocumentCategoryId
                           select doc;

            if (documents.Count() > 0)
            {
                TempData["error"] = string.Format("Cannot delete category '{0}' because it has {1} documents under it.", category.Name, documents.Count());
            }
            else
            {
                var attributes = from attr in db.DocumentAttributes
                                 where attr.DocumentCategoryId == category.DocumentCategoryId
                                 select attr;

                if (attributes.Count() > 0)
                {
                    TempData["error"] = string.Format("Cannot delete category '{0}' because it has {1} attributes under it.", category.Name, attributes.Count());
                }
                else
                {
                    db.DocumentCategories.Remove(category);
                    db.SaveChanges();
                    TempData["success"] = string.Format("'{0}' category deleted.", category.Name);
                }
                
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
