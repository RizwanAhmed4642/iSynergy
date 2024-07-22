using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSynergy.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using iSynergy.DataContexts;
using iSynergy.Areas.Procurement.Models;
using iSynergy.Areas.HR.Models;
using System.Web;
using iSynergy.Shared;
using iSynergy.Controllers;

namespace iSynergy.Areas.HR.Controllers
{
    public class EmployeesController : CustomController
    {
        private CompanyDb db = new CompanyDb();

        // GET: Employees
        [AuthorizeRedirect(Roles = "Admin, Can View Employees")]
        public ActionResult Index()
        {
            var employees = db.Employees
                            .Where(e => e.ReleaseDate == null)
                            .Include(e => e.Department)
                            .Include(e => e.Designation)
                            .Include(e => e.Office)
                            .OrderBy(e => e.Name);

            ViewBag.employeeIdToNameConverter = new EmployeeIdtoNameDictonary();
            foreach (var employee in employees)
            {
                ViewBag.employeeIdToNameConverter.Add(employee.EmployeeId, db);
            }
            return View(employees.ToList());
        }

        [AuthorizeRedirect(Roles = "Admin, Can View Employees")]
        public ActionResult ExEmployees()
        {
            var employees = db.Employees
                            .Include(e => e.Department)
                            .Include(e => e.Designation)
                            .Include(e => e.Office)
                            .OrderBy(e => e.Name);

            ViewBag.employeeIdToNameConverter = new EmployeeIdtoNameDictonary();
            foreach (var employee in employees)
            {
                ViewBag.employeeIdToNameConverter.Add(employee.EmployeeId, db);
            }
            employees = db.Employees
                            .Where(e => e.ReleaseDate != null)
                            .Include(e => e.Department)
                            .Include(e => e.Designation)
                            .Include(e => e.Office)
                            .OrderBy(e => e.Name);

            return View(employees.ToList());
        }
        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(employee);
        }

