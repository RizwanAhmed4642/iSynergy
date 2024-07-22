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
    public class ProfitAndLossStatementController : CustomController
    {
        private FinanceDb db = new FinanceDb();
        // GET: Finance/BalanceSheet
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(AppSettings.getKey("COGS")))
            {
                TempData["info"] = @"Please define 'Cost of Goods Sold' in application settings.";
                return RedirectToAction("Index", "AppSettings", new {area = "" });
            }
            var model = new IncomeStatementViewModel();

            var fiscalStartDate = DateTime.Parse(AppSettings.getKey("FiscalStartDate"));
            var oldestPostingDate = (from posting in db.Postings
                                     select posting.Journal.Date).Min();
            model.fromDate = oldestPostingDate <= fiscalStartDate ? fiscalStartDate : oldestPostingDate;
            model.toDate = DateTime.Now;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IncomeStatementViewModel model)
        {
            model = Shared.FinanceOperations.getIncomeStatementModel(model.fromDate, model.toDate);

            model.printedBy = this.thisGuy.Name;
            if (model.Income.Count() == 0)
            {
                TempData["error"] = "No records found!";
            }
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public void Export(IncomeStatementViewModel model)
        //{
        //    model = Shared.FinanceOperations.getIncomeStatementModel(model.fromDate, model.toDate);
        //    model.printedBy = this.thisGuy.Name;
        //    var dt = CSV.listToDataTable(model.Income);
        //    CSV.WriteDataTableToCSV(dt, "IncomeStatement.csv");
        //}
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