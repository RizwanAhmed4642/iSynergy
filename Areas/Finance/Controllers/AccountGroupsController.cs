using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSynergy.Areas.Finance.Models;
using iSynergy.DataContexts;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class AccountGroupsController : CustomController
    {
        private FinanceDb db = new FinanceDb();

        // GET: Finance/AccountGroups
        public ActionResult Index()
        {
            var accountGroups = db.AccountGroups
                .Include(a => a.AccountClass);
            return View(accountGroups.ToList());
        }


        // GET: Finance/AccountGroups/Create
        public ActionResult Create()
        {
            ViewBag.AccountClassId = new SelectList(db.AccountClasses, "AccountClassId", "Title");
            return View();
        }

        // POST: Finance/AccountGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,AccountClassId")] AccountGroup model)
        {
            var totalAccountGroups = db.AccountGroups
                                    .Where(x => x.AccountClassId == model.AccountClassId)
                                    .Count();

            char[] delimiters = new char[] { '\r', '\n', ',' };
            var newGroupTitles = model.Title.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var newGroupTitle in newGroupTitles)
            {
                var newAccountGroup = new AccountGroup
                {
                    AccountGroupId = model.AccountClassId + "-" + (++totalAccountGroups).ToString().PadLeft(2, '0'),
                    Title = newGroupTitle,
                    AccountClassId = model.AccountClassId
                };
                db.AccountGroups.Add(newAccountGroup);
            }

            var recordsInserted = db.SaveChanges();
            TempData["success"] = string.Format("{0} groups added successfully", recordsInserted);


            return RedirectToAction("Create");
        }

        // GET: Finance/AccountGroups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            AccountGroup accountGroup = db.AccountGroups.Find(id);
            if (accountGroup == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.AccountClassId = new SelectList(db.AccountClasses, "AccountClassId", "Title", accountGroup.AccountClassId);
            return View(accountGroup);
        }

        // POST: Finance/AccountGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountGroupId,Title,AccountClassId")] AccountGroup accountGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountClassId = new SelectList(db.AccountClasses, "AccountClassId", "Title", accountGroup.AccountClassId);
            return View(accountGroup);
        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {

            var accountGroups = (from accountGroup in db.AccountGroups
                                    join accountClass in db.AccountClasses on accountGroup.AccountClassId equals accountClass.AccountClassId
                                    where
                                     accountGroup.AccountGroupId.StartsWith(Prefix.ToLower())
                                     ||
                                     accountGroup.Title.Contains(Prefix.ToLower())
                                     ||
                                     accountClass.Title.ToLower().Contains(Prefix.ToLower())
                                    select accountGroup);

            return Json(accountGroups.OrderBy(x => x.AccountGroupId), JsonRequestBehavior.AllowGet);
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
