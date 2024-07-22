using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Areas.DMS.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    public class HomeController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/Home
        public ActionResult Index(int? id, bool? isFiltered)
        {
            var isFollower = false;
            DocumentCategory category = db.DocumentCategories.Find(id);

            if (category != null)
            {
                // check if user has access rights

                var roleIds = (from role in db.UserDocumentAccessRoles
                               where role.EmployeeId == this.thisGuy.EmployeeId
                               select role.DocumentAccessRoleId).ToList();

                var catRoleIds = (from role in db.CategoryRights
                                  where role.DocumentCategoryId == id
                                  select role.DocumentAccessRoleId).ToList();

                var folderRoleIds = (from role in db.FolderRights
                                     where role.DocumentFolderId == category.DocuementFolderId
                                     select role.DocumentAccessRoleId).ToList();

                var hasPermission = catRoleIds.Intersect(roleIds).Any() || folderRoleIds.Intersect(roleIds).Any();

                if (!hasPermission)
                {
                    return RedirectToAction("Unauthorized", "Error", new { area = "" });
                }

                isFollower = db.CategorySubscriptions
                            .Where(x => x.DocumentCategoryId == category.DocumentCategoryId && x.EmployeeId == this.thisGuy.EmployeeId)
                            .Any();
            }


            // get all documents in this category
            var documents = new List<Document>();
            if (isFiltered ?? false)
            {
                // filter button has been pressed
                documents = (List<Document>)TempData["filteredDocuments"];
            }
            else
            {
                documents = db.Documents
                            .Where(x => x.DocumentCategoryId == id)
                            .ToList();
            }
            

            // get attributes definitions associated with this category
            var attributes = db.DocumentAttributes
                                .Where(x => x.DocumentCategoryId == id)
                                .ToList();

            DataTable table = new DataTable();
            var attributeIds = new Dictionary<int, int>();
            var attributeTypes = new Dictionary<int, DocumentAttributeDataType>();
            var documentIds = new Dictionary<int, int>();

            // add attributes as column headers in table
            for (int i = 0; i < attributes.Count(); i++)
            {
                table.Columns.Add(attributes[i].Name);

                // tell the view type of each attribute
                attributeIds.Add(i, attributes.ElementAt(i).DocumentAttributeId);

                // tell the view about the data type of each attribute. 
                attributeTypes.Add(i, attributes.ElementAt(i).DocumentAttributeType.DataType);
            }

            // add metadata to the table
            for (int i = 0; i < documents.Count(); i++)
            {
                var docId = documents[i].DocumentId;
                documentIds.Add(i,docId);
                var metadata = db.DocumentMetaDatas
                                .Where(x => x.DocumentId == docId)
                                .ToList();

                DataRow row = table.NewRow();
                for (int j = 0; j < metadata.Count(); j++)
                {
                    
                    row[j] = metadata.ElementAt(j).AttributeValue;
                }
                table.Rows.Add(row);
            }

            
            
            ViewBag.attributeIds = attributeIds;
            ViewBag.attributeTypes = attributeTypes;
            ViewBag.documentIds = documentIds;
            ViewBag.CategoryId = id;
            ViewBag.Category = category;
            //ViewBag.folder = category.DocumentFolder;
            ViewBag.isFollower = isFollower;
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(int CategoryId)
        {
            var attributes = (from key in Request.Params.AllKeys
                              where key.StartsWith("attribute[")
                              select key).ToList();

            var operators = (from key in Request.Params.AllKeys
                             where key.StartsWith("operator[")
                             select key).ToList();

            var values = (from key in Request.Params.AllKeys
                          where key.StartsWith("val[")
                          select key).ToList();

            var andOrClauses = (from key in Request.Params.AllKeys
                              where key.StartsWith("andorclause[")
                              select key).ToList();

            

            // build the SQL statement
            var SQL = "";

            //var docs = new List<Document>();
            var documents = new List<Document>();

            if (attributes.Count() > 0)
            {
                // run for each filter condition
                for (int i = 0; i < attributes.Count(); i++) 
                {
                    // build the SQL statement
                    if (Request[operators[i]] == "=")
                    {
                        SQL = string.Format(
                        "SELECT * FROM DocumentMetaDatas WHERE DocumentAttributeId = '{0}' AND AttributeValue {1} '{2}'",
                        Request[attributes[i]],
                        Request[operators[i]],
                        Request[values[i]]
                        );
                    }
                    else 
                    {
                        var attrId = Int32.Parse(Request[attributes[i]]);
                        var attr = (from attribute in db.DocumentAttributes
                                   where attribute.DocumentCategoryId == CategoryId && attribute.DocumentAttributeId == attrId
                                   select attribute).FirstOrDefault();

                        // SQL for number comparison
                        if (attr.DocumentAttributeType.DataType == DocumentAttributeDataType.Number)
                        {
                            SQL = string.Format(
                                            "SELECT * FROM DocumentMetaDatas WHERE DocumentAttributeId = '{0}' AND CAST(AttributeValue AS INT) {1} {2}",
                                            Request[attributes[i]],
                                            Request[operators[i]],
                                            Request[values[i]]
                                        );
                        }
                        // SQL for date and time comparison
                        else if (attr.DocumentAttributeType.DataType == DocumentAttributeDataType.Date || attr.DocumentAttributeType.DataType == DocumentAttributeDataType.Time)
                        {
                            SQL = string.Format(
                                            "SELECT * FROM DocumentMetaDatas WHERE DocumentAttributeId = '{0}' AND CAST(AttributeValue AS datetime) {1} CAST('{2}' AS datetime)",
                                            Request[attributes[i]],
                                            Request[operators[i]],
                                            Request[values[i]]
                                        );
                        }
                        else if (attr.DocumentAttributeType.DataType == DocumentAttributeDataType.Currency)
                        {
                            SQL = string.Format(
                                            "SELECT * FROM DocumentMetaDatas WHERE DocumentAttributeId = '{0}' AND CAST(AttributeValue AS DECIMAL) {1} CAST('{2}' AS DECIMAL)",
                                            Request[attributes[i]],
                                            Request[operators[i]],
                                            Request[values[i]]
                                        );
                        }

                    }
                    
                    // run SQL and fetch records
                    var result = db.DocumentMetaDatas.SqlQuery(SQL)
                                .Select(x => x.Document)
                                .Distinct()
                                .ToList();

                    if (i > 0)
                    {
                        if (Request[andOrClauses[i - 1]] == "AND")
                        {
                            documents = intersect(documents, result);
                        }
                        else
                        {
                            documents = union(documents, result);
                        }
                    }
                    else
                    {
                        documents.AddRange(result);
                    }

                }
            }
            

            TempData["info"] = string.Format("{0} documents found.", documents.Distinct().Count());
            TempData["filteredDocuments"] = documents;
            return RedirectToAction("Index", new { id = CategoryId , isFiltered = true});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<Document> intersect(List<Document> A, List<Document> B)
        {
            List<Document> result = new List<Document>();

            for (int i = 0; i < A.Count; i++)
            {
                if (B.Contains(A[i]))
                {
                    result.Add(A[i]);
                }
            }

            return result;
        }

        private List<Document> union(List<Document> A, List<Document> B)
        {

            for (int i = 0; i < B.Count; i++)
            {
                if (!A.Contains(B[i]))
                {
                    A.Add(B[i]);
                }
            }
            return A;
        }
    }

}