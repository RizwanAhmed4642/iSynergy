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
    public class AccountsController : CustomController
    {
        private FinanceDb db = new FinanceDb();

        // GET: Finance/Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts;
            return View(accounts.ToList());
        }

        // GET: Finance/Accounts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(account);
        }

        // GET: Finance/Accounts/Create
        public ActionResult Create()
        {
            ViewBag.AccountSubGroupId = new SelectList(db.AccountSubGroups, "AccountSubGroupId", "Title");
            return View();
        }

        // POST: Finance/Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,AccountSubGroupId")] Account model)
        {
            
            var totalAccounts = db.Accounts
                                     .Where(x => x.AccountSubGroupId == model.AccountSubGroupId)
                                     .Count();
            char[] delimiters = new char[] { '\r', '\n', ',' };
            var newAccountsTitles = model.Title.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var newAccountTitle in newAccountsTitles)
            {
                var newAccount = new Account
                {
                    AccountId = model.AccountSubGroupId + "-" + (++totalAccounts).ToString().PadLeft(4, '0'),
                    AccountSubGroupId = model.AccountSubGroupId,
                    Title = newAccountTitle,
                    Status = AccountStatus.Active
                };
                db.Accounts.Add(newAccount);
            }
            
            var recordsInserted = db.SaveChanges();

            TempData["success"] = string.Format("{0} accounts added successfully", recordsInserted);
            return RedirectToAction("Create");
        }

        // GET: Finance/Accounts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.AccountSubGroupId = new SelectList(db.AccountSubGroups, "AccountSubGroupId", "Title", account.AccountSubGroupId);
            return View(account);
        }

        // POST: Finance/Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountId,Title,AccountSubGroupId,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountSubGroupId = new SelectList(db.AccountGroups, "AccountGroupId", "Title", account.AccountSubGroupId);
            return View(account);
        }

        // GET: Finance/Accounts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(account);
        }

        // POST: Finance/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            var accounts = from account in db.Accounts
                           join accountSubGroup in db.AccountSubGroups on account.AccountSubGroupId equals accountSubGroup.AccountSubGroupId
                           join accountGroup in db.AccountGroups on accountSubGroup.AccountGroupId equals accountGroup.AccountGroupId
                           join accountClass in db.AccountClasses on accountGroup.AccountClassId equals accountClass.AccountClassId
                           where
                            (
                                account.AccountId.StartsWith(Prefix)
                                ||
                                account.Title.ToLower().Contains(Prefix.ToLower())
                                ||
                                accountSubGroup.Title.Contains(Prefix.ToLower())
                                ||
                                accountGroup.Title.ToLower().Contains(Prefix.ToLower())
                                ||
                                accountClass.Title.ToLower().Contains(Prefix.ToLower())
                            )
                            &&
                            account.Status == AccountStatus.Active
                           select account;

            return Json(accounts.OrderBy(x => x.AccountId), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SearchWithCategories(string Prefix)
        {
            var accounts = (from account in db.Accounts
                           where 
                           (
                                account.Title.ToLower().Contains(Prefix.ToLower())
                                ||
                                account.AccountId.StartsWith(Prefix)
                           )
                           &&
                           account.Status == AccountStatus.Active
                            select new {
                               account = account.Title,
                               id = account.AccountId
                           }).ToList();

            var accountSubGroups = (from accountSubGroup in db.AccountSubGroups
                           where
                           accountSubGroup.Title.ToLower().Contains(Prefix.ToLower())
                            ||
                            accountSubGroup.AccountSubGroupId.StartsWith(Prefix)
                           select new
                           {
                               account = accountSubGroup.Title,
                               id = accountSubGroup.AccountSubGroupId
                           }).ToList();

            var accountGroups = (from accoutnGroup in db.AccountGroups
                             where
                             accoutnGroup.Title.ToLower().Contains(Prefix.ToLower())
                              ||
                              accoutnGroup.AccountGroupId.StartsWith(Prefix)
                             select new
                             {
                                 account = accoutnGroup.Title,
                                 id = accoutnGroup.AccountGroupId
                             }).ToList();

            var accountClasses = (from accountClass in db.AccountClasses
                                 where
                                 accountClass.Title.ToLower().Contains(Prefix.ToLower())
                                  ||
                                  accountClass.AccountClassId.StartsWith(Prefix)
                                 select new
                                 {
                                     account = accountClass.Title,
                                     id = accountClass.AccountClassId
                                 }).ToList();
            var result = accounts.Concat(accountSubGroups.Concat(accountGroups.Concat(accountClasses)));

            return Json(result.OrderBy(x => x.id), JsonRequestBehavior.AllowGet);
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
