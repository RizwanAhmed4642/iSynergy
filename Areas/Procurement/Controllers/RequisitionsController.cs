using iSynergy.Areas.HR.Models;
using iSynergy.Areas.Procurement.Models;
using iSynergy.Controllers;
using iSynergy.DataContexts;
using iSynergy.Models;
using iSynergy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iSynergy.Areas.Procurement.Controllers
{
    public class RequisitionsController : CustomController
    {
        private RequisitionDb requisitionDb = new RequisitionDb();
        private CompanyDb companyDb = new CompanyDb();
        private IdentityDb identityDb = new IdentityDb();
        private FinanceDb financeDb = new FinanceDb();

        // GET: Notifications
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can View Requisitions")]
        public ActionResult Notifications()
        {
            Employee thisGuy = this.thisGuy;
            ViewBag.InboxMessages = companyDb.Notifications
                                    .Where(eim => eim.EmployeeId == thisGuy.EmployeeId &&
                                            eim.isRead == false)
                                    .OrderByDescending(eim => eim.Date);
            return View("_MyInboxPartial");
        }

        /// <summary>
        /// GET: Requisitions
        /// this method passes all the pending requisitions of the currenyly logged-in guy.
        /// </summary>
        /// <returns></returns>
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can View Requisitions")]
        public ActionResult Index()
        {
            // get the employee id of the currently logged-in guy
            int myEmpId = this.thisGuy.EmployeeId;

            // get a list of all the pending requisitions of the currenyly logged-in guy
            var model = requisitionDb.Requisitions
                            .Where(r => r.EmployeeId == myEmpId &&
                            r.CurrentState != RequisitionStates.Closed);

            ViewBag.Title = "My Requistions";
            return View(model);
        }
        // GET: Requisitions/Create
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions")]
        [CheckLicenses]
        public ActionResult Create()
        {
            var model = new RequisitionViewModel();
            return View(model);
        }
        /// <summary>
        /// This method updates Requisitions, RequistionItems, RequisitionQuotations, RequisitionEvnets
        /// and EmployeeInboxMessages repositories upon creation of a new requisition.
        /// </summary>
        /// <param name="model">Requisition data</param>
        /// <param name="itemFiles">images of item files uploaded</param>
        /// <param name="quotationFiles">images of quotatinos uploaded</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions")]
        [CheckLicenses]
        public ActionResult Create(RequisitionViewModel model, IEnumerable<HttpPostedFileBase> itemFiles, IEnumerable<HttpPostedFileBase> quotationFiles)
        {
            
            var items = itemFiles.ToList(); // get a list of uploaded item images
            var quotations = quotationFiles == null ? null : quotationFiles.ToList(); // get a list of uploaded quotation images

            #region Save attachments
            List<string> itemFilesPaths = FileOperations.SaveAttachmentsOnServer(itemFiles, Server);
            List<string> quotationFilesPaths = FileOperations.SaveAttachmentsOnServer(quotationFiles, Server);
            #endregion

            #region create new requisition 
            var today = System.DateTime.Now;
            var newRequisition = new Requisition();
            Employee thisGuy = this.thisGuy;
            newRequisition.creationDate = today;
            newRequisition.Description = model.requisitionDescription;
            newRequisition.CurrentState = RequisitionStates.Requested;
            newRequisition.EmployeeId = thisGuy.EmployeeId;
            requisitionDb.Requisitions.Add(newRequisition);
            requisitionDb.SaveChanges(); // this is important to generate the Id for new requisition.
            #endregion

            #region save requisition items
            if (model.requisitionItems.Count() > 0)
            {

                for (var index = 0; index < model.requisitionItems.Count(); index++)
                {
                    var newItem = new RequisitionItem();
                    newItem.ItemName = model.requisitionItems[index].ItemName;
                    newItem.Quantity = model.requisitionItems[index].Quantity;
                    newItem.UnitPrice = model.requisitionItems[index].UnitPrice;
                    newItem.RequisitionId = newRequisition.RequisitionId;
                    if (items[index] != null)
                    {
                        newItem.File = itemFilesPaths.ElementAt(index);
                    }
                    requisitionDb.RequisitionItems.Add(newItem);
                }
            }
            #endregion

            #region save requisition quotations
            if (model.requisitionQuotations != null)
            {
                if (model.requisitionQuotations.Count() > 0)
                {
                    for (var index = 0; index < model.requisitionQuotations.Count(); index++)
                    {
                        var newQuotation = new RequisitionQuotation();
                        newQuotation.SupplierName = model.requisitionQuotations[index].SupplierName;
                        newQuotation.QuotationPrice = model.requisitionQuotations[index].QuotationPrice;
                        newQuotation.RequisitionId = newRequisition.RequisitionId;
                        requisitionDb.RequisitionQuotations.Add(newQuotation);
                        if (quotations[index] != null)
                        {
                            newQuotation.File = quotationFilesPaths.ElementAt(index);
                        }
                        requisitionDb.RequisitionQuotations.Add(newQuotation);
                    }
                }
            }
            #endregion

            #region identify the total amount requested for the requistion
            var itemsTotal = model.requisitionItems.Sum(i => (i.UnitPrice * i.Quantity));
            var totalQuotations = 0;
            if (model.requisitionQuotations != null) // some quotations are attached
            {
                totalQuotations = model.requisitionQuotations.Count();
            }

            if (totalQuotations > 0) // if there are some quotations attached, give them priority and pick the minimum price offered
            {
                newRequisition.TotalAmountRequested = model.requisitionQuotations.Min(q => q.QuotationPrice);
            }
            else // if no quotations attached, just use the total of items price or zero if null.
            {
                newRequisition.TotalAmountRequested = itemsTotal ?? 0; //assign 0 if null is returned.
            }
            #endregion

            #region decide who will respond next
            Employee nextGuy = new Employee();
            if (Workflow.isLocalOperationsHead(thisGuy))
            {
                nextGuy = Workflow.getCoo();
            }
            else if (Workflow.isLocalHod(thisGuy))
            {
                nextGuy = Workflow.getLocalOperationsHeadFor(newRequisition);
            }
            else
            {
                nextGuy = Workflow.getNextGuy(thisGuy);
            }

            newRequisition.pendingWith = nextGuy.EmployeeId;
            requisitionDb.SaveChanges();
            #endregion

            #region create a requisition history event
            var newRequisitionEvent = new RequisitionEvent();
            newRequisitionEvent.EventDate = today;
            newRequisitionEvent.EventState = RequisitionStates.Requested;
            newRequisitionEvent.RespondedBy = thisGuy.EmployeeId;
            newRequisitionEvent.Comments = String.Format("submitted to {0} for approval.", nextGuy.Name);
            newRequisitionEvent.RequisitionId = newRequisition.RequisitionId;
            newRequisitionEvent.PendingWith = nextGuy.EmployeeId;
            requisitionDb.RequisitionEvents.Add(newRequisitionEvent);
            #endregion

            #region send inbox message to next guy
            var newNotification = new Notification();
            newNotification.Date = today;
            newNotification.Text = String.Format("Requisition {0}: {1} needs, {2}", newRequisition.RequisitionId, thisGuy.Name, newRequisition.Description);
            newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", newRequisition.RequisitionId);
            newNotification.isRead = false;
            newNotification.EmployeeId = nextGuy.EmployeeId;
            newNotification.ExternalReferenceId = newRequisition.RequisitionId;
            newNotification.Module = "Procurement";
            companyDb.Notifications.Add(newNotification);
            companyDb.SaveChanges();
            #endregion

            //send email to next guy
            Workflow.sendNotification(newNotification, nextGuy);

            // commit changes to database
            requisitionDb.SaveChanges();

            TempData["success"] = "Requisition created and submitted!";
            return RedirectToAction("Index", "Requisitions", new { area = "Procurement" });
        }

        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can Approve Requisitions")]
        //GET: Requisitions/Review/5
        public ActionResult Review(int? id)
        {
            #region Validation checks
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

            #region create model (data for view)
            var model = new RequisitionReviewViewModel();
            model.requisition = thisRequisition;
            model.requisitionItems = requisitionDb.RequisitionItems.Where(ri => ri.RequisitionId == thisRequisition.RequisitionId);
            model.requisitionQuotations = requisitionDb.RequisitionQuotations.Where(rq => rq.RequisitionId == thisRequisition.RequisitionId)
                                                                                .OrderBy(rq => rq.QuotationPrice);
            model.requisitionEvents = requisitionDb.RequisitionEvents.Where(re => re.RequisitionId == thisRequisition.RequisitionId);
            model.requisitionBills = requisitionDb.RequistionBills.Where(rb => rb.RequisitionId == thisRequisition.RequisitionId);
            ViewBag.Quotations = requisitionDb.RequisitionQuotations
                                                            .Where(rq => rq.RequisitionId == thisRequisition.RequisitionId)
                                                            .OrderBy(rq => rq.QuotationPrice)
                                                            .AsEnumerable()
                                                            .ToList()
                                                            .Select(
                                                                        rq => new {
                                                                            QuotationId = rq.RequisitionQuotationId,
                                                                            QuotationName = string.Format("{0} ({1})", rq.SupplierName, rq.QuotationPrice)
                                                                        }
                                                                    );
            #endregion

            #region send a data dictionary in model to display names of employees instead of their IDs.
            model.employeeIdToNameConverter = new EmployeeIdtoNameDictonary(); 
            model.employeeIdToNameConverter.Add(model.requisition.pendingWith, companyDb);
            foreach (RequisitionEvent eventItem in model.requisitionEvents)
            {
                model.employeeIdToNameConverter.Add(eventItem.RespondedBy, companyDb);
            }
            #endregion

            #region instruct the view to show or hide the buttons
            var lastEvent = model.requisitionEvents.LastOrDefault();
            /*
            This requisition should open in read only mode if and only if
            this requisition is not pending with the current user
            or this requisition has a status of FinalApproved, Rejectd or Closed.
            */
            if (lastEvent == null 
                || lastEvent.PendingWith != this.thisGuy.EmployeeId    
                || lastEvent.EventState == RequisitionStates.FinalApproved
                || lastEvent.EventState == RequisitionStates.Rejected
                || lastEvent.EventState == RequisitionStates.Closed)
            {
                model.readOnlyAccess = true;
            }
            else
            {
                model.readOnlyAccess = false;
            }
            #endregion

            #region let the view know who just reviewed
            Employee thisGuy = this.thisGuy;

            if ((Workflow.getLocalOperationsHeadFor(thisRequisition).EmployeeId == thisGuy.EmployeeId))
            {
                model.isOperationsHeadReviewing = true;
            }
            else if (Workflow.getCoo().EmployeeId == thisGuy.EmployeeId)
            {
                model.isCooReviewing = true;
            }
            else if (Workflow.getLocalFinanceHeadFor(thisRequisition).EmployeeId == thisGuy.EmployeeId)
            {
                model.isFinanceManagerReviewing = true;
            }
            else if (Workflow.getLocalProcurementManagerFor(thisRequisition).EmployeeId == thisGuy.EmployeeId)
            {
                model.isProcurementManagerReviewing = true;
            }

            if (Workflow.getRequisitionOwner(thisRequisition).EmployeeId == thisGuy.EmployeeId)
            {
                model.isInitiatorReviewing = true;
            }
            #endregion

            #region mark this notification as read
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(thisRequisition.RequisitionId, thisGuy);
            if (thisNotification != null)
            {
                Workflow.markNotificationAsRead(thisNotification.NotificationId);
                TempData["NotificationId"] = thisNotification.NotificationId; // post action method would need this value
            }
            #endregion

            return View(model);
        }
        //POST: Requisitions/Review/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can Approve Requisitions")]
        public ActionResult Review(string eventAction, RequisitionReviewViewModel model)
        {

            RequisitionEvent newRequistionEvent = new RequisitionEvent();
            Requisition thisRequisition = requisitionDb.Requisitions.Find(model.newEvent.RequisitionId);
            Employee thisGuy = this.thisGuy;
            Employee nextGuy = Workflow.getNextGuy(thisGuy);
            DateTime today = System.DateTime.Now;
            Notification newNotification = new Notification();

            #region set the requisition amount based on the selected quotation
            if (Workflow.isCoo(thisGuy) || Workflow.isLocalOperationsHead(thisGuy))
            {
                if (model.selectedQuotationId != 0)
                {
                    var selectedQuotaion = requisitionDb.RequisitionQuotations.Find(model.selectedQuotationId);
                    thisRequisition.TotalAmountApproved = selectedQuotaion.QuotationPrice;
                }
                else
                {
                    thisRequisition.TotalAmountApproved = thisRequisition.TotalAmountRequested;
                }
                requisitionDb.SaveChanges();

            }
            #endregion

            #region decide what to do next based on buttons clicked by current reviewer
            if (eventAction.Equals("Approved"))
            {

                // if finaly approved by operations head or COO
                // then send it finance
                if (Workflow.isLocalOperationsHead(thisGuy) || Workflow.isCoo(thisGuy))
                {
                    // format the message link to land on payment advice page for finance
                    newNotification.URL = String.Format("/Finance/JournalEntries/PayRequisition/{0}", thisRequisition.RequisitionId);
                    newNotification.Text = String.Format("Payment advice for requisition {0}: {1}", thisRequisition.RequisitionId, thisRequisition.Description);

                    newRequistionEvent.EventState = RequisitionStates.FinalApproved;
                    requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.FinalApproved;
                    nextGuy = Workflow.getLocalFinanceHeadFor(thisRequisition);

                }
                else //else send it to nextGuy for approval
                {
                    if (Workflow.isLocalHod(thisGuy))
                    {
                        nextGuy = Workflow.getLocalOperationsHeadFor(thisRequisition);
                    }
                    // format the message link to land on requisition review page for managers
                    newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                    newNotification.Text = String.Format("Requisition {0}: {1} needs {2}", thisRequisition.RequisitionId, thisRequisition.Employee.Name, thisRequisition.Description);

                    newRequistionEvent.EventState = RequisitionStates.ManagerApproved;
                    requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.ManagerApproved;
                }
            }
            else if (eventAction.Equals("Rejected"))
            {
                // format the message link to land on requisition review page for initiator
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.Text = String.Format("{0} REJECTED: {1}", thisGuy.Name, thisRequisition.Description);

                // mark it rejected
                newRequistionEvent.EventState = RequisitionStates.Rejected;
                requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.Rejected;

                // send the intimation to requester
                nextGuy = Workflow.getRequisitionOwner(thisRequisition);

            }
            else if (eventAction.Equals("RequestQuotations"))
            {
                // format the message link to land on requisition review page for initiator
                newNotification.URL = String.Format("/Procurement/Requisitions/AddQuotations/{0}", thisRequisition.RequisitionId);
                newNotification.Text = String.Format("Quotations needed for Requisition {0}: {1}", thisRequisition.RequisitionId, thisRequisition.Description);

                // mark it for quotations needed
                newRequistionEvent.EventState = RequisitionStates.QuotationsNeeded;
                requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.QuotationsNeeded;

                // send the intimation to local procurement manager.
                nextGuy = Workflow.getLocalProcurementManagerFor(thisRequisition);
            }
            else if (eventAction.Equals("ForwardToCoo") && Workflow.isLocalOperationsHead(thisGuy))
            {
                // format the message link to land on requisition review page for managers
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.Text = String.Format("{0} needs, {1}", thisRequisition.Employee.Name, thisRequisition.Description);


                newRequistionEvent.EventState = RequisitionStates.PendingForCoo;
                requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.PendingForCoo;

                nextGuy = Workflow.getCoo();
            }
            else if (eventAction.Equals("CashReceived") && Workflow.isLocalProcurementManager(thisGuy))
            {
                // format the message link to land on requisition review page for managers
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.Text = String.Format("Update procurement status for Requisition # {0}: {1}", thisRequisition.RequisitionId, thisRequisition.Description);


                newRequistionEvent.EventState = RequisitionStates.CashReceived;
                requisitionDb.Entry(thisRequisition).Entity.CurrentState = RequisitionStates.CashReceived;

                nextGuy = thisGuy;
            }
            else if (eventAction.Equals("Delivered") && Workflow.isLocalProcurementManager(thisGuy))
            {
                // save attaced bill(s)
                List<string> billsFilesPaths = FileOperations.SaveAttachmentsOnServer(model.Bills, Server);

                if (model.Bills.Count() > 0)
                {
                    for (int index = 0; index < model.Bills.Count(); index++)
                    {
                        var newBill = new RequisitionBill
                        {
                            File = billsFilesPaths.ElementAt(index),
                            Requisition = thisRequisition
                        };
                        requisitionDb.RequistionBills.Add(newBill);
                    }

                    //add code to send notification to finance to view the attached bill(s)
                    // format the message link to land on requisition review page for managers
                    newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                    newNotification.Text = String.Format("Bill submitted by {0} for Requisition # {1}: {2}", thisGuy.Name, thisRequisition.RequisitionId, thisRequisition.Description);

                    nextGuy = Workflow.getLocalFinanceHeadFor(thisRequisition);


                    // add a new notification to nextGuy's inbox.
                    newNotification.Date = today;
                    newNotification.isRead = false;
                    newNotification.EmployeeId = nextGuy.EmployeeId;
                    newNotification.ExternalReferenceId = thisRequisition.RequisitionId;
                    newNotification.Module = "Procurement";
                    companyDb.Notifications.Add(newNotification);
                    companyDb.SaveChanges();
                }

                // code to handle discount returned (if any)
                if ((thisRequisition.TotalAmountApproved - model.requisition.PurchaseAmount) > 0)
                {
                    thisRequisition.PurchaseAmount = model.requisition.PurchaseAmount;
                    thisRequisition.DiscountReturned = thisRequisition.TotalAmountApproved - model.requisition.PurchaseAmount;
                    requisitionDb.SaveChanges();

                    var newDiscount = new RequisitionDiscount
                    {
                        Amount = thisRequisition.TotalAmountApproved - model.requisition.PurchaseAmount,
                        ReceivedById = Workflow.getLocalFinanceHeadFor(thisRequisition).EmployeeId,
                        OfficeId = thisRequisition.Employee.Office.OfficeId,
                        isCollectedByFinance = false,
                        RequisitionId = thisRequisition.RequisitionId
                    };

                    requisitionDb.RequisitionDiscounts.Add(newDiscount);
                    requisitionDb.SaveChanges();

                    // format the message link to land on requisition review page for managers
                    newNotification.URL = String.Format("/Finance/JournalEntries/DiscountReturned/{0}", thisRequisition.RequisitionId);
                    newNotification.Text = String.Format("Acknowledge the receiving of discount returned by {0} for Requisition # {1}: {2}", thisGuy.Name, thisRequisition.RequisitionId, thisRequisition.Description);

                    nextGuy = Workflow.getLocalFinanceHeadFor(thisRequisition);


                    // add a new notification to nextGuy's inbox.
                    newNotification.Date = today;
                    newNotification.isRead = false;
                    newNotification.EmployeeId = nextGuy.EmployeeId;
                    newNotification.ExternalReferenceId = thisRequisition.RequisitionId;
                    newNotification.Module = "Procurement";
                    companyDb.Notifications.Add(newNotification);
                    companyDb.SaveChanges();
                    Workflow.sendNotification(newNotification, nextGuy);
                }
                else
                {
                    requisitionDb.Entry(thisRequisition).Entity.PurchaseAmount = thisRequisition.TotalAmountIssued;
                }

                newRequistionEvent.EventState = RequisitionStates.Delivered;
                thisRequisition.CurrentState = RequisitionStates.Delivered;


                requisitionDb.SaveChanges();


                // format the message link to land on requisition review page for managers
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.Text = String.Format("Update delivery status for your Requisition # {0}: {1}", thisRequisition.RequisitionId, thisRequisition.Description);

                nextGuy = thisRequisition.Employee;
            }
            else if (eventAction.Equals("Received") && (thisRequisition.EmployeeId == thisGuy.EmployeeId))
            {
                newRequistionEvent.EventState = RequisitionStates.Closed;
                thisRequisition.CurrentState = RequisitionStates.Closed;

                requisitionDb.SaveChanges();

                // creat new event in database.
                newRequistionEvent.EventDate = today;
                newRequistionEvent.Comments = model.newEvent.Comments;
                newRequistionEvent.RespondedBy = thisGuy.EmployeeId;
                newRequistionEvent.PendingWith = nextGuy.EmployeeId;
                newRequistionEvent.RequisitionId = thisRequisition.RequisitionId;
                newNotification.Module = "Procurement";
                requisitionDb.RequisitionEvents.Add(newRequistionEvent);
                requisitionDb.SaveChanges(); // save changes

                // remove old notification from thisGuy's inbox.
                var oldMessages = companyDb.Notifications.
                    Where(eim => eim.EmployeeId == thisGuy.EmployeeId &&
                    eim.ExternalReferenceId == thisRequisition.RequisitionId);
                // this loop will delete two messages. 
                // first message about viewing the bill. 
                // second messsage about delivery confirmation
                // the purpose is to avoid deleting the 3rd message which is about discount return confirmation
                foreach (var msg in oldMessages)
                {
                    if (msg.Text.Contains("Update delivery status for your Requisition") || msg.Text.Contains("Bill submitted by"))
                    {
                        companyDb.Notifications.Remove(msg);
                    }

                }

                //oldMessage.isRead = true;

                // save changes
                companyDb.SaveChanges();


                //TODO: send an email to nextGuyUrl

                return Redirect("~/Home");
            }
            thisRequisition.pendingWith = nextGuy.EmployeeId;
            requisitionDb.SaveChanges();
            #endregion

            #region create new requisition history event
            newRequistionEvent.EventDate = today;
            newRequistionEvent.Comments = model.newEvent.Comments;
            newRequistionEvent.RespondedBy = thisGuy.EmployeeId;
            newRequistionEvent.PendingWith = nextGuy.EmployeeId;
            newRequistionEvent.RequisitionId = thisRequisition.RequisitionId;
            requisitionDb.RequisitionEvents.Add(newRequistionEvent);
            requisitionDb.SaveChanges(); // save changes
            #endregion

            #region send a new notification to nextGuy's inbox.
            newNotification.Date = today;
            newNotification.isRead = false;
            newNotification.EmployeeId = nextGuy.EmployeeId;
            newNotification.ExternalReferenceId = thisRequisition.RequisitionId;
            newNotification.Module = "Procurement";
            companyDb.Notifications.Add(newNotification);
            companyDb.SaveChanges();
            #endregion

            Workflow.removeNotification((int)TempData["NotificationId"]);

            //send email to next guy
            Workflow.sendNotification(newNotification, nextGuy);

            return Redirect("~/Home");
        }

        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can Approve Requisitions")]
        public ActionResult AddQuotations(int? id)
        {
            var thisGuy = this.thisGuy;

            #region create  model (data for view)
            var model = new RequisitionReviewViewModel();
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            model.requisition = requisitionDb.Requisitions.Find(id);
            if (model.requisition == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            model.requisitionItems = requisitionDb.RequisitionItems.Where(ri => ri.RequisitionId == model.requisition.RequisitionId);
            model.requisitionQuotations = requisitionDb.RequisitionQuotations.Where(rq => rq.RequisitionId == model.requisition.RequisitionId);
            model.requisitionEvents = requisitionDb.RequisitionEvents.Where(re => re.RequisitionId == model.requisition.RequisitionId);
            model.employeeIdToNameConverter = new EmployeeIdtoNameDictonary(); 
            model.employeeIdToNameConverter.Add(model.requisition.pendingWith, companyDb);
            #endregion

            #region send a data dictionary in model to display names of employees instead of their IDs
            foreach (RequisitionEvent eventItem in model.requisitionEvents)
            {
                model.employeeIdToNameConverter.Add(eventItem.RespondedBy, companyDb);
            }
            #endregion

            #region mark this notification as Read
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(model.requisition.RequisitionId, thisGuy);
            Workflow.markNotificationAsRead(thisNotification.NotificationId);
            TempData["NotificationId"] = thisNotification.NotificationId; // post action method would need this value
            #endregion

            #region instruct the view to show/hide buttons
            var lastEvent = model.requisitionEvents.LastOrDefault();
            // if this requistion is not pending with the currently logged in guy
            if (lastEvent == null || lastEvent.PendingWith != thisGuy.EmployeeId)
            {
                model.readOnlyAccess = true;
                // take him to a readonly details review page instead.
                //TempData["error"] = "You do not have permission to respond to this requistion. You can only view";
                return View(model);
            }
            model.readOnlyAccess = false;
            #endregion

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions, Can Approve Requisitions")]
        public ActionResult AddQuotations(RequisitionReviewViewModel model, IEnumerable<HttpPostedFileBase> quotationFiles)
        {
            var quotations = quotationFiles == null ? null : quotationFiles.ToList();

            #region save uploaded quotations
            List<string> quotationFilePaths = FileOperations.SaveAttachmentsOnServer(quotationFiles, Server);
            #endregion

            if (model.requisitionQuotations != null)
            {
                #region save quotations in DB
                var modelQuotations = model.requisitionQuotations.ToList();
                var thisRequisition = requisitionDb.Requisitions.Find(model.newEvent.RequisitionId);
                if (model.requisitionQuotations.Count() > 0)
                {
                    for (var index = 0; index < model.requisitionQuotations.Count(); index++)
                    {
                        var newQuotation = new RequisitionQuotation();
                        newQuotation.SupplierName = modelQuotations[index].SupplierName;
                        newQuotation.QuotationPrice = modelQuotations[index].QuotationPrice;
                        newQuotation.File = quotationFilePaths.ElementAt(index);
                        newQuotation.RequisitionId = thisRequisition.RequisitionId;
                        requisitionDb.RequisitionQuotations.Add(newQuotation);
                    }
                }
                #endregion

                #region Save new event
                var today = System.DateTime.Now;
                Employee thisGuy = this.thisGuy;
                Employee nextGuy = Workflow.getLocalOperationsHeadFor(thisRequisition);
                var newRequisitionEvent = new RequisitionEvent();
                newRequisitionEvent.EventDate = today;
                newRequisitionEvent.EventState = RequisitionStates.QuotationsAttached;
                newRequisitionEvent.RespondedBy = thisGuy.EmployeeId;
                newRequisitionEvent.Comments = String.Format("{0} attached quotations and sent to {1}", thisGuy.Name, nextGuy.Name);
                newRequisitionEvent.RequisitionId = thisRequisition.RequisitionId;
                newRequisitionEvent.PendingWith = nextGuy.EmployeeId;
                requisitionDb.RequisitionEvents.Add(newRequisitionEvent);
                requisitionDb.SaveChanges();
                #endregion

                #region also update the Amount Requested
                var allQuotations = requisitionDb.RequisitionQuotations.Where(q => q.RequisitionId == thisRequisition.RequisitionId);
                thisRequisition.TotalAmountRequested = allQuotations.Min(q => q.QuotationPrice);
                requisitionDb.SaveChanges();
                #endregion

                #region send a message to next guy's inbox
                Notification newNotification = new Notification();
                newNotification.Date = today;
                newNotification.Text = String.Format("{0} attached quotations for Requistion {1}: {2}.", thisGuy.Name, thisRequisition.RequisitionId, thisRequisition.Description);
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.isRead = false;
                newNotification.EmployeeId = nextGuy.EmployeeId;
                newNotification.ExternalReferenceId = thisRequisition.RequisitionId;
                newNotification.Module = "Procurement";
                companyDb.Notifications.Add(newNotification);
                companyDb.SaveChanges();
                #endregion

                Workflow.removeNotification((int)TempData["NotificationId"]);

                //send email to next guy
                Workflow.sendNotification(newNotification, nextGuy);

                return Redirect("~/Home");
            }
            return View(model);
        }


        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var thisRequisition = requisitionDb.Requisitions.Find(id);
            if (thisRequisition == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            var model = new RequisitionViewModel();
            model.RequisitionId = thisRequisition.RequisitionId;
            model.requisitionDescription = thisRequisition.Description;
            model.requisitionItems = requisitionDb.RequisitionItems
                                    .Where(x => x.RequisitionId == thisRequisition.RequisitionId)
                                    .ToList();
            model.requisitionQuotations = requisitionDb.RequisitionQuotations
                                    .Where(x => x.RequisitionId == thisRequisition.RequisitionId)
                                    .ToList();

            var thisGuy = this.thisGuy;
            var thisNotification = Workflow.getNotificationFromRequisitionForThisGuy(thisRequisition.RequisitionId, thisGuy);

            if (thisNotification != null) // it will be only NULL when a power user wants to edit the requisition in an attempt to correct the data mistakes.
            {
                TempData["NotificationId"] = thisNotification.NotificationId; // post action method would need this value
            }
           

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Create Requisitions")]
        public ActionResult Edit(RequisitionViewModel model, IEnumerable<HttpPostedFileBase> itemFiles, IEnumerable<HttpPostedFileBase> quotationFiles)
        {
            var items = itemFiles.ToList(); // get a list of uploaded item images
            var quotations = quotationFiles == null ? null : quotationFiles.ToList(); // get a list of uploaded quotation images

            #region Save attachments
            List<string> itemFilesPaths = FileOperations.SaveAttachmentsOnServer(itemFiles, Server);
            List<string> quotationFilesPaths = FileOperations.SaveAttachmentsOnServer(quotationFiles, Server);
            #endregion

            #region edit requisition 
            var today = System.DateTime.Now;
            var thisRequisition = requisitionDb.Requisitions.Find(model.RequisitionId);
            Employee thisGuy = this.thisGuy;
            //thisRequisition.creationDate = today;
            thisRequisition.Description = model.requisitionDescription;
            thisRequisition.CurrentState = RequisitionStates.Requested;
            thisRequisition.EmployeeId = thisGuy.EmployeeId;
            requisitionDb.SaveChanges(); 
            #endregion

            #region save requisition items
            if (model.requisitionItems.Count() > 0)
            {

                for (var index = 0; index < model.requisitionItems.Count(); index++)
                {
                    var thisItem = requisitionDb.RequisitionItems.Find(model.requisitionItems[index].RequisitionItemId);
                    thisItem.ItemName = model.requisitionItems[index].ItemName;
                    thisItem.Quantity = model.requisitionItems[index].Quantity;
                    thisItem.UnitPrice = model.requisitionItems[index].UnitPrice;
                    thisItem.RequisitionId = thisRequisition.RequisitionId;
                    if (items[index] != null)
                    {
                        if (!string.IsNullOrEmpty(itemFilesPaths.ElementAt(index)))
                        {
                            thisItem.File = itemFilesPaths.ElementAt(index);
                        }
                        
                    }
                    requisitionDb.SaveChanges();
                }
            }
            #endregion

            #region save requisition quotations
            if (model.requisitionQuotations != null)
            {
                if (model.requisitionQuotations.Count() > 0)
                {
                    for (var index = 0; index < model.requisitionQuotations.Count(); index++)
                    {
                        var thisQuotation = requisitionDb.RequisitionQuotations.Find(model.requisitionQuotations[index].RequisitionQuotationId);
                        thisQuotation.SupplierName = model.requisitionQuotations[index].SupplierName;
                        thisQuotation.QuotationPrice = model.requisitionQuotations[index].QuotationPrice;
                        thisQuotation.RequisitionId = thisRequisition.RequisitionId;
                        if (quotations[index] != null)
                        {
                            if (!string.IsNullOrEmpty(quotationFilesPaths.ElementAt(index)))
                            {
                                thisQuotation.File = quotationFilesPaths.ElementAt(index);
                            }
                            
                        }
                        requisitionDb.SaveChanges();
                    }
                }
            }
            #endregion

            #region identify the total amount requested for the requistion
            var itemsTotal = model.requisitionItems.Sum(i => (i.UnitPrice * i.Quantity));
            var totalQuotations = 0;
            if (model.requisitionQuotations != null)
            {
                totalQuotations = model.requisitionQuotations.Count();
            }

            if (totalQuotations > 0) // if there are some quotations attached, give them priority and pick the minimum price offered
            {
                thisRequisition.TotalAmountRequested = model.requisitionQuotations.Min(q => q.QuotationPrice);
            }
            else // if no quotations attached, just use the total of items price or zero if null.
            {
                thisRequisition.TotalAmountRequested = itemsTotal ?? 0; //assign 0 if null is returned.
            }
            #endregion

            #region decide who will respond next
            Employee nextGuy = new Employee();
            if (Workflow.isLocalOperationsHead(thisGuy))
            {
                nextGuy = Workflow.getCoo();
            }
            else if (Workflow.isLocalHod(thisGuy))
            {
                nextGuy = Workflow.getLocalOperationsHeadFor(thisRequisition);
            }
            else
            {
                nextGuy = Workflow.getNextGuy(thisGuy);
            }

            thisRequisition.pendingWith = nextGuy.EmployeeId;
            requisitionDb.SaveChanges();
            #endregion

            #region create a requisition history event
            var newRequisitionEvent = new RequisitionEvent();
            newRequisitionEvent.EventDate = today;
            newRequisitionEvent.EventState = RequisitionStates.Requested;
            newRequisitionEvent.RespondedBy = thisGuy.EmployeeId;
            newRequisitionEvent.Comments = String.Format("submitted to {0} for re-approval.", nextGuy.Name);
            newRequisitionEvent.RequisitionId = thisRequisition.RequisitionId;
            newRequisitionEvent.PendingWith = nextGuy.EmployeeId;
            requisitionDb.RequisitionEvents.Add(newRequisitionEvent);
            #endregion

            if (TempData["NotificationId"] != null) // it will be only NULL when a power user edited the requisition in an attempt to correct the data mistakes.
            {
                Workflow.removeNotification((int)TempData["NotificationId"]);

                #region send inbox message to next guy
                var newNotification = new Notification();
                newNotification.Date = today;
                newNotification.Text = String.Format("Requisition {0}: {1} needs, {2}", thisRequisition.RequisitionId, thisGuy.Name, thisRequisition.Description);
                newNotification.URL = String.Format("/Procurement/Requisitions/Review/{0}", thisRequisition.RequisitionId);
                newNotification.isRead = false;
                newNotification.EmployeeId = nextGuy.EmployeeId;
                newNotification.ExternalReferenceId = thisRequisition.RequisitionId;
                newNotification.Module = "Procurement";
                companyDb.Notifications.Add(newNotification);
                companyDb.SaveChanges();
                #endregion

                //send email to next guy
                Workflow.sendNotification(newNotification, nextGuy);

            }

            

            // commit changes to database
            requisitionDb.SaveChanges();

            TempData["success"] = "Requisition saved and submitted!";
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeRedirect(Roles = "Admin, Can Search Requisitions")]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Search Requisitions")]
        public ActionResult Search(int? requisitionId, DateTime? fromDate, DateTime? toDate, string description, string requestedBy, string pendingWith)
        {
            IEnumerable<Requisition> allRequisitions = requisitionDb.Requisitions;

            if (requisitionId != null)
            {
                allRequisitions = (from x in allRequisitions
                                   where x.RequisitionId == requisitionId
                                   select x).ToList();
            }

            if (fromDate != null)
            {
                allRequisitions = (from x in allRequisitions
                                   where x.creationDate >= fromDate
                                   select x).ToList();
            }

            if (toDate != null)
            {
                allRequisitions = (from x in allRequisitions
                                   where x.creationDate <= toDate
                                   select x).ToList();
            }

            if (!String.IsNullOrEmpty(description))
            {
                allRequisitions = (from x in allRequisitions
                                   where x.Description.ToLower().Contains(description.ToLower().Trim())
                                   select x).ToList();
            }

            if (!String.IsNullOrEmpty(requestedBy))
            {
                allRequisitions = (from x in allRequisitions
                                   where x.Employee.Name.ToLower().Contains(requestedBy.ToLower().Trim())
                                   select x).ToList();
            }

            if (!String.IsNullOrEmpty(pendingWith))
            {
                Employee approver = companyDb.Employees.Where(x => x.Name.ToLower().Contains(pendingWith.ToLower().Trim())).FirstOrDefault();
                if (approver != null)
                {
                    allRequisitions = (from x in allRequisitions
                                       where x.pendingWith == approver.EmployeeId
                                       select x).ToList();
                }
                
            }

            ViewBag.Title = "Search Result";
            return View("Index", allRequisitions);
        }


        [AuthorizeRedirect(Roles = "Admin")]
        public ActionResult PowerEdit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            var thisRequisition = requisitionDb.Requisitions.Find(id);
            if (thisRequisition == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            var model = new RequisitionViewModel();
            model.RequisitionId = thisRequisition.RequisitionId;
            model.requisitionDescription = thisRequisition.Description;
            model.requisitionItems = requisitionDb.RequisitionItems
                                    .Where(x => x.RequisitionId == thisRequisition.RequisitionId)
                                    .ToList();
            model.requisitionQuotations = requisitionDb.RequisitionQuotations
                                    .Where(x => x.RequisitionId == thisRequisition.RequisitionId)
                                    .ToList();
            model.requisitionBill = requisitionDb.RequistionBills
                                    .Where(x => x.RequisitionId == thisRequisition.RequisitionId)
                                    .FirstOrDefault();
            var thisGuy = this.thisGuy;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin")]
        public ActionResult PowerEdit(
                                        RequisitionViewModel model, 
                                        IEnumerable<HttpPostedFileBase> itemFiles, 
                                        IEnumerable<HttpPostedFileBase> quotationFiles,
                                        HttpPostedFileBase requisitionBillFile)
        {
            var items = itemFiles.ToList(); // get a list of uploaded item images
            var quotations = quotationFiles == null ? null : quotationFiles.ToList(); // get a list of uploaded quotation images

            #region Save attachments
            List<string> itemFilesPaths = FileOperations.SaveAttachmentsOnServer(itemFiles, Server);
            List<string> quotationFilesPaths = FileOperations.SaveAttachmentsOnServer(quotationFiles, Server);
            var billFilePath = FileOperations.SaveFile(requisitionBillFile, Server);
            #endregion

            #region edit requisition 
            var today = System.DateTime.Now;
            var thisRequisition = requisitionDb.Requisitions.Find(model.RequisitionId);
            Employee thisGuy = this.thisGuy;
            //thisRequisition.creationDate = today;
            thisRequisition.Description = model.requisitionDescription;
            //thisRequisition.CurrentState = RequisitionStates.Requested;  // do not change the state in powerEdit mode.
            thisRequisition.EmployeeId = thisGuy.EmployeeId;
            requisitionDb.SaveChanges();
            #endregion

            #region save requisition items
            if (model.requisitionItems.Count() > 0)
            {

                for (var index = 0; index < model.requisitionItems.Count(); index++)
                {
                    var thisItem = requisitionDb.RequisitionItems.Find(model.requisitionItems[index].RequisitionItemId);
                    thisItem.ItemName = model.requisitionItems[index].ItemName;
                    thisItem.Quantity = model.requisitionItems[index].Quantity;
                    thisItem.UnitPrice = model.requisitionItems[index].UnitPrice;
                    thisItem.RequisitionId = thisRequisition.RequisitionId;
                    if (items[index] != null)
                    {
                        if (!string.IsNullOrEmpty(itemFilesPaths.ElementAt(index)))
                        {
                            thisItem.File = itemFilesPaths.ElementAt(index);
                        }
                        
                    }
                    requisitionDb.SaveChanges();
                }
            }
            #endregion

            #region save requisition quotations
            if (model.requisitionQuotations != null)
            {
                if (model.requisitionQuotations.Count() > 0)
                {
                    for (var index = 0; index < model.requisitionQuotations.Count(); index++)
                    {
                        var thisQuotation = requisitionDb.RequisitionQuotations.Find(model.requisitionQuotations[index].RequisitionQuotationId);
                        thisQuotation.SupplierName = model.requisitionQuotations[index].SupplierName;
                        thisQuotation.QuotationPrice = model.requisitionQuotations[index].QuotationPrice;
                        thisQuotation.RequisitionId = thisRequisition.RequisitionId;
                        if (quotations[index] != null)
                        {
                            if (!string.IsNullOrEmpty(quotationFilesPaths.ElementAt(index)))
                            {
                                thisQuotation.File = quotationFilesPaths.ElementAt(index);
                            }
                            
                        }
                        requisitionDb.SaveChanges();
                    }
                }
            }
            #endregion

            #region save bill
            var thisBill = requisitionDb.RequistionBills.Find(model.requisitionBill.RequisitionBillId);
            if (thisBill != null)
            {
                if (!string.IsNullOrEmpty(billFilePath))
                {
                    thisBill.File = billFilePath;
                    requisitionDb.SaveChanges();
                }
                
            }
            
            #endregion

            #region identify the total amount requested for the requistion
            var itemsTotal = model.requisitionItems.Sum(i => (i.UnitPrice * i.Quantity));
            var totalQuotations = 0;
            if (model.requisitionQuotations != null)
            {
                totalQuotations = model.requisitionQuotations.Count();
            }

            if (totalQuotations > 0) // if there are some quotations attached, give them priority and pick the minimum price offered
            {
                thisRequisition.TotalAmountRequested = model.requisitionQuotations.Min(q => q.QuotationPrice);
            }
            else // if no quotations attached, just use the total of items price or zero if null.
            {
                thisRequisition.TotalAmountRequested = itemsTotal ?? 0; //assign 0 if null is returned.
            }
            #endregion

            // commit changes to database
            requisitionDb.SaveChanges();

            TempData["success"] = "Requisition updated!";
            return RedirectToAction("Index", "Home");
        }


    }
}