using iSynergy.DataContexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.Shared;
using iSynergy.Controllers;
using iSynergy.Models;

namespace iSynergy.Areas.DMS.Controllers
{
    public class DocumentsController : CustomController
    {
        private FullDb db = new FullDb();
        // GET: DMS/Documents
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            var versions = db.DocumentVersions
                            .Where(x => x.DocumentId == id)
                            .OrderByDescending(x => x.VersionNumber)
                            .ToList();
            var employeeDictionary = new Dictionary<int, string>();
            foreach (var emp in db.Employees)
            {
                employeeDictionary.Add(emp.EmployeeId, emp.Name);
            }

            ViewBag.employeeDictionary = employeeDictionary;

            // mark notification as read
            var notifications = db.Notifications
                                .Where(
                                        x => x.EmployeeId == this.thisGuy.EmployeeId 
                                        && x.ExternalReferenceId == document.DocumentId
                                        && x.Module == "DMS"
                                        );

            db.Notifications.RemoveRange(notifications);
            db.SaveChanges();
            return View(versions);
        }

        [CheckLicenses]
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var category = db.DocumentCategories.Find(id);
            if (category == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            // check if user has access rights

            var roleIds = (from role in db.UserDocumentAccessRoles
                           where role.EmployeeId == this.thisGuy.EmployeeId
                           select role.DocumentAccessRoleId).ToList();

            var catRoleIds = (from role in db.CategoryRights
                              where role.DocumentCategoryId == id && role.hasWriteAccess == true
                              select role.DocumentAccessRoleId).ToList();

            var folderRoleIds = (from role in db.FolderRights
                                 where role.DocumentFolderId == category.DocuementFolderId && role.hasWriteAccess == true
                                 select role.DocumentAccessRoleId).ToList();

            var hasPermission = catRoleIds.Intersect(roleIds).Any() || folderRoleIds.Intersect(roleIds).Any();

            if (!hasPermission)
            {
                return RedirectToAction("Unauthorized", "Error", new { area = "" });
            }

            DataTable table = new DataTable();

            var attributes = (from attribute in db.DocumentAttributes
                             where attribute.DocumentCategoryId == id
                             select attribute).ToList();

            var lookupData = new Dictionary<int, List<string>>();
            var attributeTypes = new Dictionary<int, DocumentAttributeDataType>();
            var attributeIds = new Dictionary<int, int>();
            for (int i = 0; i < attributes.Count(); i++)
            {
                var attr = attributes[i];
                // add the attribute name as column header
                table.Columns.Add(attr.Name);

                // tell the view if this column needs to be populated with a drop down list
                if (attr.isMappedToDataList)
                {
                    var dataList = (from listItem in db.AttributeDataListItems
                                    where listItem.AttributeDataListId == attr.AttributeDataListId
                                    select listItem.Text)
                                   .ToList();

                    lookupData.Add(i, dataList);
                }

                // tell the view about the data type of each attribute. 
                attributeTypes.Add(i, attr.DocumentAttributeType.DataType);

                // tell the view about the attribute id of each attribute so that controls in the view can be named.
                attributeIds.Add(i,attr.DocumentAttributeId);

            }

            // add blank row that will be filled by the form in the view.
            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            ViewBag.lookupData = lookupData;
            ViewBag.attributeTypes = attributeTypes;
            ViewBag.attributeIds = attributeIds;
            ViewBag.Title = "Add new " + category.Name.ToLower();
            ViewBag.CategoryId = id;

            return View(table);
        }
        [CheckLicenses]
        public ActionResult CheckOut(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            if (document.isCheckedOut)
            {
                TempData["error"] = "Operations Failed! This document is already checked-out.";
                return RedirectToAction("Details", new { id = id });
            }
            if (document.isLocked)
            {
                TempData["error"] = "Operations Failed! This document is locked.";
                return RedirectToAction("Details", new { id = id });
            }
            document.isCheckedOut = true;
            document.checkedOutBy = thisGuy.EmployeeId;
            db.SaveChanges();
            var lastVersion = db.DocumentVersions
                                .Where(x => x.DocumentId == id)
                                .OrderByDescending(x => x.VersionNumber)
                                .First();

            return RedirectToAction("Index", "DownloadAttachment", new {area = "", id = lastVersion.URL });
        }
        [CheckLicenses]
        public ActionResult CheckIn(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            if (!document.isCheckedOut)
            {
                TempData["error"] = "Operations Failed! This document is already checked-in.";
                return RedirectToAction("Details", new { id = id });
            }
            if (document.isLocked)
            {
                TempData["error"] = "Operations Failed! This document is locked.";
                return RedirectToAction("Details", new { id = id });
            }
            TempData["documentId"] = document.DocumentId;
            return View();
        }
        [CheckLicenses]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIn(IEnumerable<HttpPostedFileBase> attachments, string notes)
        {
            int? id = (int?)TempData["documentId"];

            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            if (!document.isCheckedOut)
            {
                TempData["error"] = "Operations Failed! This document is already checked-in.";
                return RedirectToAction("Details", new { id = id });
            }
            if (document.isLocked)
            {
                TempData["error"] = "Operations Failed! This document is locked.";
                return RedirectToAction("Details", new { id = id });
            }
            document.isCheckedOut = false;
            db.SaveChanges();
            var lastVersion = db.DocumentVersions
                                .Where(x => x.DocumentId == id)
                                .OrderByDescending(x => x.DocumentVersionId)
                                .First();

            string filePath = "";
            //if more than 1 files attached, zip them together.
            if (attachments.Count() > 1)
            {
                filePath = FileOperations.createZipFileOnServer(attachments, Server);
            }
            else
            {
                // save file on server
                filePath = FileOperations.SaveFile(attachments.ElementAt(0), Server);
            }

            var newVersion = new DocumentVersion
            {
                DateCreated = DateTime.Today,
                EmployeeId = thisGuy.EmployeeId,
                DocumentId = document.DocumentId,
                VersionNumber = lastVersion.VersionNumber + 1,
                Notes = notes,
                URL = filePath
            };
            db.DocumentVersions.Add(newVersion);
            db.SaveChanges();
            informSubscribers(document.DocumentCategoryId, newVersion);
            TempData["success"] = "Document checked-in successfully.";
            return RedirectToAction("Details", new { id = id });
        }
        [CheckLicenses]
        public ActionResult Lock(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            if (document.isLocked)
            {
                TempData["error"] = "Operations Failed! This document is already locked.";
                return RedirectToAction("Details", new { id = id });
            }
            document.isLocked = true;
            document.lockedBy = thisGuy.EmployeeId;
            db.SaveChanges();
            TempData["success"] = "Document locked successfully!";
            var lastVersion = db.DocumentVersions
                                .Where(x => x.DocumentId == id)
                                .OrderByDescending(x => x.VersionNumber)
                                .First();

            return RedirectToAction("Details", new { id = id });
        }
        [CheckLicenses]
        public ActionResult Unlock(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            if (!document.isLocked)
            {
                TempData["error"] = "Operations Failed! This document is already unlocked.";
                return RedirectToAction("Details", new { id = id });
            }
            document.isLocked = false;
            db.SaveChanges();
            TempData["success"] = "Document unlocked successfully!";
            var lastVersion = db.DocumentVersions
                                .Where(x => x.DocumentId == id)
                                .OrderByDescending(x => x.VersionNumber)
                                .First();

            return RedirectToAction("Details", new { id = id });
        }

