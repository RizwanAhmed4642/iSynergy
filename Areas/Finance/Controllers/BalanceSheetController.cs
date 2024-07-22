using iSynergy.Areas.Finance.Models;
using iSynergy.DataContexts;
using iSynergy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class BalanceSheetController : CustomController
    {
        private FinanceDb db = new FinanceDb();
        // GET: Finance/BalanceSheet
        public ActionResult Index()
        {
            var model = new BalanceSheetViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BalanceSheetViewModel model)
        {
            model.startingYear = (from journal in db.Journals
                                select journal.Date)
                                .Min()
                                .Year;
            model.endingYear = model.asOfDate.Year;

            model.yearsSpan = model.endingYear - model.startingYear;

            List<string> creditNormalAccounts = new List<string>() { "Income", "Equity", "Liabilities"};
 

            var postings = from posting in db.Postings
                          join accountSubGroup in db.AccountSubGroups on posting.Account.AccountSubGroupId equals accountSubGroup.AccountSubGroupId
                          join accountGroup in db.AccountGroups on accountSubGroup.AccountGroupId equals accountGroup.AccountGroupId
                          join accountClass in db.AccountClasses on accountGroup.AccountClassId equals accountClass.AccountClassId
                          where posting.Journal.Date <= model.asOfDate
                          select new {
                                        AccountClass = accountClass,
                                        AccountGroup = accountGroup,
                                        AccountSubGroup = accountSubGroup,
                                        Account = posting.Account.Title,
                                        Balance = creditNormalAccounts.Contains(accountClass.Title) ? posting.Credit - posting.Debit : posting.Debit - posting.Credit,
                                        Year = posting.Journal.Date.Year
                                      };

            model.fromDate = (from posting in db.Postings
                              select posting.Journal.Date).Min();

            var records = from posting in postings
                          group posting by new { posting.AccountSubGroup, posting.Year } into g
                          select new YearlyBalanceViewModel {
                                        AccountClass = g.FirstOrDefault().AccountClass,
                                        AccountGroup = g.FirstOrDefault().AccountGroup,
                                        AccountSubGroup = g.FirstOrDefault().AccountSubGroup,
                                        Balance = g.Sum(x => x.Balance),
                                        Year = g.FirstOrDefault().Year
                                    };

            var assets = from record in records
                           where record.AccountClass.Title == "Assets"
                           select record;

            var liabilities = from record in records
                         where record.AccountClass.Title == "Liabilities"
                         select record;

            var equity = from record in records
                         where record.AccountClass.Title == "Equity"
                         select record;

            model.Assets = assets.ToList();
            model.Liabilities = liabilities.ToList();
            model.Equity = equity.ToList();
            model.printedBy = this.thisGuy.Name;
            if (assets.Count() == 0)
            {
                TempData["error"] = "No records found!";
            }

            return View(model);
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