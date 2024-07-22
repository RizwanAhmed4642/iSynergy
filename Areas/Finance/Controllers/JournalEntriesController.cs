using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Areas.Finance.Models;
using iSynergy.Areas.HR.Models;
using Microsoft.AspNet.Identity;
using System.Web;
using iSynergy.Shared;
using iSynergy.Areas.Finance.Shared;
using iSynergy.Areas.Procurement.Models;
using System;
using iSynergy.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class JournalEntriesController : CustomController
    {
        FinanceDb financeDb = new FinanceDb();
        RequisitionDb requisitionDb = new RequisitionDb();
        CompanyDb companyDb = new CompanyDb();

        public JournalEntriesController()
        {
            FinanceOperations.AddFiscalPeriodsIfRequired();
        }
        public ActionResult Index()
        {

            var model = new List<JournalEntryViewModel>();

            // journal entries of last 3 months
            var last3Months = DateTime.Today.AddMonths(-3);
            var journals = financeDb.Journals
                            .Where(x => DateTime.Compare(x.Date, last3Months) >= 0)
                            .OrderByDescending(x => x.Date)
                            .ThenByDescending(x => x.JournalId);

            foreach (var journal in journals)
            {
                var jvm = new JournalEntryViewModel();
                jvm.JournalId = journal.JournalId;
                jvm.Date = journal.Date;
                jvm.Memo = journal.Memo;
                jvm.Postings = new List<Posting>();
                jvm.VoucherType = journal.VoucherType;
                jvm.Reference = journal.Reference;

                using (FinanceDb db2 = new FinanceDb())
                {
                    var postings = db2.Postings
                        .Where(x => x.JournalId == journal.JournalId);

                    foreach (var posting in postings)
                    {
                        var _posting = new Posting
                        {
                            PostingId = posting.PostingId,
                            Account = posting.Account,
                            AccountId = posting.AccountId,
                            Memo = posting.Memo,
                            Debit = posting.Debit,
                            Credit = posting.Credit,
                            Attachment = posting.Attachment
                        };
                        jvm.Postings.Add(_posting);
                    }
                }
                model.Add(jvm);
            }

            return View(model);
        }
        [CheckLicenses]
        public ActionResult Create(int? id)
        {
            //ViewBag.Accounts = getActiveAccounts();

            var model = new JournalEntryViewModel();
            model.Date = DateTime.Today;
            if (id == null) // create empty model
            {
                model.Postings = new List<Posting>();
                model.Postings.Add(new Posting());

            }
            else // create a model that is copy of a journal with given id.
            {
                var journal = financeDb.Journals.Find(id);

                if (journal == null)
                {
                    return RedirectToAction("PageNotFound", "Error", new { area = "" });
                }

                model.Date = journal.Date;
                model.Memo = journal.Memo;
                model.VoucherType = journal.VoucherType;
                model.Postings = new List<Posting>();
                using (FinanceDb db2 = new FinanceDb())
                {
                    var postings = db2.Postings
                        .Where(x => x.JournalId == journal.JournalId);

                    foreach (var posting in postings)
                    {
                        var account = new Account();
                        account.Title = posting.Account.Title;
                        account.AccountId = posting.AccountId;
                        account.AccountSubGroupId = posting.Account.AccountSubGroupId;
                        var _posting = new Posting
                        {
                            Account = account,
                            AccountId = account.AccountId,
                            Memo = posting.Memo,
                            Debit = posting.Debit,
                            Credit = posting.Credit,
                            Attachment = ""
                        };
                        model.Postings.Add(_posting);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckLicenses]
        public ActionResult Create(JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments)
        {
            
            //ViewBag.Accounts = getActiveAccounts();
            var validation = FinanceOperations.validatePostings(model.Postings);
            if (validation.hasError)
            {
                TempData["Error"] = validation.ErrorMessage;
            }
            else if (FinanceOperations.isClosedOrFrozenPeriodDate(model.Date))
            {
                TempData["Error"] = "Cannot post in dates which fall into closed or frozen fiscal periods.";
            }
            else if (!FinanceOperations.CreateNewJournal(model, attachments, Server, User.Identity.GetUserId()))
            {
                TempData["error"] = "An error occured while creating a journal entry. Operation failed!";
            }

            return RedirectToAction("Index");

        }

        public ActionResult CreateReverseEntry(int? id)
        {
            ViewBag.Accounts = getActiveAccounts();

            var model = new JournalEntryViewModel();
            if (id == null) // create empty model
            {
                model.Postings = new List<Posting>();
                model.Postings.Add(new Posting());

            }
            else // create a model that is copy of a journal with given id.
            {
                var journal = financeDb.Journals.Find(id);

                if (journal == null)
                {
                    return RedirectToAction("PageNotFound", "Error", new { area = "" });
                }

                model.Date = journal.Date;
                model.Memo = "Reverse Entry : " + journal.Memo;
                model.VoucherType = journal.VoucherType;
                model.Postings = new List<Posting>();
                using (FinanceDb db2 = new FinanceDb())
                {
                    var postings = db2.Postings
                        .Where(x => x.JournalId == journal.JournalId);

                    foreach (var posting in postings)
                    {
                        var account = new Account();
                        account.Title = posting.Account.Title;
                        account.AccountId = posting.AccountId;
                        account.AccountSubGroupId = posting.Account.AccountSubGroupId;
                        var _posting = new Posting
                        {
                            Account = account,
                            AccountId = account.AccountId,
                            Memo = posting.Memo,
                            Debit = posting.Credit,
                            Credit = posting.Debit
                        };
                        model.Postings.Add(_posting);
                    }
                }
            }
            return View("Create", model);
        }

        public ActionResult PayRequisition(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Requisition thisRequisition = requisitionDb.Requisitions.Find(id);
            if (thisRequisition == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }

            #region mark this notification as read
            var thisGuy = this.thisGuy;
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(thisRequisition.RequisitionId, thisGuy);
            Workflow.markNotificationAsRead(thisNotification.NotificationId);
            #endregion

            #region prepare model & supporting data for view
            var model = new JournalEntryViewModel();
            model.Postings = new List<Posting>();
            model.Postings.Add(new Posting());

            ViewBag.RequisitionId = thisRequisition.RequisitionId;
            TempData["info"] = "Total amount due for this requisition is " + @String.Format("{0:n}", thisRequisition.TotalAmountApproved);
            ViewBag.Accounts = getActiveAccounts();
            #endregion

            TempData["NotificationId"] = thisNotification.NotificationId; // post action method would need this value

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PayRequisition(JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, string requisitionId, string userAction)
        {
            #region validation checks
            if (!userAction.Equals("Request Reapproval"))
            {
                var validation = FinanceOperations.validateRequisitionPaymentPostings(model.Postings, Int32.Parse(requisitionId));
                if (validation.hasError)
                {
                    TempData["Error"] = validation.ErrorMessage;
                    ViewBag.Accounts = getActiveAccounts();
                    model = new JournalEntryViewModel();
                    model.Postings = new List<Posting>();
                    model.Postings.Add(new Posting());
                    ViewBag.RequisitionId = Int32.Parse(requisitionId);
                    return View(model);
                }
                if (FinanceOperations.isClosedOrFrozenPeriodDate(model.Date))
                {
                    TempData["Error"] = "Cannot post in dates which fall into closed or frozen fiscal periods.";
                    ViewBag.Accounts = getActiveAccounts();
                    model = new JournalEntryViewModel();
                    model.Postings = new List<Posting>();
                    model.Postings.Add(new Posting());
                    ViewBag.RequisitionId = Int32.Parse(requisitionId);
                    return View(model);
                }
            }
            #endregion

            Workflow.removeNotification((int)TempData["NotificationId"]);
            var thisRequisition = requisitionDb.Requisitions.Find(Int32.Parse(requisitionId));
            var thisGuy = this.thisGuy;

            if (userAction == "Pay for Procurement")
            {
                PayForProcurement(thisRequisition, model, attachments, thisGuy);

            }
            else if (userAction == "Pay as Liability")
            {
                PayAsLiability(thisRequisition, model, attachments, thisGuy);
            }
            else if (userAction == "Request Reapproval")
            {
                RequestReapproval(thisRequisition, model, thisGuy);
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult DiscountReturned(int? id)
        {
            #region validation checks
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Requisition thisRequisition = requisitionDb.Requisitions.Find(id);
            if (thisRequisition == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            #endregion

            #region mark this notification as read
            var thisGuy = this.thisGuy;
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(thisRequisition.RequisitionId, thisGuy);
            Workflow.markNotificationAsRead(thisNotification.NotificationId);
            var thisDiscount = requisitionDb.RequisitionDiscounts.Where(x => x.RequisitionId == id).First();
            #endregion

            #region prepare model for view
            var model = new JournalEntryViewModel();
            model.Postings = new List<Posting>();
            model.Postings.Add(new Posting());
            ViewBag.RequisitionId = thisRequisition.RequisitionId;
            TempData["info"] = "Total amount due for this requisition is " + @String.Format("{0:n}", thisRequisition.DiscountReturned);
            ViewBag.Accounts = getActiveAccounts();
            ViewBag.RequisitionId = thisRequisition.RequisitionId;
            #endregion

            TempData["DiscountId"] = thisDiscount.RequisitionDiscountId; // post action method would need this value

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can View Discounts")]
        public ActionResult DiscountReturned(JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments)
        {
            #region validation checks
            var thisDiscount = requisitionDb.RequisitionDiscounts.Find((int)TempData["DiscountId"]);
            var validation = FinanceOperations.validateDiscountCollectionPostings(model.Postings, thisDiscount.RequisitionDiscountId);
            if (validation.hasError)
            {
                TempData["Error"] = validation.ErrorMessage;
                ViewBag.Accounts = getActiveAccounts();
                model = new JournalEntryViewModel();
                model.Postings = new List<Posting>();
                model.Postings.Add(new Posting());
                TempData["DiscountId"] = thisDiscount.RequisitionDiscountId;
                return View(model);
            }
            if (FinanceOperations.isClosedOrFrozenPeriodDate(model.Date))
            {
                TempData["Error"] = "Cannot post in dates which fall into closed or frozen periods.";
                ViewBag.Accounts = getActiveAccounts();
                model = new JournalEntryViewModel();
                model.Postings = new List<Posting>();
                model.Postings.Add(new Posting());
                TempData["DiscountId"] = thisDiscount.RequisitionDiscountId;
                return View(model);
            }
            #endregion

            //create new journal entry
            var newDiscountReturnedEntry = Shared.FinanceOperations.CreateNewDiscountReturned(thisDiscount.RequisitionDiscountId, model, attachments, Server, User.Identity.GetUserId());

            // remove notification
            var thisRequisition = Workflow.getRequisitionForDiscount((int)TempData["DiscountId"]);
            var thisGuy = this.thisGuy;
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(thisRequisition.RequisitionId, thisGuy);
            Workflow.removeNotification(thisNotification.NotificationId);

            // mark discount as collected by finance
            Workflow.markDiscountAsCollected(thisDiscount.RequisitionDiscountId, thisGuy);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private void PayForProcurement(Requisition thisRequisition, JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, Employee thisGuy)
        {
            var nextGuy = Workflow.getLocalProcurementManagerFor(thisRequisition);
            var newPayment = FinanceOperations.CreateNewRequisitionPayment(thisRequisition.RequisitionId, model, attachments, Server, User.Identity.GetUserId());

            // update the status of this requisition
            thisRequisition.CurrentState = RequisitionStates.CashIssued;
            thisRequisition.pendingWith = nextGuy.EmployeeId;
            requisitionDb.SaveChanges();

            // create new event
            var newEvent = new RequisitionEvent
            {
                EventDate = DateTime.Now,
                EventState = RequisitionStates.CashIssued,
                Comments = String.Format("Cash Issued by {0}", thisGuy.Name),
                RequisitionId = thisRequisition.RequisitionId,
                PendingWith = thisRequisition.pendingWith,
                RespondedBy = thisGuy.EmployeeId
            };
            requisitionDb.RequisitionEvents.Add(newEvent);
            requisitionDb.SaveChanges();

            //add code to create inbox message for the next guy.
            var newNotification = new Notification
            {
                ExternalReferenceId = thisRequisition.RequisitionId,
                isRead = false,
                Date = DateTime.Now,
                Text = String.Format("Did you receive payment by {0} for Requisition # {1}: {2}", thisGuy.Name, thisRequisition.RequisitionId, thisRequisition.Description),
                URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId),
                Module = "Procurement",
                EmployeeId = nextGuy.EmployeeId
            };
            companyDb.Notifications.Add(newNotification);
            companyDb.SaveChanges();

            //send email to next guy
            Workflow.sendNotification(newNotification, nextGuy);
        }

        private void PayAsLiability(Requisition thisRequisition, JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, Employee thisGuy)
        {
            var nextGuy = Workflow.getRequisitionOwner(thisRequisition);
            var newPayment = Shared.FinanceOperations.CreateNewRequisitionPayment(thisRequisition.RequisitionId, model, attachments, Server, User.Identity.GetUserId());

            // update the status of this requisition
            thisRequisition.CurrentState = RequisitionStates.Closed;
            thisRequisition.pendingWith = nextGuy.EmployeeId;
            requisitionDb.SaveChanges();

            // create new event
            var newEvent = new RequisitionEvent
            {
                EventDate = DateTime.Now,
                EventState = RequisitionStates.CashIssued,
                Comments = String.Format("Your requistion # {0} ({1}) has been closed.", thisRequisition.RequisitionId, thisRequisition.Description),
                RequisitionId = thisRequisition.RequisitionId,
                PendingWith = thisRequisition.pendingWith,
                RespondedBy = thisGuy.EmployeeId
            };
            requisitionDb.RequisitionEvents.Add(newEvent);
            requisitionDb.SaveChanges();

            //add code to create inbox message for the next guy.
            var newEmployeeInboxMessage = new Notification
            {
                ExternalReferenceId = thisRequisition.RequisitionId,
                isRead = false,
                Date = DateTime.Now,
                Text = String.Format("Did you receive payment by {0} for Requisition # {1}: {2}", thisGuy.Name, thisRequisition.RequisitionId, thisRequisition.Description),
                URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId),
                Module = "Procurement",
                EmployeeId = nextGuy.EmployeeId
            };
            companyDb.Notifications.Add(newEmployeeInboxMessage);
            companyDb.SaveChanges();

            //send email to next guy
            Workflow.sendNotification(newEmployeeInboxMessage, nextGuy);
        }

        private void RequestReapproval(Requisition thisRequisition, JournalEntryViewModel model, Employee thisGuy)
        {
            // create new event
            var newEvent = new RequisitionEvent
            {
                Comments = model.Memo,
                EventDate = DateTime.Now,
                PendingWith = thisRequisition.pendingWith,
                RequisitionId = thisRequisition.RequisitionId,
                EventState = RequisitionStates.Reapproval,
                RespondedBy = thisGuy.EmployeeId,
            };
            requisitionDb.RequisitionEvents.Add(newEvent);
            requisitionDb.SaveChanges();

            // add a new notification to nextGuy's inbox.
            var nextGuy = Workflow.getRequisitionOwner(thisRequisition);
            var newEmployeeInboxMessage = new Notification
            {
                Date = DateTime.Now,
                isRead = false,
                EmployeeId = nextGuy.EmployeeId,
                ExternalReferenceId = thisRequisition.RequisitionId,
                URL = String.Format("/Procurement/Requisitions/Edit/{0}", thisRequisition.RequisitionId),
                Text = String.Format("{0} requested reapproval for: {1}", thisGuy.Name, thisRequisition.Description),
                Module = "Procurement"
            };
            companyDb.Notifications.Add(newEmployeeInboxMessage);
            companyDb.SaveChanges();

            Workflow.sendNotification(newEmployeeInboxMessage, nextGuy);
        }

        private IEnumerable<Account> getActiveAccounts()
        {
            return financeDb.Accounts
                    .Where(x => x.Status == AccountStatus.Active)
                    .OrderBy(x => x.Title);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                financeDb.Dispose();
                requisitionDb.Dispose();
                companyDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}