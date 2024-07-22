using iSynergy.Areas.Finance.Models;
using iSynergy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iSynergy.DataContexts;

namespace iSynergy.Areas.Finance.Shared
{
    public static class FinanceOperations
    {
        public static RequisitionPayment CreateNewRequisitionPayment(int RequisitionId, JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, HttpServerUtilityBase server, string IdentityUserId)
        {
            using (FinanceDb db = new FinanceDb())
            {
                List<string> files = FileOperations.SaveAttachmentsOnServer(attachments, server);

                RequisitionPayment requisitionPayment = new RequisitionPayment
                {
                    Date = model.Date,
                    Memo = model.Memo,
                    PostedById = Workflow.getLoggedInEmployee(IdentityUserId).EmployeeId,
                    RequisitionId = RequisitionId,
                    VoucherType = (VoucherType)model.VoucherType
                };

                db.RequisitionPayments.Add(requisitionPayment);
                db.SaveChanges();
                requisitionPayment.Reference = requisitionPayment.VoucherType.ToString() + "-" + requisitionPayment.JournalId.ToString();
                createNewPostings((Journal)requisitionPayment, model.Postings, files);

                return requisitionPayment;
            }
        }
        public static DiscountReturned CreateNewDiscountReturned(int RequisitionDiscountId, JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, HttpServerUtilityBase server, string IdentityUserId)
        {
            using (FinanceDb db = new FinanceDb())
            {
                List<string> files = FileOperations.SaveAttachmentsOnServer(attachments, server);

                DiscountReturned discountReturned = new DiscountReturned
                {
                    Date = model.Date,
                    Memo = model.Memo,
                    PostedById = Workflow.getLoggedInEmployee(IdentityUserId).EmployeeId,
                    RequisitionDiscountId = RequisitionDiscountId,
                    VoucherType = (VoucherType)model.VoucherType
                };

                db.DiscountReturns.Add(discountReturned);
                db.SaveChanges();
                discountReturned.Reference = discountReturned.VoucherType.ToString() + "-" + discountReturned.JournalId.ToString();
                db.SaveChanges();

                createNewPostings((Journal)discountReturned, model.Postings, files);

                return discountReturned;
            }
        }
        public static bool CreateNewJournal(JournalEntryViewModel model, IEnumerable<HttpPostedFileBase> attachments, HttpServerUtilityBase server, string IdentityUserId)
        {
            using (FinanceDb db = new FinanceDb())
            {
                List<string> files = FileOperations.SaveAttachmentsOnServer(attachments, server);

                Journal journal = new Journal
                {
                    Date = model.Date,
                    Memo = model.Memo,
                    PostedById = Workflow.getLoggedInEmployee(IdentityUserId).EmployeeId,
                    VoucherType = (VoucherType)model.VoucherType
                };
                db.Journals.Add(journal);
                db.SaveChanges();
                journal.Reference = journal.VoucherType.ToString() + "-" + journal.JournalId.ToString();
                db.SaveChanges();
                try
                {
                    createNewPostings(journal, model.Postings, files);
                }
                catch (Exception)
                {

                    db.Journals.Remove(journal);
                    db.SaveChanges();
                    return false;
                }


                return true;
            }
        }
        public static void createNewPostings(Journal journal, List<Posting> postings, List<string> files)
        {
            using (FinanceDb db = new FinanceDb())
            {
                var index = 0;
                foreach (var posting in postings)
                {
                    Posting newPosting = new Posting
                    {
                        AccountId = posting.AccountId,
                        Debit = posting.Debit == null ? 0 : posting.Debit,
                        Credit = posting.Credit == null ? 0 : posting.Credit,
                        Memo = posting.Memo,
                        JournalId = journal.JournalId,
                        Attachment = files.Count == 0 ? null : (String.IsNullOrWhiteSpace(files.ElementAt(index)) ? null : files.ElementAt(index))
                    };
                    db.Postings.Add(newPosting);
                    index++;
                }
                db.SaveChanges();
            }
        }
        public static jvValidationData validatePostings(List<Posting> Postings)
        {
            decimal CreditsTotal = (decimal)Postings.Sum(x => x.Credit);
            decimal DebitsTotal = (decimal)Postings.Sum(x => x.Debit); ;
            var result = new jvValidationData();
            foreach (var posting in Postings)
            {

                if (posting.Credit == null && posting.Debit == null)
                {
                    result.ErrorMessage = "At least of the debit or credit fields must have a valid value.";
                    result.hasError = true;
                }
                else if (posting.Credit != null && posting.Debit != null)
                {
                    if (!((posting.Credit != null && posting.Debit == 0) || (posting.Credit == 0 && posting.Debit != null)))
                    {
                        result.ErrorMessage = "You cannot fill both the debit and credit fileds in a row. Only one of debit or credit fields can have a value.";
                        result.hasError = true;
                    }

                }
            }
            if (DebitsTotal != CreditsTotal)
            {
                result.ErrorMessage = "Sum of all the debits must be equal to sum of all the credits.";
                result.hasError = true;
            }

            return result;
        }
        public static jvValidationData validateRequisitionPaymentPostings(List<Posting> Postings, int requisitionId)
        {
            var result = validatePostings(Postings);
            using (RequisitionDb db = new RequisitionDb())
            {
                var requisition = db.Requisitions.Find(requisitionId);
                var totalDebit = (decimal)Postings.Sum(x => x.Debit);

                if (totalDebit != requisition.TotalAmountApproved)
                {
                    result.ErrorMessage = "Total paid amount must be equal to " + String.Format("{0:n}", requisition.TotalAmountApproved);
                    result.hasError = true;
                }

                return result;
            }
        }
        public static jvValidationData validateDiscountCollectionPostings(List<Posting> Postings, int discountId)
        {
            var result = validatePostings(Postings);
            using (RequisitionDb db = new RequisitionDb())
            {
                var discount = db.RequisitionDiscounts.Find(discountId);
                var totalDebit = (decimal)Postings.Sum(x => x.Debit);

                if (totalDebit != discount.Amount)
                {
                    result.ErrorMessage = "Total paid amount must be equal to " + String.Format("{0:n}", discount.Amount);
                    result.hasError = true;
                }

                return result;
            }
        }
        public static bool isClosedOrFrozenPeriodDate(DateTime date)
        {
            using (FinanceDb db = new FinanceDb())
            {
                return db.FiscalPeriods
                        .Where(x => x.StartDate <= date && x.EndDate >= date && x.Status != FiscalPeriodStatus.Open)
                        .Any();
            }
        }
        public static void AddFiscalPeriodsIfRequired()
        {
            using (FinanceDb db = new FinanceDb())
            {
                var overlappingPeriods = db.FiscalPeriods
                                            .Where(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today);

                if (!overlappingPeriods.Any())
                {
                    var periods = AppSettings.hasKey("FiscalPeriods") ? Int32.Parse(AppSettings.getKey("FiscalPeriods")) : 1;
                    var startDate = AppSettings.hasKey("FiscalStartDate")
                                    ? DateTime.Parse(AppSettings.getKey("FiscalStartDate"))
                                    : new DateTime(
                                                    DateTime.Today.Month > 7 ? DateTime.Today.Year : DateTime.Today.Year - 1
                                                    , 7
                                                    , 1
                                                );
                    var endDate = startDate.AddMonths(12).AddDays(-1);

                    if (endDate < DateTime.Today) // configured period has expired. 
                    {
                        startDate = endDate.AddDays(1);
                        AppSettings.setKey("FiscalStartDate", startDate.ToString());
                    }
                    var model = getFiscalCalendarModal();

                    // add new periods starting from new start date
                    var singlePeriodDuration = 12 / model.Periods;
                    for (int i = 0; i < model.Periods; i++)
                    {

                        var period = new FiscalPeriod
                        {
                            FiscalYear = model.NewStartDate.Year,
                            StartDate = model.NewStartDate.AddMonths(singlePeriodDuration * i),
                            EndDate = model.NewStartDate.AddMonths((singlePeriodDuration * i) + singlePeriodDuration).AddDays(-1)
                        };
                        db.FiscalPeriods.Add(period);
                        db.SaveChanges();
                    }
                    AppSettings.setKey("FiscalPeriods", model.Periods.ToString());
                    AppSettings.setKey("FiscalStartDate", model.NewStartDate.ToString());
                }
            }
        }
        public static Decimal? getAccountOpeningDebitBalance(Account account, DateTime date)
        {
            using (FinanceDb db = new FinanceDb())
            {
                var postings = from posting in db.Postings
                               where posting.Account.AccountId == account.AccountId && posting.Journal.Date < date
                               select posting;
                return postings.Sum(x => x.Debit);
            }
        }
        public static Decimal? getAccountOpeningCreditBalance(Account account, DateTime date)
        {
            using (FinanceDb db = new FinanceDb())
            {
                var postings = from posting in db.Postings
                               where posting.Account.AccountId == account.AccountId && posting.Journal.Date < date
                               select posting;
                return postings.Sum(x => x.Credit);
            }
        }

