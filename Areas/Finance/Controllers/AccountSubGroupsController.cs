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
using System.Data.Entity.Core.Objects;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class AccountSubGroupsController : CustomController
    {
        private FinanceDb db = new FinanceDb();

        // GET: Finance/AccountSubGroups
        public ActionResult Index()
        {
            var accountSubGroups = db.AccountSubGroups;
            return View(accountSubGroups);
        }


        // GET: Finance/AccountSubGroups/Create
        public ActionResult Create()
        {
            ViewBag.AccountGroupId = new SelectList(db.AccountGroups, "AccountGroupId", "Title");
            return View();
        }

        // POST: Finance/AccountSubGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,AccountGroupId")] AccountSubGroup model)
        {

            var totalAccountSubGroups = db.AccountSubGroups
                                    .Where(x => x.AccountGroupId == model.AccountGroupId)
                                    .Count();
            char[] delimiters = new char[] { '\r', '\n', ',' };
            var newSubGroupTitles = model.Title.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var newSubGroupTitle in newSubGroupTitles)
            {
                var newAccountSubGroup = new AccountSubGroup
                {
                    AccountSubGroupId = model.AccountGroupId + "-" + (++totalAccountSubGroups).ToString().PadLeft(2, '0'),
                    Title = newSubGroupTitle,
                    AccountGroupId = model.AccountGroupId
                };
                db.AccountSubGroups.Add(newAccountSubGroup);
            }

            var recordsInserted = db.SaveChanges();
            TempData["success"] = string.Format("{0} sub groups added successfully", recordsInserted);

            return RedirectToAction("Create");
        }

        // GET: Finance/AccountSubGroups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            AccountSubGroup accountSubGroup = db.AccountSubGroups.Find(id);
            if (accountSubGroup == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.AccountGroupId = new SelectList(db.AccountGroups, "AccountGroupId", "Title", accountSubGroup.AccountGroupId);
            return View(accountSubGroup);
        }

        // POST: Finance/AccountSubGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountSubGroupId,Title,AccountGroupId")] AccountSubGroup accountSubGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountSubGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountGroupId = new SelectList(db.AccountClasses, "AccountClassId", "Title", accountSubGroup.AccountGroupId);
            return View(accountSubGroup);
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