        // GET: Employees/Create
        [AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        [CheckLicenses]
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.DesignationId = new SelectList(db.Designations, "DesignationId", "Title");
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location");
            ViewBag.ManagerId = new SelectList(db.Employees, "EmployeeId", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        [CheckLicenses]
        public ActionResult Create([Bind(Include = "EmployeeId,DepartmentId,DesignationId,OfficeId,ManagerId,Name,Email,JoiningDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();

                // application user must be created as soon as an employee is created.
                //
                //get access to Identity context
                var identityDb = new IdentityDb();

                // get access to application's roles and users repository.
                var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(identityDb));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(identityDb));

                ApplicationUser newAppUser = new ApplicationUser { UserName = employee.Email.Substring(0, employee.Email.IndexOf('@')), Email = employee.Email };

                // create the user with default password
                var identityResult = UserManager.Create(newAppUser, Properties.Settings.Default.DefaultPassword);

                // assign a default role to the newly created application user.
                identityResult = UserManager.AddToRole(newAppUser.Id, Properties.Settings.Default.DefaultRoleName);

                TempData["success"] = "Employee added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.DesignationId = new SelectList(db.Designations, "DesignationId", "Title", employee.DesignationId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", employee.OfficeId);
            ViewBag.ManagerId = new SelectList(db.Employees, "EmployeeId", "Name", employee.ManagerId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        [AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.DesignationId = new SelectList(db.Designations, "DesignationId", "Title", employee.DesignationId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", employee.OfficeId);
            ViewBag.ManagerId = new SelectList(db.Employees.Where(e => e.ReleaseDate == null), "EmployeeId", "Name", employee.ManagerId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = "Admin, Can Edit Employees")]
        public ActionResult Edit([Bind(Include = "EmployeeId,DepartmentId,DesignationId,OfficeId,ManagerId,Name,Email,JoiningDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.DesignationId = new SelectList(db.Designations, "DesignationId", "Title", employee.DesignationId);
            ViewBag.OfficeId = new SelectList(db.Offices, "OfficeId", "Location", employee.OfficeId);
            ViewBag.ManagerId = new SelectList(db.Employees.Where(e => e.ReleaseDate == null), "EmployeeId", "Name", employee.ManagerId);
            return View(employee);
        }


        [AuthorizeRedirect(Roles = "Admin")]
        public ActionResult Release(int id)
        {
            var companyDb = new CompanyDb();
            var employee = companyDb.Employees.Find(id);
            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            var subOrdinates = companyDb.Employees.Where(e => e.ManagerId == employee.EmployeeId).Count();
            if (subOrdinates > 0)
            {
                TempData["error"] = String.Format("Cannot release because {0} employees are reporting to {1}", subOrdinates, employee.Name);
                return RedirectToAction("Index");
            }
            var identityDb = new IdentityDb();
            var userAccount = identityDb.Users.Where(u => u.Email == employee.Email).First();

            if (identityDb.Users.Find(userAccount) != null)
            {
                identityDb.Users.Remove(userAccount);
                identityDb.SaveChanges();
            }

            employee.ReleaseDate = DateTime.Now;
            companyDb.SaveChanges();
            TempData["success"] = String.Format("Employee released successfully!", subOrdinates, employee.Name);
            return RedirectToAction("Index");
        }
        [CheckLicenses]
        public ActionResult UploadEmployees()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckLicenses]
        public ActionResult UploadEmployees(HttpPostedFileBase FileUpload)
        {
            if (ModelState.IsValid)
            {

                if (FileUpload != null && FileUpload.ContentLength > 0)
                {

                    if (FileUpload.FileName.EndsWith(".csv"))
                    {
                        try
                        {
                            DataTable model = CSV.csv2DataTable(FileUpload);
                            if (!CSV.isValidTableFormat(dtCOA: model, TotalColumnsExpected: 8, HeadersExpected: new string[] { "EmployeeId", "Name", "Email", "DepartmentId", "DesignationId", "OfficeId", "ManagerId", "JoiningDate" }))
                            {
                                TempData["error"] = "The uploaded file columns are not as per the standard format.";
                            }
                            else
                            {
                                TempData["Employees"] = model; // to be used in post action method
                                return View(model);
                            }
                        }
                        catch (Exception)
                        {

                            TempData["error"] = "Error occured while reading the uploaded file.";
                        }
                    }
                    else
                    {
                        TempData["error"] = "This file format is not supported";
                    }
                }
                else
                {
                    TempData["error"] = "Please upload a CSV file that contains the Chart of Accounts.";
                }
            }
            return RedirectToAction("UploadEmployees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmUploadEmployees()
        {
            DataTable Employees = (DataTable)TempData["Employees"];

            for (int row = 0; row < Employees.Rows.Count; row++)
            {
                string employeeId = Employees.Rows[row][0].ToString();
                string name = Employees.Rows[row][1].ToString();
                string email = Employees.Rows[row][2].ToString();
                string departmentId = Employees.Rows[row][3].ToString();
                string designationId = Employees.Rows[row][4].ToString();
                string officeId = Employees.Rows[row][5].ToString();
                string managerId = Employees.Rows[row][6].ToString();
                string joiningDate = Employees.Rows[row][7].ToString();

                Employee employee = db.Employees.Where(
                                                x => x.EmployeeId == Int32.Parse(employeeId)
                                                || x.Email.ToLower().CompareTo(email.ToLower()) == 0
                                                )
                                                .FirstOrDefault();


                // check if the employee already exists, if not add it. 
                if (employee == null)
                {
                    employee = new Employee
                    {
                        EmployeeId = Int32.Parse(employeeId),
                        Name = name,
                        Email = email,
                        DepartmentId = Int32.Parse(departmentId),
                        DesignationId = Int32.Parse(designationId),
                        OfficeId = Int32.Parse(officeId),
                        ManagerId = Int32.Parse(managerId),
                        JoiningDate = DateTime.Parse(joiningDate)
                    };
                    db.Employees.Add(employee);
                    db.SaveChanges();

                    // application user must be created as soon as an employee is created.
                    //
                    //get access to Identity context
                    var identityDb = new IdentityDb();

                    // get access to application's roles and users repository.
                    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(identityDb));
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(identityDb));

                    ApplicationUser newAppUser = new ApplicationUser { UserName = employee.Email.Substring(0, employee.Email.IndexOf('@')), Email = employee.Email };

                    // create the user with default password
                    var identityResult = UserManager.Create(newAppUser, Properties.Settings.Default.DefaultPassword);

                    // assign a default role to the newly created application user.
                    identityResult = UserManager.AddToRole(newAppUser.Id, Properties.Settings.Default.DefaultRoleName);
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
