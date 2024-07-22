using iSynergy.DataContexts;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using iSynergy.Areas.Inventory.Models;
using iSynergy.Areas.HR.Models;
using iSynergy.Controllers;

namespace iSynergy.Areas.Inventory.Controllers
{
    [AuthorizeRedirect(Roles = "Admin, Can Manage Inventory")]
    public class AssetsController : CustomController
    {
        private CompanyDb companyDb = new CompanyDb();
        private InventoryDb inventoryDb = new InventoryDb();
        private IdentityDb identityDb = new IdentityDb();


        // GET: Assets/Computers
        public ActionResult Computers()
        {
            return View(inventoryDb.Computers.Where(x => x.Disposed == false).ToList());
        }
        // GET: Assets/AddComputer

        [CheckLicenses]
        public ActionResult AddComputer()
        {
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");

            return View();
        }
        [HttpPost]
        [CheckLicenses]
        [ValidateAntiForgeryToken]
        public ActionResult AddComputer(Computer computer)
        {
            if (ModelState.IsValid)
            {
                // add the asset to DB
                inventoryDb.Computers.Add(computer);
                inventoryDb.SaveChanges();
                return RedirectToAction("Computers");
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");
            return View(computer);
        }
        // GET: Assets/EditComputer
        public ActionResult EditComputer(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Computer computer = inventoryDb.Computers.Find(id);
            if (computer == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location", computer.OfficeId);
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name", computer.CustodianId);
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name", computer.SupplierId);
            return View(computer);
        }
        // POST: Assets/EditComputer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComputer(Computer computer)
        {
            if (ModelState.IsValid)
            {
                var oldObject = inventoryDb.Computers.Find(computer.AssetId);
                string messageBodyHtml = "<html><table width='100%'><tr><td>Data Item</td><td>Old Value</td><td>New Value<td></tr>" +
                                        "<tr><td>Name</td><td>" + oldObject.Name + "</td><td>" + computer.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + oldObject.OfficeId + "</td><td>" + computer.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + oldObject.CustodianId + "</td><td>" + computer.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + oldObject.SupplierId + "</td><td>" + computer.SupplierId + "</td></tr>" +
                                        "<tr><td>Serial Number</td><td>" + oldObject.SerialNumber + "</td><td>" + computer.SerialNumber + "</td></tr>" +
                                        "<tr><td>Manufacturer</td><td>" + oldObject.Manufacturer + "</td><td>" + computer.Manufacturer + "</td></tr>" +
                                        "<tr><td>Model</td><td>" + oldObject.Model + "</td><td>" + computer.Model + "</td></tr>" +
                                        "<tr><td>Processor</td><td>" + oldObject.Processor + "</td><td>" + computer.Processor + "</td></tr>" +
                                        "<tr><td>RAM</td><td>" + oldObject.RAM + "</td><td>" + computer.RAM + "</td></tr>" +
                                        "<tr><td>Hard Disk</td><td>" + oldObject.HardDisk + "</td><td>" + computer.HardDisk + "</td></tr>" +
                                        "</table></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(oldObject.OfficeId)).Email,
                        "Inventory Updated: A computer record has been updated. ",
                        messageBodyHtml
                    );

                InventoryDb newInventoryDb = new InventoryDb();
                newInventoryDb.Entry(computer).State = EntityState.Modified;
                newInventoryDb.SaveChanges();
                newInventoryDb.Dispose();
                return RedirectToAction("Computers");
            }
            return View(computer);
        }


        //GET: Assets/Devices
        public ActionResult Devices()
        {
            return View(inventoryDb.Devices.Where(x => x.Disposed == false).ToList());
        }
        // GET: Assets/AddDevice
        [CheckLicenses]
        public ActionResult AddDevice()
        {
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");

            return View();
        }
        [HttpPost]
        [CheckLicenses]
        [ValidateAntiForgeryToken]
        public ActionResult AddDevice(Device device)
        {
            if (ModelState.IsValid)
            {
                // add the asset to DB
                inventoryDb.Devices.Add(device);
                inventoryDb.SaveChanges();
                return RedirectToAction("Devices");
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");
            return View(device);
        }
        // GET: Assets/EditDevice
        public ActionResult EditDevice(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Device device = inventoryDb.Devices.Find(id);
            if (device == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location", device.OfficeId);
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name", device.CustodianId);
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name", device.SupplierId);
            return View(device);
        }
        // POST: Assets/EditDevice/5
        [HttpPost]
        public ActionResult EditDevice(Device device)
        {
            if (ModelState.IsValid)
            {
                var oldObject = inventoryDb.Devices.Find(device.AssetId);
                string messageBodyHtml = "<html><table width='100%'><tr><td>Data Item</td><td>Old Value</td><td>New Value<td></tr>" +
                                        "<tr><td>Name</td><td>" + oldObject.Name + "</td><td>" + device.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + oldObject.OfficeId + "</td><td>" + device.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + oldObject.CustodianId + "</td><td>" + device.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + oldObject.SupplierId + "</td><td>" + device.SupplierId + "</td></tr>" +
                                        "<tr><td>Serial Number</td><td>" + oldObject.SerialNumber + "</td><td>" + device.SerialNumber + "</td></tr>" +
                                        "<tr><td>Manufacturer</td><td>" + oldObject.Manufacturer + "</td><td>" + device.Manufacturer + "</td></tr>" +
                                        "<tr><td>Model</td><td>" + oldObject.Model + "</td><td>" + device.Model + "</td></tr>" +
                                        "</table></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(oldObject.OfficeId)).Email,
                        "Inventory Updated: A device record has been updated. ",
                        messageBodyHtml
                    );

                InventoryDb newInventoryDb = new InventoryDb();
                newInventoryDb.Entry(device).State = EntityState.Modified;
                newInventoryDb.SaveChanges();
                newInventoryDb.Dispose();
                return RedirectToAction("Devices");
            }
            return View(device);
        }


        //GET: Assets/BulkAssets
        public ActionResult BulkAssets()
        {
            return View(inventoryDb.BulkAssets.Where(x => x.Disposed == false).ToList());
        }
        // GET: Assets/AddBulkAsset

        [CheckLicenses]
        public ActionResult AddBulkAsset()
        {
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");

            return View();
        }
        [HttpPost]
        [CheckLicenses]
        [ValidateAntiForgeryToken]
        public ActionResult AddBulkAsset(BulkAsset bulkAsset)
        {
            if (ModelState.IsValid)
            {
                // add the asset to DB
                inventoryDb.BulkAssets.Add(bulkAsset);
                inventoryDb.SaveChanges();
                return RedirectToAction("BulkAssets");
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location");
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name");
            return View(bulkAsset);
        }
        // GET: Assets/EditBulkAsset
        public ActionResult EditBulkAsset(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            BulkAsset bulkAsset = inventoryDb.BulkAssets.Find(id);
            if (bulkAsset == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ViewBag.OfficeId = new SelectList(companyDb.Offices, "OfficeId", "Location", bulkAsset.OfficeId);
            ViewBag.CustodianId = new SelectList(companyDb.Employees, "EmployeeId", "Name", bulkAsset.CustodianId);
            ViewBag.SupplierId = new SelectList(inventoryDb.Suppliers, "SupplierId", "Name", bulkAsset.SupplierId);
            return View(bulkAsset);
        }
        // POST: Assets/EditDevice/5
        [HttpPost]
        public ActionResult EditBulkAsset(BulkAsset bulkAsset)
        {
            if (ModelState.IsValid)
            {
                var oldObject = inventoryDb.BulkAssets.Find(bulkAsset.AssetId);
                string messageBodyHtml = "<html><table width='100%'><tr><td>Data Item</td><td>Old Value</td><td>New Value<td></tr>" +
                                        "<tr><td>Name</td><td>" + oldObject.Name + "</td><td>" + bulkAsset.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + oldObject.OfficeId + "</td><td>" + bulkAsset.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + oldObject.CustodianId + "</td><td>" + bulkAsset.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + oldObject.SupplierId + "</td><td>" + bulkAsset.SupplierId + "</td></tr>" +
                                        "<tr><td>Quantity</td><td>" + oldObject.Quantity + "</td><td>" + bulkAsset.Quantity + "</td></tr>" +
                                        "<tr><td>Re-Order Level</td><td>" + oldObject.ReOrderLevel + "</td><td>" + bulkAsset.ReOrderLevel + "</td></tr>" +
                                        "</table></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(oldObject.OfficeId)).Email,
                        "Inventory Updated: A bulk asset record has been updated. ",
                        messageBodyHtml
                    );

                InventoryDb newInventoryDb = new InventoryDb();
                newInventoryDb.Entry(bulkAsset).State = EntityState.Modified;
                newInventoryDb.SaveChanges();
                newInventoryDb.Dispose();
                return RedirectToAction("BulkAssets");
            }
            return View(bulkAsset);
        }

        //GET: Assets/DisposeComputer/5
        public ActionResult DisposeComputer(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Computer computer = inventoryDb.Computers.Find(id);
            if (computer == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(computer);
        }
        //POST: Assets/DisposeComputer
        [HttpPost]
        public ActionResult DisposeComputer(Computer disposedComputer)
        {
            if (disposedComputer.DisposalReason != null)
            {
                var computer = inventoryDb.Computers.Find(disposedComputer.AssetId);
                var disposalReason = "";
                if (disposedComputer.DisposalReason == (int)DisposalReasons.Broken) { computer.markBroken(DateTime.Now, this.thisGuy, disposedComputer.DisposalComments); disposalReason = "Broken"; }
                else if (disposedComputer.DisposalReason == (int)DisposalReasons.Lost) { computer.markLost(DateTime.Now, this.thisGuy, disposedComputer.DisposalComments); disposalReason = "Lost"; }
                else if (disposedComputer.DisposalReason == (int)DisposalReasons.OutOfOrder) { computer.markOutOfOrder(DateTime.Now, this.thisGuy, disposedComputer.DisposalComments); disposalReason = "OutOfOrder"; }

                //inventoryDb.Entry(computer).State = EntityState.Modified;
                inventoryDb.SaveChanges();

                string messageBodyHtml = "<html><P><strong>Following computer has been disposed by " + companyDb.Employees.Find(computer.DisposedById).Name + "</strong></p>" +
                                        "<table width='100%'>" +
                                        "<tr><td>Name</td><td>" + computer.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + computer.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + computer.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + computer.SupplierId + "</td></tr>" +
                                        "<tr><td>Serial Number</td><td>" + computer.SerialNumber + "</td></tr>" +
                                        "<tr><td>Manufacturer</td><td>" + computer.Manufacturer + "</td></tr>" +
                                        "<tr><td>Model</td><td>" + computer.Model + "</td></tr>" +
                                        "<tr><td>Processor</td><td>" + computer.Processor + "</td></tr>" +
                                        "<tr><td>RAM</td><td>" + computer.RAM + "</td></tr>" +
                                        "<tr><td>Hard Disk</td><td>" + computer.HardDisk + "</td></tr>" +
                                        "</table>" +
                                        "<P><strong>Dispoasal Reason: " + disposalReason + "</strong></p></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(computer.OfficeId)).Email,
                        "Inventory Disposed: A computer has been marked disposed. ",
                        messageBodyHtml
                    );

                return RedirectToAction("Computers");
            }
            return View(disposedComputer);
        }

        //GET: Assets/DisposeDevice/5
        public ActionResult DisposeDevice(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            Device device = inventoryDb.Devices.Find(id);
            if (device == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(device);
        }
        //POST: Assets/DisposeDevice
        [HttpPost]
        public ActionResult DisposeDevice(Device disposedDevice)
        {
            if (disposedDevice.DisposalReason != null)
            {
                var device = inventoryDb.Devices.Find(disposedDevice.AssetId);
                var disposalReason = "";
                if (disposedDevice.DisposalReason == (int)DisposalReasons.Broken) { device.markBroken(DateTime.Now, this.thisGuy, disposedDevice.DisposalComments); disposalReason = "Broken"; }
                else if (disposedDevice.DisposalReason == (int)DisposalReasons.Lost) { device.markLost(DateTime.Now, this.thisGuy, disposedDevice.DisposalComments); disposalReason = "Lost"; }
                else if (disposedDevice.DisposalReason == (int)DisposalReasons.OutOfOrder) { device.markOutOfOrder(DateTime.Now, this.thisGuy, disposedDevice.DisposalComments); disposalReason = "OutOfOrder"; }

                //inventoryDb.Entry(computer).State = EntityState.Modified;
                inventoryDb.SaveChanges();

                string messageBodyHtml = "<html><P><strong>Following device has been disposed by " + companyDb.Employees.Find(device.DisposedById).Name + "</strong></p>" +
                                        "<table width='100%'>" +
                                        "<tr><td>Name</td><td>" + device.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + device.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + device.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + device.SupplierId + "</td></tr>" +
                                        "<tr><td>Serial Number</td><td>" + device.SerialNumber + "</td></tr>" +
                                        "<tr><td>Manufacturer</td><td>" + device.Manufacturer + "</td></tr>" +
                                        "<tr><td>Model</td><td>" + device.Model + "</td></tr>" +
                                        "</table>" +
                                        "<P><strong>Dispoasal Reason: " + disposalReason + "</strong></p></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(device.OfficeId)).Email,
                        "Inventory Disposed: A device has been marked disposed. ",
                        messageBodyHtml
                    );

                return RedirectToAction("Devices");
            }
            return View(disposedDevice);
        }


        //GET: Assets/DisposeBulkAsset/5
        public ActionResult DisposeBulkAsset(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", new { area = "" });
            }
            BulkAsset bulkAsset = inventoryDb.BulkAssets.Find(id);
            if (bulkAsset == null)
            {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            return View(bulkAsset);
        }
        //POST: Assets/DisposeBulkAsset
        [HttpPost]
        public ActionResult DisposeBulkAsset(BulkAsset disposedBulkAsset)
        {
            if (disposedBulkAsset.DisposalReason != null)
            {
                var bulkAsset = inventoryDb.BulkAssets.Find(disposedBulkAsset.AssetId);
                var disposalReason = "";
                if (disposedBulkAsset.DisposalReason == (int)DisposalReasons.Broken) { bulkAsset.markBroken(DateTime.Now, this.thisGuy, disposedBulkAsset.DisposalComments); disposalReason = "Broken"; }
                else if (disposedBulkAsset.DisposalReason == (int)DisposalReasons.Lost) { bulkAsset.markLost(DateTime.Now, this.thisGuy, disposedBulkAsset.DisposalComments); disposalReason = "Lost"; }
                else if (disposedBulkAsset.DisposalReason == (int)DisposalReasons.OutOfOrder) { bulkAsset.markOutOfOrder(DateTime.Now, this.thisGuy, disposedBulkAsset.DisposalComments); disposalReason = "OutOfOrder"; }
                else if (disposedBulkAsset.DisposalReason == (int)DisposalReasons.Consumed) { bulkAsset.markConsumed(DateTime.Now, this.thisGuy, disposedBulkAsset.DisposalComments); disposalReason = "Consumed"; }
                else if (disposedBulkAsset.DisposalReason == (int)DisposalReasons.Expired) { bulkAsset.markExpired(DateTime.Now, this.thisGuy, disposedBulkAsset.DisposalComments); disposalReason = "Expired"; }
                //inventoryDb.Entry(computer).State = EntityState.Modified;
                inventoryDb.SaveChanges();

                string messageBodyHtml = "<html><P><strong>Following bulk asset has been disposed by " + companyDb.Employees.Find(bulkAsset.DisposedById).Name + "</strong></p>" +
                                        "<table width='100%'>" +
                                        "<tr><td>Name</td><td>" + bulkAsset.Name + "</td></tr>" +
                                        "<tr><td>Property of</td><td>" + bulkAsset.OfficeId + "</td></tr>" +
                                        "<tr><td>Custodian</td><td>" + bulkAsset.CustodianId + "</td></tr>" +
                                        "<tr><td>Supplier</td><td>" + bulkAsset.SupplierId + "</td></tr>" +
                                        "<tr><td>Quantity</td><td>" + bulkAsset.Quantity + "</td></tr>" +
                                        "<tr><td>Re-Order Level</td><td>" + bulkAsset.ReOrderLevel + "</td></tr>" +
                                        "</table>" +
                                        "<P><strong>Dispoasal Reason: " + disposalReason + "</strong></p></html>";

                iSynergy.Shared.Mail.SendEmail(
                        getLocalOperationsHeadFor(companyDb.Offices.Find(bulkAsset.OfficeId)).Email,
                        "Inventory Disposed: A bulk asset has been marked disposed. ",
                        messageBodyHtml
                    );

                return RedirectToAction("BulkAssets");
            }
            return View(disposedBulkAsset);
        }

        private Employee getLocalOperationsHeadFor(Office office)
        {
            return companyDb.Employees.Where(e => e.EmployeeId == office.LocalOperationsHeadId).First();
        }
    }
}