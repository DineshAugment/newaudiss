using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using EndToEnd.Models;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.IO;
using PagedList;
using Rotativa;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace EndToEnd.Controllers
{
    public class HomeController : Controller
    {
        private AudissEntities db = new AudissEntities();
        ApplicationDbContext context = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Authorize]
        public ActionResult Index(string currentFilter = null, int? page = 1)
        {

            try
            {
                ApplicationUser user = UserManager.FindByName(User.Identity.Name);
                var ctrl = db.Controls.Where(x => x.Status == "Draft").OrderByDescending(q => q.ControlID).ToList();
                if (User.IsInRole("Auditor"))
                {
                    ctrl = db.Controls.Where(x => x.Status == "Audit" && x.AssignedTo == user.Id).OrderBy(q => q.ControlID).ToList();
                }
                else if (User.IsInRole("Tester"))
                {
                    ctrl = db.Controls.Where(x => x.Status == "Testing" && x.AssignedTo == user.Id).OrderBy(q => q.ControlID).ToList();
                }

                return View(ctrl.ToList().ToPagedList(page ?? 1, 5));
            }
            catch (Exception ex)
            {
                Exception str = ex.InnerException;
                return null;
            }
        }

        [HttpGet]
        [ActionName("Index_Partial")]
        public PartialViewResult Index_Partial(string currentFilter_partial = null, int? page_partial = 1)
        {
            try
            {
                List<Control> listControlFilter = null;
                List<Control> listControl = db.Controls.ToList();
                string strUserId = User.Identity.GetUserId();

                listControlFilter = (from control in listControl
                                     join users in db.AspNetUsers
                                     on control.AssignedTo equals users.Id into res
                                     from users in res.DefaultIfEmpty()
                                     select new Control
                                     {
                                         ControlName = control.ControlName,
                                         Status = control.Status,
                                         CreatedBy = users != null ? users.Email : "",
                                         CreatedDate = control.CreatedDate,
                                         ControlID = control.ControlID,
                                         FileDetails = control.FileDetails,
                                         Workflows = control.Workflows
                                     }).ToList();

                if (!string.IsNullOrEmpty(strUserId) && listControlFilter != null && listControlFilter.Count > 0 && !User.IsInRole("Administrator"))
                {
                    int[] iArrControlId = (from workflow in db.Workflows
                                           where workflow.AssignedTo == strUserId
                                           group workflow by workflow.ControlID into grp
                                           select grp.Key).ToArray();
                    if (iArrControlId != null && iArrControlId.Length > 0)
                        listControlFilter = (from obj in listControlFilter where iArrControlId.Contains(obj.ControlID) select obj).ToList();
                    else
                        listControlFilter = null;
                }
                return PartialView(listControlFilter.ToPagedList(page_partial ?? 1, 5));
            }
            catch (Exception)
            {
                return PartialView();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult CreateCtrl()
        {
            ViewBag.ControlType = db.ControlTypes.ToList();
            List<ActiveDirectoryDetail> listADDetail = db.ActiveDirectoryDetails.ToList();
            ActiveDirectoryDetail objADDetail = new ActiveDirectoryDetail() { DomainName = "", Category = "Select", Id = 0 };
            listADDetail.Insert(0, objADDetail);
            ViewBag.ActiveDirectoryDetails = listADDetail;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCtrl([Bind(Exclude = "ControlID")] Control ctrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ctrl.CreatedDate = DateTime.Now;
                    ctrl.CreatedBy = User.Identity.Name;
                    ctrl.Status = "Draft";
                    string strDomainName = Request.Form["DatasSource"];
                    string strADD_ID = Request.Form["ADD_ID"];
                    List<FileDetail> fileDetails = new List<FileDetail>();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            FileDetail fileDetail = new FileDetail()
                            {
                                FileName = fileName,
                                Extension = Path.GetExtension(fileName),
                                ID = Guid.NewGuid()
                            };
                            fileDetails.Add(fileDetail);

                            var path = Path.Combine(Server.MapPath("~/Content/File/"), fileDetail.ID + fileDetail.Extension);
                            file.SaveAs(path);
                        }
                    }
                    ctrl.FileDetails = fileDetails;
                    db.Controls.Add(ctrl);
                    db.SaveChanges();
                    ViewBag.Message = "File Uploaded successfully.";
                    if (!string.IsNullOrEmpty(strDomainName) && !string.IsNullOrEmpty(strADD_ID))
                        return RedirectToAction("DetailsSave", "ActiveDirectory", new { cID = ctrl.ControlID, strDomainName = strDomainName, strADD_ID = strADD_ID });
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex /* dex */)
            {
                throw ex;
            }
            return View(ctrl);
        }

        public ActionResult EditCtrl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Control ctrl = db.Controls.Find(id);
            //if (ctrl == null)
            //{
            //    return HttpNotFound();
            //}

            Control ctrl = db.Controls.Include(s => s.FileDetails).SingleOrDefault(x => x.ControlID == id);
            if (ctrl == null)
            {
                return HttpNotFound();
            }
            if (ctrl != null && ctrl.ActiveDirectoryUsers != null && ctrl.ActiveDirectoryUsers.Count > 0)
            {
                int? intADID = ctrl.ActiveDirectoryUsers.Select(x => x.ADD_ID).FirstOrDefault();
                ViewBag.DomainName = db.ActiveDirectoryDetails.Where(y => y.Id == intADID).Select(x => x.DomainName).SingleOrDefault();
            }
            var roles = db.AspNetRoles.ToList();
            var userlist = db.AspNetUsers.ToList();
            var status = db.StatusDetails.ToList();
            ViewBag.SelectedItem = ctrl.Status;
            ViewBag.UserRole = status;
            ViewBag.UserList = userlist;

            ViewBag.ControlTypeItems = db.ControlTypes.ToList();
            //ViewBag.ADCategory = Convert.ToString(ctrl.ActiveDirectoryUsers.Select(model => model.ADD_ID).SingleOrDefault());

            return View(ctrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditSave")]
        public ActionResult EditSave(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Control objControl = db.Controls.Find(id);
            string strOldStatus = objControl?.Status;

            if (TryUpdateModel(objControl))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];

                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                FileDetail fileDetail = new FileDetail()
                                {
                                    FileName = fileName,
                                    Extension = Path.GetExtension(fileName),
                                    ID = Guid.NewGuid(),
                                    ControlId = objControl.ControlID
                                };
                                var path = Path.Combine(Server.MapPath("~/Content/File/"), fileDetail.ID + fileDetail.Extension);
                                file.SaveAs(path);

                                db.Entry(fileDetail).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }
                    }

                    var roles = db.AspNetRoles.ToList();
                    var status = db.StatusDetails.ToList();
                    ViewBag.ControlTypeItems = db.ControlTypes.ToList();
                    var assigned = status.Where(p => p.Id == objControl.Status).ToList();
                    SelectList selectList = new SelectList(assigned, "Id", "Status");

                    foreach (SelectListItem sitem in selectList)
                    {
                        string tex = sitem.Text;
                        string val = sitem.Value;
                        ViewBag.SelectedItem = tex;
                        var role = (from r in context.Roles where r.Id.Contains(val) select r).FirstOrDefault();
                        var users = context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToList();

                        ViewBag.UserRole = status;
                        ViewBag.UserList = users;
                        ViewBag.AssignStatus = tex;
                    }

                    objControl.Status = ViewBag.AssignStatus;

                    if (!strOldStatus.Equals(objControl.Status))
                    {
                        Workflow work = new Workflow();
                        work.ControlID = objControl.ControlID;
                        work.ActionDate = DateTime.Now;
                        work.StatusFrom = strOldStatus;
                        work.StatusTo = objControl.Status;
                        work.ActionBy = User.Identity.Name;
                        work.Comments = Request.Form["WorkFlowComments"];
                        work.AssignedTo = objControl.AssignedTo;
                        db.Workflows.Add(work);
                    }
                    db.Entry(objControl).State = EntityState.Modified;
                    db.SaveChanges();

                    //return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return RedirectToAction("EditCtrl", new { id = objControl.ControlID });
        }

        public ActionResult UserList(string roleName)
        {
            return View();
        }

        [HttpGet]
        [ActionName("DeleteFile")]
        public JsonResult DeleteFile(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("No file found", JsonRequestBehavior.AllowGet);
            try
            {
                string strFilepath = Path.Combine(Server.MapPath("~"), "Content", "File", id);
                if (System.IO.File.Exists(strFilepath))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Guid fileGuid = new Guid(id.Split('.')[0]);
                    var objFileDetail = db.FileDetails.Where(model => model.ID == fileGuid).SingleOrDefault();
                    db.Entry(objFileDetail).State = EntityState.Deleted;
                    db.SaveChanges();
                    System.IO.File.Delete(strFilepath);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json("File has been deleted succcessfully", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var objControl = db.Controls.Where(x => x.ControlID == id).SingleOrDefault();
            if (!string.IsNullOrEmpty(objControl.AssignedTo))
                objControl.AssignedTo = (db.AspNetUsers.Where(y => y.Id == objControl.AssignedTo).SingleOrDefault()).Email;
            return View(objControl);
        }

        [HttpGet]
        public ActionResult DetailsPDf(int id)
        {
            var objControl = db.Controls.Where(x => x.ControlID == id).SingleOrDefault();
            if (!string.IsNullOrEmpty(objControl.AssignedTo))
                objControl.AssignedTo = (db.AspNetUsers.Where(y => y.Id == objControl.AssignedTo).SingleOrDefault()).Email;
            return View(objControl);
        }

        [HttpGet]
        public ActionResult HTMLToPDF(int id)
        {
            return new Rotativa.ActionAsPdf("DetailsPDf", new { id = id }) { FileName = "ControlDetails.pdf" };
        }

    }
}