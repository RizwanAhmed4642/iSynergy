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
    public class AttributesController : CustomController
    {
        private ArchivingDb db = new ArchivingDb();

        // GET: DMS/Attributes
        public ActionResult Index()
        {
            var documentAttributes = db.DocumentAttributes.Include(d => d.DocumentAttributeType).Include(d => d.DocumentCategory);
            return View(documentAttributes.ToList());
        }

        // GET: DMS/Attributes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAttribute documentAttribute = db.DocumentAttributes.Find(id);
            if (documentAttribute == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentAttribute);
        }

        // GET: DMS/Attributes/Create
        public ActionResult Create()
        {
            ViewBag.DocumentAttributeTypeId = new SelectList(db.DocumentAttributeTypes, "DocumentAttributeTypeId", "DataType");
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name");
            ViewBag.AttributeDataListId = new SelectList(db.AttributeDataLists, "AttributeDataListId", "Name");
            return View();
        }

        // POST: DMS/Attributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentAttributeId,Name,DocumentCategoryId,DocumentAttributeTypeId")] DocumentAttribute documentAttribute)
        {
            if (ModelState.IsValid)
            {
                db.DocumentAttributes.Add(documentAttribute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocumentAttributeTypeId = new SelectList(db.DocumentAttributeTypes, "DocumentAttributeTypeId", "DataType", documentAttribute.DocumentAttributeTypeId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", documentAttribute.DocumentCategoryId);
            return View(documentAttribute);
        }

        // GET: DMS/Attributes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAttribute documentAttribute = db.DocumentAttributes.Find(id);
            if (documentAttribute == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DocumentAttributeTypeId = new SelectList(db.DocumentAttributeTypes, "DocumentAttributeTypeId", "DataType", documentAttribute.DocumentAttributeTypeId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", documentAttribute.DocumentCategoryId);
            ViewBag.AttributeDataListId = new SelectList(db.AttributeDataLists, "AttributeDataListId", "Name", documentAttribute.DocumentCategoryId);
            return View(documentAttribute);
        }

        // POST: DMS/Attributes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentAttributeId,Name,DocumentCategoryId,DocumentAttributeTypeId,isMappedToDataList,AttributeDataListId")] DocumentAttribute documentAttribute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentAttribute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentAttributeTypeId = new SelectList(db.DocumentAttributeTypes, "DocumentAttributeTypeId", "DataType", documentAttribute.DocumentAttributeTypeId);
            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories, "DocumentCategoryId", "Name", documentAttribute.DocumentCategoryId);
            return View(documentAttribute);
        }

        // GET: DMS/Attributes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            DocumentAttribute documentAttribute = db.DocumentAttributes.Find(id);
            if (documentAttribute == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(documentAttribute);
        }

        // POST: DMS/Attributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentAttribute attribute = db.DocumentAttributes.Find(id);
            var documents = from doc in db.Documents
                            where doc.DocumentCategoryId == attribute.DocumentCategoryId
                            select doc;

            if (documents.Count() > 0)
            {
                TempData["error"] = string.Format("Cannot delete attribute '{0}' because it has {1} documents using it.", attribute.Name, documents.Count());
            }
            else
            {
                db.DocumentAttributes.Remove(attribute);
                db.SaveChanges();
                TempData["success"] = string.Format("'{0}' attribute deleted.", attribute.Name);
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
