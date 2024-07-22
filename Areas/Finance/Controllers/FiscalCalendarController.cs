using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.Shared;
using iSynergy.Areas.Finance.Models;
using iSynergy.DataContexts;
using iSynergy.Areas.Finance.Shared;
using iSynergy.Controllers;

namespace iSynergy.Areas.Finance.Controllers
{
    public class FiscalCalendarController : CustomController
    {
        FinanceDb db = new FinanceDb();
        // GET: Finance/FiscalCalendar
        public ActionResult Index()
        {
            var items = new List<SelectListItem>();
            var model = FinanceOperations.getFiscalCalendarModal();
            for (int i = 12; i >= 1; i--)
            {
                var item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                item.Selected = model.Periods == i ? true : false;
                items.Add(item);
            }
            ViewBag.listItems = items;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FiscalCalendarViewModel model)
        {
            // show error if new start date overlaps with closed or frozen periods. 
            var closedPeriods = db.FiscalPeriods
                                    .Where(x => x.Status == FiscalPeriodStatus.Closed || x.Status == FiscalPeriodStatus.Frozen)
                                    .ToList();

            var overlapingClosedPeriod = (from period in closedPeriods
                                          where period.StartDate <= model.NewStartDate && period.EndDate >= model.NewStartDate
                                          select period).FirstOrDefault();

            if (overlapingClosedPeriod == null)
            {
                // remove all open periods
                var openPeriods = db.FiscalPeriods
                                        .Where(x => x.Status == FiscalPeriodStatus.Open)
                                        .ToList();
                if (openPeriods != null)
                {
                    db.FiscalPeriods.RemoveRange(openPeriods);
                }
                db.SaveChanges();

                // add new periods starting from end date of last frozen or closed period and ending one day before new start date
                if (closedPeriods.Count > 0)
                {
                    var lastClosedPeriods = (from periods in closedPeriods
                                             orderby periods.FiscalPeriodId descending
                                             select periods).FirstOrDefault();
                    if ((model.NewStartDate - lastClosedPeriods.EndDate).Days > 1)
                    {
                        var newPeriod = new FiscalPeriod
                        {
                            FiscalYear = model.NewStartDate.Year,
                            StartDate = lastClosedPeriods.EndDate,
                            EndDate = model.NewStartDate.AddDays(-1)
                        };
                        db.FiscalPeriods.Add(newPeriod);
                        db.SaveChanges();
                    }
                }

                // add new period starting from new start date
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


                db.SaveChanges();
                AppSettings.setKey("FiscalPeriods", model.Periods.ToString());
                AppSettings.setKey("FiscalStartDate", model.NewStartDate.ToString());
                TempData["success"] = "Fiscal calendar updated!";
            }
            else
            {
                TempData["error"] = "Cannot have a start date that falls into a closed or frozen period.";
            }

            // prepare model
            var newModel = FinanceOperations.getFiscalCalendarModal();
            var items = new List<SelectListItem>();
            for (int i = 12; i >= 1; i--)
            {
                var item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                item.Selected = newModel.Periods == i ? true : false;
                items.Add(item);
            }
            ViewBag.listItems = items;
            return View(newModel);
        }

        

        public ActionResult ClosePeriod(int id)
        {
            var period = db.FiscalPeriods.Find(id);
            if (period != null)
            {
                period.Status = FiscalPeriodStatus.Closed;
                db.SaveChanges();
                TempData["success"] = String.Format("Period starting from {0} and ending at {1} has been closed.", period.StartDate.ToShortDateString(), period.EndDate.ToShortDateString());
            }
            return RedirectToAction("Index");
        }
        public ActionResult FreezePeriod(int id)
        {
            var period = db.FiscalPeriods.Find(id);
            if (period != null)
            {
                period.Status = FiscalPeriodStatus.Frozen;
                db.SaveChanges();
                TempData["success"] = String.Format("Period starting from {0} and ending at {1} has been frozen.", period.StartDate.ToShortDateString(), period.EndDate.ToShortDateString());
            }
            return RedirectToAction("Index");
        }
        public ActionResult ReOpenPeriod(int id)
        {
            var period = db.FiscalPeriods.Find(id);
            if (period != null)
            {
                if (period.Status != FiscalPeriodStatus.Frozen)
                {
                    TempData["error"] = String.Format("Period starting from {0} and ending at {1} is not frozen.", period.StartDate.ToShortDateString(), period.EndDate.ToShortDateString());
                }
                else
                {
                    period.Status = FiscalPeriodStatus.Open;
                    db.SaveChanges();
                    TempData["success"] = String.Format("Period starting from {0} and ending at {1} has been re opened.", period.StartDate.ToShortDateString(), period.EndDate.ToShortDateString());
                }
            }
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