using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using iSynergy.DataContexts;
using iSynergy.Areas.Finance.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    [AuthorizeRedirect(Roles ="Admin, Can Edit Bank Accounts")]
    public class BankAccountsController : CustomController
    {
        private FinanceDb db = new FinanceDb();
        private CompanyDb companyDb = new CompanyDb();

        // GET: BankAccounts
        public ActionResult Index()
        {
            var model = db.BankAccounts.Include(x => x.Bank).Include(x => x.Office);
            return View(model);
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name");
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankAccountId,AccountNumber,AccountTitle,BankId,OfficeId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                TempData["success"] = "Bank account added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name", bankAccount.BankId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name", bankAccount.BankId);
            var companyDb = new CompanyDb();
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location", bankAccount.OfficeId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BankAccountId,AccountNumber,AccountTitle,BankId,OfficeId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name", bankAccount.BankId);
            return View(bankAccount);
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