        [CheckLicenses]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(DataTable model, int CategoryId, IEnumerable<HttpPostedFileBase> attachments, string notes)
        {
            if (attachments == null)
            {
                TempData["error"] = "Please upload one or more files.";
                return View();
            }

            string filePath = "";
            //if more than 1 files attached, zip them together.
            if (attachments.Count() > 1)
            {
                filePath = FileOperations.createZipFileOnServer(attachments, Server);
            }
            else
            {
                // save file on server
                filePath = FileOperations.SaveFile(attachments.ElementAt(0), Server);
            }

            var attributes = (from attribute in db.DocumentAttributes
                              where attribute.DocumentCategoryId == CategoryId
                              select attribute).ToList();


            // add document
            var newDocument = new Document();
            newDocument.DocumentCategoryId = CategoryId;
            db.Documents.Add(newDocument);
            db.SaveChanges();

            // add version
            var newDocVersion = new DocumentVersion();
            newDocVersion.DocumentId = newDocument.DocumentId;
            newDocVersion.EmployeeId = this.thisGuy.EmployeeId;
            newDocVersion.VersionNumber = 1;
            newDocVersion.Notes = notes;
            newDocVersion.URL = filePath;
            db.DocumentVersions.Add(newDocVersion);
            db.SaveChanges();

            // add meta data
            for (int i = 0; i < attributes.Count(); i++)
            {
                var attr = attributes[i];
                var metadata = new DocumentMetaData
                {
                    DocumentId = newDocument.DocumentId,
                    DocumentAttributeId = attr.DocumentAttributeId,
                    AttributeValue = Request["Rows[0][" + i + "]"]
                };
                db.DocumentMetaDatas.Add(metadata);
            }
            db.SaveChanges();
            informSubscribers(CategoryId, newDocVersion);
            TempData["success"] = "Document added successfully.";
            return RedirectToAction("Index","Home", new { id= CategoryId });
        }

        private void informSubscribers(int CategoryId, DocumentVersion version)
        {
            var subscriberIds = db.CategorySubscriptions
                                .Where(x => x.DocumentCategoryId == CategoryId)
                                .Select(x => x.EmployeeId)
                                .ToList();

            var category = db.DocumentCategories.Find(CategoryId);

            foreach (var id in subscriberIds)
            {
                var msg = "";
                var publisher = db.Employees.Find(version.EmployeeId);

                if (version.VersionNumber > 1)
                {
                    msg = string.Format("{0} added a new document to {1}", publisher.Name, category.Name);
                }
                else
                {
                    msg = string.Format("{0} has updated a document in {1}", publisher.Name, category.Name);
                }
                
                var notification = new Notification
                {
                    Date = DateTime.Today,
                    EmployeeId = id,
                    isRead = false,
                    Text = msg,
                    URL = String.Format("/DMS/Documents/Details/{0}", version.DocumentId),
                    ExternalReferenceId = version.DocumentId,
                    Module = "DMS"
                };

                db.Notifications.Add(notification);
                db.SaveChanges();
            }
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