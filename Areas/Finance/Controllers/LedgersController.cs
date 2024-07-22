using iSynergy.Areas.Finance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Shared;
using Microsoft.AspNet.Identity;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class LedgersController : CustomController
    {
        FinanceDb db = new FinanceDb();
        // GET: Finance/GeneralLedgerInquiry
        public ActionResult Index()
        {
            var model = new GeneralLedgerInquiryViewModel();
            model.FromDate = Shared.FinanceOperations.getFiscalCalendarModal().StartDate;
            model.ToDate = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GeneralLedgerInquiryViewModel model)
        {

            return View(fillLedgerModel(model));
        }


        public ActionResult Detail(string id)
        {
            var model = new GeneralLedgerInquiryViewModel();
            model.fromAccountId = id;
            model.FromDate = Shared.FinanceOperations.getFiscalCalendarModal().StartDate;
            model.ToDate = DateTime.Today;

            return View("Index", fillLedgerModel(model));
        }

        private GeneralLedgerInquiryViewModel fillLedgerModel(GeneralLedgerInquiryViewModel model)
        {
            IEnumerable<Posting> postings = db.Postings
                                                .Where(x => x.Journal.Date >= model.FromDate
                                                && x.Journal.Date <= model.ToDate);



            if (model.fromAccountId != null)
            {

                postings = (from posting in postings
                            where posting.AccountId.StartsWith(model.fromAccountId)
                            select posting).ToList();
            }


            if (model.toAccountId != null)
            {
                // get account id of the first account in the range that crosses the border of intended search range.
                var accountId = (from posting in postings
                                 where posting.AccountId.StartsWith(model.toAccountId)
                                 orderby posting.AccountId
                                 select posting.AccountId)
                                .FirstOrDefault();
                if (accountId != null)
                {
                    postings = (from posting in postings
                                where String.Compare(posting.AccountId, accountId) < 0
                                select posting).ToList();
                }
                else
                {
                    TempData["info"] = "To Account code must fall within the range of From Account codes";
                }


            }

            if (model.FromDate != null)
            {
                postings = from posting in postings
                           where posting.Journal.Date >= model.FromDate
                           select posting;
            }

            if (model.ToDate != null)
            {
                postings = from posting in postings
                           where posting.Journal.Date <= model.ToDate
                           select posting;
            }

            if (!postings.Any())
            {
                TempData["Error"] = "No entries found.";
            }

            var ledgers = from posting in postings
                          group posting by posting.Account into a
                          select new { Account = a.Key, Postings = a.ToList() };
            model.Ledgers = new List<LedgerViewModel>();
            foreach (var ledger in ledgers)
            {
                var lvm = new LedgerViewModel();
                lvm.Account = new Account
                {
                    AccountId = ledger.Account.AccountId,
                    AccountSubGroupId = ledger.Account.AccountSubGroupId,
                    Status = ledger.Account.Status,
                    Title = ledger.Account.Title,
                };
                lvm.OpeningDebitBalance = Shared.FinanceOperations.getAccountOpeningDebitBalance(lvm.Account, model.FromDate);
                lvm.OpeningCreditBalance = Shared.FinanceOperations.getAccountOpeningCreditBalance(lvm.Account, model.FromDate);
                lvm.Postings = ledger.Postings.ToList();
                model.Ledgers.Add(lvm);
            }

            model.printedBy = this.thisGuy.Name;
            ViewBag.Accounts = db.Accounts
                   .OrderBy(x => x.Title);

            var postingDates = from posting in postings
                               select posting.Journal.Date;
            var startDateYear = postingDates.Min().Year;
            var endDateYear = postingDates.Max().Year;
            if (endDateYear - startDateYear == 0)
            {
                model.fiscalYear = string.Format("{0}", startDateYear);
            }
            else
            {
                model.fiscalYear = string.Format("{0} - {1}", startDateYear, endDateYear);
            }

            return model;
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