using iSynergy.Areas.HelpDesk.Models;
using iSynergy.Areas.HelpDesk.ViewModel;
using iSynergy.DataContexts;
using iSynergy.Models;
using iSynergy.Shared;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace iSynergy.Areas.HelpDesk.Controllers
{
    public class ServiceRequestController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private FullDb db = new FullDb();
        public ServiceRequestController()
        {
        }

        public ServiceRequestController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            try
            {

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
                ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name");
                ServiceRequestViewModel model = new ServiceRequestViewModel();
                //ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location");
                //ViewBag.ManagerId = new SelectList(db.Employees, "EmployeeId", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        //[CheckLicenses]
        public ActionResult Create(ServiceRequestViewModel model)
        {

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name");
            if (ModelState.IsValid)
            {
                try
                {
                    string myid = "";
                    if (Session["userId"].ToString() != "")
                    {
                        myid = Session["userId"].ToString();
                    }

                    ServiceRequest _entity = new ServiceRequest();
                    model.Attachment = FileOperations.SaveFile(model.Files, Server);
                    _entity.Subject = model.Subject;
                    
                
                    _entity.status = status.Pending.ToString();
                    _entity.Priorty = model.Priorty;
                    _entity.CreateDate = DateTime.Now;

                    var _serviceCategories = db.ServiceCategories.Find(Convert.ToInt32(model.ServicesCategoryId));

                    _entity.ServicesCategory = _serviceCategories.Name;
                    _entity.ServicesCategoryId = _serviceCategories.Id;

                    var department = db.Departments.Find(Convert.ToInt32(model.DepartmentId));
                    _entity.Department = department.Name;
                    _entity.DepartmentId = department.DepartmentId;
                    _entity.UserId = myid;

                    _entity.HODId = department.HodId;
                    _entity.Attachment = model.Attachment;
                    _entity.Description = model.Description;
                    db.ServiceRequests.Add(_entity);
                    db.SaveChanges();
                    IdentityDb dbs = new IdentityDb();
               
                    _entity.UserId = Session["userId"].ToString();

                    var user = dbs.Users.Find(_entity.UserId);



                    var _msgForAdmin = "<div><h1> Thankyou for Ticket </h1 ><h4>We will contact you  as soon as possible </h4><div>";
                    EmailSend _emailsend = new EmailSend();

                    _emailsend.FromEmailAddress = ConfigurationManager.AppSettings["From_Email"];

                    var _email = ConfigurationManager.AppSettings["MENAFATF_Email"];
                    var _pass = ConfigurationManager.AppSettings["From_Email_Password"];
                    var _dispName = "Help Desk";
                    MailMessage mymessage = new MailMessage();
                    mymessage.To.Add(user.Email);
                    mymessage.From = new MailAddress(_email, _dispName);
                    mymessage.Subject = "Request Submitted";
                    mymessage.Body = _msgForAdmin;
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = ConfigurationManager.AppSettings["host"];
                        smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(_email, _pass);

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(mymessage);

                    }
                    var Hod = db.Employees.Find(_entity.HODId);




                    var _msgForHod = "<div><h1> New Request </h1 ><h4>You have recived new request. </h4><div>";
                    EmailSend _emailsendforHOd = new EmailSend();

                    _emailsend.FromEmailAddress = ConfigurationManager.AppSettings["From_Email"];

                    var _emailHod = ConfigurationManager.AppSettings["MENAFATF_Email"];
                    var _passHod = ConfigurationManager.AppSettings["From_Email_Password"];
                    var _dispNameHod = "Help Desk";
                    MailMessage mymessageHod = new MailMessage();
                    mymessage.To.Add(Hod.Email);
                    mymessage.From = new MailAddress(_emailHod, _dispNameHod);
                    mymessage.Subject = "New Request";
                    mymessage.Body = _msgForHod;
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = ConfigurationManager.AppSettings["host"];
                        smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(_emailHod, _passHod);

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(mymessage);

                    }




                    TempData["Msg"] = "Request  added successfully!";
                    return RedirectToAction("MyRequest");
                }
                catch (Exception ex)
                {
                    var Massage = ex.Message;
                    TempData["Msg"] = "Opps" + Massage;
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                    return View(model);
                }

            }
            else
            {
                var Massage = ModelState.IsValid.ToString();
                TempData["Msg"] = "Opps" + Massage;
                return View(model);
            }
        }

        public ActionResult AllRequestForMe()
        {

            var hodid = Convert.ToInt32(Session["empId"].ToString());
            var RequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid).ToList();
            var _resultModel = new List<ServiceRequestViewModel>();

            foreach (var item in RequestForMe)
            {
                var _model = new ServiceRequestViewModel();

                _model.Subject = item.Subject;
                _model.UserId = item.UserId;
                _model.status = item.status;
                _model.Priorty = item.Priorty;
                _model.CreateDate = item.CreateDate;


                _model.ServicesCategory = item.ServicesCategory;
                _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                _model.Department = item.Department;
                _model.DepartmentId = item.DepartmentId.ToString();

                _model.HODId = item.HODId;
                _model.Attachment = item.Attachment;
                _model.Description = item.Description;
                _resultModel.Add(_model);


            }
            IdentityDb dbs = new IdentityDb();
            var user = dbs.Users.ToList();



            if (_resultModel.Count > 0)
            {
                for (int i = 0; i < _resultModel.Count(); i++)
                {
                    foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                    {
                        _resultModel[i].Email = item.Email;
                    }
                }
            }
            return View(_resultModel);



        }
        public ActionResult MyRequest()
        {
            try
            {
                string myid = "";
                if (Session["userId"].ToString() != "")
                {
                    myid = Session["userId"].ToString();
                }
                var myRequest = db.ServiceRequests.Where(x => x.UserId == myid).ToList();

                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in myRequest)
                {
                    var _model = new ServiceRequestViewModel();

                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.CreateDate = item.CreateDate;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult PendingRequestForMe()
        {
            try
            {


                var hodid = Convert.ToInt32(Session["empId"].ToString());
                var RequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid && x.status == status.Pending.ToString()).ToList();
                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in RequestForMe)
                {
                    var _model = new ServiceRequestViewModel();
                    _model.Id = item.Id;
                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.CreateDate = item.CreateDate;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult AssignedRequestToEmployees()
        {
            try
            {


                var hodid = Convert.ToInt32(Session["empId"].ToString());
                var RequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid && x.status == status.Inprogress.ToString()).ToList();
                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in RequestForMe)
                {
                    var _model = new ServiceRequestViewModel();
                    _model.Id = item.Id;
                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.CreateDate = item.CreateDate;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult AssignRequest(int id)
        {
            try
            {


                var _request = db.ServiceRequests.Find(id);
                var _employees = db.Employees.Where(x => x.DepartmentId == _request.DepartmentId).ToList();
                ViewBag.Employees = new SelectList(_employees, "EmployeeId", "Name");
                ServiceRequestViewModel model = new ServiceRequestViewModel();
                model.Attachment = _request.Attachment;
                model.Subject = _request.Subject;
                model.UserId = _request.UserId;
                model.status = _request.status;
                model.Priorty = _request.Priorty;
                model.CreateDate = _request.CreateDate;


                model.ServicesCategory = _request.ServicesCategory;
                model.ServicesCategoryId = _request.ServicesCategoryId.ToString();

                model.Department = _request.Department;
                model.DepartmentId = _request.DepartmentId.ToString();

                model.HODId = _request.HODId;
                model.Attachment = _request.Attachment;
                model.Description = _request.Description;
                model.DueDate = _request.DueDate;




                return View(model);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        //[CheckLicenses]
        public ActionResult AssignRequest(ServiceRequestViewModel model)
        {

            var _request = db.ServiceRequests.Find(model.Id);
            var _employees = db.Employees.Where(x => x.DepartmentId == _request.DepartmentId).ToList();
            ViewBag.Employees = new SelectList(_employees, "EmployeeId", "Name");
            if (ModelState.IsValid)
            {
                try
                {
                    var _entity = db.ServiceRequests.Find(model.Id);



                    var _emp = db.Employees.Find(Convert.ToInt32(model.AssignedEmpId));
                    _entity.AssignedEmpId = Convert.ToInt32(model.AssignedEmpId);
                    _entity.AssignedEmpName = _emp.Name;
                    _entity.status = status.Inprogress.ToString();
                    _entity.AssignedDate = DateTime.Now;
                    _entity.DueDate = model.DueDate;
                    _entity.HodComments = model.HodComments;


                    db.Entry(_entity).State = EntityState.Modified;
                    db.SaveChanges();


                    var emp = db.Employees.Find(_entity.AssignedEmpId);
                    var _msgForAdmin = "<div><h1> Hi "+  _entity.AssignedEmpName  + " </h1 ><h4>You have recived new task</h4><div>";
                    EmailSend _emailsend = new EmailSend();

                    _emailsend.FromEmailAddress = ConfigurationManager.AppSettings["From_Email"];

                    var _email = ConfigurationManager.AppSettings["MENAFATF_Email"];
                    var _pass = ConfigurationManager.AppSettings["From_Email_Password"];
                    var _dispName = "Help Desk";
                    MailMessage mymessage = new MailMessage();
                    mymessage.To.Add(emp.Email);
                    mymessage.From = new MailAddress(_email, _dispName);
                    mymessage.Subject = "Assigned Task";
                    mymessage.Body = _msgForAdmin;
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = ConfigurationManager.AppSettings["host"];
                        smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(_email, _pass);

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(mymessage);

                    }
                    TempData["Msg"] = "Assigned request successfully! ";
                    return RedirectToAction("PendingRequestForMe");

                }
                catch (Exception ex)
                {
                    var Massage = ex.Message;
                    TempData["Msg"] = "Opps" + Massage;
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                    return View(model);
                }




                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                return View(model);
            }


            return View(model);
        }
        [HttpGet]
        public ActionResult MyTask()
        {
            try
            {


                var _empid = Convert.ToInt32(Session["empId"].ToString());
                var RequestForMe = db.ServiceRequests.Where(x => x.AssignedEmpId == _empid && x.status == status.Inprogress.ToString()).ToList();
                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in RequestForMe)
                {
                    var _model = new ServiceRequestViewModel();
                    _model.Id = item.Id;
                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.AssignedDate = item.AssignedDate;
                    _model.DueDate = item.DueDate;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult ProgressAssignRequest(int id)
        {
            try
            {
                var _request = db.ServiceRequests.Find(id);

                ServiceRequestViewModel model = new ServiceRequestViewModel();
                model.Attachment = _request.Attachment;
                model.Subject = _request.Subject;
                model.UserId = _request.UserId;
                model.status = _request.status;
                model.Priorty = _request.Priorty;
                model.CreateDate = _request.CreateDate;


                model.ServicesCategory = _request.ServicesCategory;
                model.ServicesCategoryId = _request.ServicesCategoryId.ToString();

                model.Department = _request.Department;
                model.DepartmentId = _request.DepartmentId.ToString();

                model.HODId = _request.HODId;
                model.Attachment = _request.Attachment;
                model.Description = _request.Description;
                model.DueDate = _request.DueDate;
                model.HodComments = _request.HodComments;




                return View(model);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        //[CheckLicenses]
        public ActionResult ProgressAssignRequest(ServiceRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _entity = db.ServiceRequests.Find(model.Id);

                    _entity.status = status.Done.ToString();


                    db.Entry(_entity).State = EntityState.Modified;
                    db.SaveChanges();
                    IdentityDb dbs = new IdentityDb();
                    var emp = dbs.Users.Find(_entity.UserId);




                    var _msgForAdmin = "<div><h1> Hi " + emp.UserName + " </h1 ><h4>You Ticket has been done</h4><div>";
                    EmailSend _emailsend = new EmailSend();

                    _emailsend.FromEmailAddress = ConfigurationManager.AppSettings["From_Email"];

                    var _email = ConfigurationManager.AppSettings["MENAFATF_Email"];
                    var _pass = ConfigurationManager.AppSettings["From_Email_Password"];
                    var _dispName = "Help Desk";
                    MailMessage mymessage = new MailMessage();
                    mymessage.To.Add(emp.Email);
                    mymessage.From = new MailAddress(_email, _dispName);
                    mymessage.Subject = "Ticket Closed";
                    mymessage.Body = _msgForAdmin;
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = ConfigurationManager.AppSettings["host"];
                        smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(_email, _pass);

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(mymessage);

                    }


                    TempData["Msg"] = "Task Completed successfully! ";

                    return RedirectToAction("MyTask");

                }
                catch (Exception ex)
                {
                    var Massage = ex.Message;
                    TempData["Msg"] = "Opps" + Massage;
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                    return View(model);
                }



                TempData["Msg"] = "Request  added successfully!";
                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                return View(model);
            }


            return View(model);
        }
        public ActionResult CompletedMyTask()
        {
            try
            {
                var _empid = Convert.ToInt32(Session["empId"].ToString());
                var RequestForMe = db.ServiceRequests.Where(x => x.AssignedEmpId == _empid && x.status == status.Done.ToString()).ToList();
                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in RequestForMe)
                {
                    var _model = new ServiceRequestViewModel();
                    _model.Id = item.Id;
                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.AssignedDate = item.AssignedDate;
                    _model.DueDate = item.DueDate;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);

            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public ActionResult RequestBackToHod(string EmployeeComment, string ReqId)
        {

            int Id = Convert.ToInt32(ReqId);
            var _entity = db.ServiceRequests.Find(Id);
            try
            {



                _entity.status = status.RollBack.ToString();
                _entity.EmolyeeComments = EmployeeComment;


                db.Entry(_entity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Msg"] = "Request send back successfully! ";
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("MyTask");
                return Json(new { Url = redirectUrl });


            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
                ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name");

                return View(_entity);
            }



            TempData["Msg"] = "Request  added successfully!";
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name");

            return View(_entity);




        }
        public ActionResult RollBackRequestForMe()
        {
            try
            {


                var hodid = Convert.ToInt32(Session["empId"].ToString());
                var RequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid && x.status == status.RollBack.ToString()).ToList();
                var _resultModel = new List<ServiceRequestViewModel>();

                foreach (var item in RequestForMe)
                {
                    var _model = new ServiceRequestViewModel();
                    _model.Id = item.Id;
                    _model.Subject = item.Subject;
                    _model.UserId = item.UserId;
                    _model.status = item.status;
                    _model.Priorty = item.Priorty;
                    _model.CreateDate = item.CreateDate;
                    _model.AssignedEmpName = item.AssignedEmpName;

                    _model.EmolyeeComments = item.EmolyeeComments;


                    _model.ServicesCategory = item.ServicesCategory;
                    _model.ServicesCategoryId = item.ServicesCategoryId.ToString();


                    _model.Department = item.Department;
                    _model.DepartmentId = item.DepartmentId.ToString();

                    _model.HODId = item.HODId;
                    _model.Attachment = item.Attachment;
                    _model.Description = item.Description;
                    _resultModel.Add(_model);


                }
                IdentityDb dbs = new IdentityDb();
                var user = dbs.Users.ToList();



                if (_resultModel.Count > 0)
                {
                    for (int i = 0; i < _resultModel.Count(); i++)
                    {
                        foreach (var item in user.ToList().Where(x => x.Id == _resultModel[i].UserId))
                        {
                            _resultModel[i].Email = item.Email;
                        }
                    }
                }
                return View(_resultModel);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult RollbackAssignRequest(int id)
        {
            try
            {
                var _request = db.ServiceRequests.Find(id);
                var _employees = db.Employees.Where(x => x.DepartmentId == _request.DepartmentId).ToList();
                ViewBag.Employees = new SelectList(_employees, "EmployeeId", "Name");
                ServiceRequestViewModel model = new ServiceRequestViewModel();
                model.Attachment = _request.Attachment;
                model.Subject = _request.Subject;
                model.UserId = _request.UserId;
                model.status = _request.status;
                model.Priorty = _request.Priorty;
                model.CreateDate = _request.CreateDate;
                model.EmolyeeComments = _request.EmolyeeComments;
                model.HodComments = _request.HodComments;
                model.ServicesCategory = _request.ServicesCategory;
                model.ServicesCategoryId = _request.ServicesCategoryId.ToString();

                model.Department = _request.Department;
                model.DepartmentId = _request.DepartmentId.ToString();

                model.HODId = _request.HODId;
                model.Attachment = _request.Attachment;
                model.Description = _request.Description;
                model.DueDate = _request.DueDate;
                model.HODId = _request.HODId;
                




                return View(model);
            }
            catch (Exception ex)
            {
                var Massage = ex.Message;
                TempData["Msg"] = "Opps" + Massage;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        //[CheckLicenses]
        public ActionResult RollbackAssignRequest(ServiceRequestViewModel model)
        {
            var _request = db.ServiceRequests.Find(model.Id);
            var _employees = db.Employees.Where(x => x.DepartmentId == _request.DepartmentId).ToList();
            ViewBag.Employees = new SelectList(_employees, "EmployeeId", "Name");
            if (ModelState.IsValid)
            {
                try
                {


                    var _entity = db.ServiceRequests.Find(model.Id);
                    var _emp = db.Employees.Find(Convert.ToInt32(model.AssignedEmpId));
                    _entity.AssignedEmpId = Convert.ToInt32(model.AssignedEmpId);
                    _entity.AssignedEmpName = _emp.Name;
                    _entity.status = status.Inprogress.ToString();
                    _entity.AssignedDate = DateTime.Now;
                    _entity.DueDate = model.DueDate;
                    _entity.HodComments = model.HodComments;



                    db.Entry(_entity).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Msg"] = "Assigned request successfully! ";
                    return RedirectToAction("RollBackRequestForMe");

                }
                catch (Exception ex)
                {
                    var Massage = ex.Message;
                    TempData["Msg"] = "Opps" + Massage;
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);
                    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "Id", "Name", model.ServicesCategoryId);

                    return View(model);
                }




            }


            return View(model);
        }


    }
}