        public static FiscalCalendarViewModel getFiscalCalendarModal()
        {
            using (FinanceDb db = new FinanceDb())
            {
                var model = new FiscalCalendarViewModel();


                if (fiscalCalendarExists())
                {

                    // get a list of all open peroids. 
                    model.OpenPeriods = db.FiscalPeriods
                                        .Where(x => x.Status == FiscalPeriodStatus.Open)
                                        .ToList();

                    var oldestOpenPeriod = model.OpenPeriods.LastOrDefault();

                    model.Periods = Int32.Parse(AppSettings.getKey("FiscalPeriods"));
                    model.StartDate = DateTime.Parse(AppSettings.getKey("FiscalStartDate"));
                    model.NewStartDate = oldestOpenPeriod == null ? model.StartDate : oldestOpenPeriod.EndDate.AddDays(1);

                    // get a list of all frozen peroids. 
                    model.FrozenPeriods = db.FiscalPeriods
                                        .Where(x => x.Status == FiscalPeriodStatus.Frozen)
                                        .ToList();

                    var latestFrozenOrClosedPeriods = from period in db.FiscalPeriods
                                                      where period.Status != FiscalPeriodStatus.Open
                                                      select period.EndDate;

                    model.LatestFrozenOrClosedPeriodEndDate = latestFrozenOrClosedPeriods.Count() == 0 ? model.StartDate : latestFrozenOrClosedPeriods.Max().AddDays(1);
                }

                return model;
            }

        }
        public static bool fiscalCalendarExists()
        {
            return AppSettings.hasKey("FiscalPeriods") && AppSettings.hasKey("FiscalStartDate");
        }

