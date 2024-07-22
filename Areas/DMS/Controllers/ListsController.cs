using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Areas.DMS.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.DMS.Controllers
{
    [AuthorizeRedirect(Roles = "Admin")]
    public class ListsController : CustomController
    {
        private FullDb db = new FullDb();

        // GET: DMS/AttributeDataLists
        public ActionResult Index()
        {
            return View(db.AttributeDataLists.ToList());
        }

        // GET: DMS/AttributeDataLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            AttributeDataList attributeDataList = db.AttributeDataLists.Find(id);
            if (attributeDataList == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(attributeDataList);
        }

        // GET: DMS/AttributeDataLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DMS/AttributeDataLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AttributeDataList model, string listItems)
        {
            if (ModelState.IsValid)
            {
                var dataList = new AttributeDataList {
                    Name = iSynergy.Shared.StringOperations.ToTitleCase(model.Name)
                };
                db.AttributeDataLists.Add(dataList);
                db.SaveChanges();
                char[] deliminator = { '\n', ','};
                var items = listItems.Split(deliminator);
                foreach (var item in items)
                {
                    var thisItem = iSynergy.Shared.StringOperations.ToTitleCase(item);
                    var listItem = new AttributeDataListItem {
                        Value = thisItem,
                        Text = thisItem,
                        AttributeDataListId = dataList.AttributeDataListId
                    };
                    db.AttributeDataListItems.Add(listItem);
                }
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: DMS/AttributeDataLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            AttributeDataList attributeDataList = db.AttributeDataLists.Find(id);
            if (attributeDataList == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(attributeDataList);
        }

        // POST: DMS/AttributeDataLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AttributeDataListId,Name")] AttributeDataList attributeDataList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attributeDataList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attributeDataList);
        }

        // GET: DMS/AttributeDataLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            AttributeDataList attributeDataList = db.AttributeDataLists.Find(id);
            if (attributeDataList == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(attributeDataList);
        }

        // POST: DMS/AttributeDataLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttributeDataList attributeDataList = db.AttributeDataLists.Find(id);
            db.AttributeDataLists.Remove(attributeDataList);
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