        public static IncomeStatementViewModel getIncomeStatementModel(DateTime fromDate, DateTime toDate)
        {
            using (FinanceDb db = new FinanceDb())
            {
                var startingYear = fromDate.Year;
                var endingYear = toDate.Year;


                List<string> creditNormalAccounts = new List<string>() { "Income", "Equity", "Liabilities" };
                var postings = from posting in db.Postings
                               join accountSubGroup in db.AccountSubGroups on posting.Account.AccountSubGroupId equals accountSubGroup.AccountSubGroupId
                               join accountGroup in db.AccountGroups on accountSubGroup.AccountGroupId equals accountGroup.AccountGroupId
                               join accountClass in db.AccountClasses on accountGroup.AccountClassId equals accountClass.AccountClassId
                               where posting.Journal.Date >= fromDate && posting.Journal.Date <= toDate
                               select new
                               {
                                   AccountClass = accountClass,
                                   AccountGroup = accountGroup,
                                   AccountSubGroup = accountSubGroup,
                                   Account = posting.Account,
                                   Balance = creditNormalAccounts.Contains(accountClass.Title) ? posting.Credit - posting.Debit : posting.Debit - posting.Credit,
                                   Year = posting.Journal.Date.Year
                               };

                var records = from posting in postings
                              group posting by new { posting.AccountSubGroup, posting.Year } into g
                              select new YearlyBalanceViewModel
                              {
                                  AccountClass = g.FirstOrDefault().AccountClass,
                                  AccountGroup = g.FirstOrDefault().AccountGroup,
                                  AccountSubGroup = g.FirstOrDefault().AccountSubGroup,
                                  Balance = g.Sum(x => x.Balance),
                                  Year = g.FirstOrDefault().Year
                              };

                var income = (from record in records
                              where record.AccountClass.Title == "Income"
                              select record)
                             .ToList();

                var cogsSetting = AppSettings.getKey("COGS");

                var cogs = (from record in records
                            where record.AccountGroup.AccountGroupId == cogsSetting
                            select record)
                           .ToList();

                var expense = (from record in records
                               where record.AccountClass.Title == "Expense" && record.AccountGroup.AccountGroupId != cogsSetting
                               select record)
                              .ToList();

                return new IncomeStatementViewModel(fromDate, toDate, startingYear, endingYear, income, cogs, expense);
            }

        }

    }
    public class jvValidationData
    {
        public jvValidationData()
        {
            hasError = false;
        }
        public bool hasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}