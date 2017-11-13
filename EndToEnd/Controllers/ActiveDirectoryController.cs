using EndToEnd.Managers;
using EndToEnd.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace EndToEnd.Controllers
{
    public class ActiveDirectoryController : Controller
    {
        // GET: ActiveDirectory
        public ActionResult Index(string SearchText = "")
        {
            ViewBag.SearchText = SearchText;
            return View(ActiveDirectoryManager.GetDetailsFromAD(SearchText));
        }

        public ActionResult Details(int cID, string strDomainName = "")
        {
            ViewBag.ControlID = cID;
            ViewBag.DomainName = strDomainName;
            return View(ActiveDirectoryManager.GetADDetailsFromIP(strDomainName));
        }

        public JsonResult GetDetails(string strDomainName = "")
        {
            return Json(ActiveDirectoryManager.GetADDetailsFromIP(strDomainName), JsonRequestBehavior.AllowGet);
        }

        [ActionName("DetailsSave")]
        public ActionResult DetailsSave(int cID, string strDomainName, int strADD_ID)
        {
            int intControlID = cID;
            int intADD_ID = strADD_ID;
            List<ActiveDirectoryModel> listADModel = ActiveDirectoryManager.GetADDetailsFromIP(strDomainName);
            if (listADModel?.Count > 0)
            {
                List<ActiveDirectoryUser> listADUser = listADModel.Select(model => new ActiveDirectoryUser
                {
                    ControlID = intControlID,
                    EmailId = model.EmailID,
                    FirtName = model.FirstName,
                    UserName = model.UserName,
                    ADD_ID = intADD_ID

                }).ToList();
                using (AudissEntities objEntity = new AudissEntities())
                {
                    foreach (var obj in listADUser)
                    {
                        obj.ADD_ID = obj.ADD_ID;
                        objEntity.ActiveDirectoryUsers.Add(obj);
                        objEntity.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create([Bind(Exclude = "ID")]ActiveDirectoryDetail objADDetails)
        {
            if (ModelState.IsValid)
            {
                using (AudissEntities objContext = new AudissEntities())
                {
                    objADDetails.LDAPAddress = string.Concat("LDAP://", objADDetails.IPAddress);
                    objContext.ActiveDirectoryDetails.Add(objADDetails);
                    objContext.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public ActionResult List()
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                List<ActiveDirectoryDetail> listADDetails = objContext.ActiveDirectoryDetails.ToList();
                return View(listADDetails);
            }
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit(int id)
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                ActiveDirectoryDetail objADDetails = objContext.ActiveDirectoryDetails.Where(x => x.Id == id).SingleOrDefault();
                return View(objADDetails);
            }
        }

        [HttpPost]
        [ActionName("EditSave")]
        public ActionResult EditSave(int id)
        {
            ActiveDirectoryDetail objADDetails = null;
            if (ModelState.IsValid)
            {

                using (AudissEntities objContext = new AudissEntities())
                {
                    objADDetails = objContext.ActiveDirectoryDetails.Find(id);
                    if (TryUpdateModel(objADDetails))
                    {
                        objADDetails.LDAPAddress = string.Concat("LDAP://", objADDetails.IPAddress);
                        objContext.Entry(objADDetails).State = EntityState.Modified;
                        objContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                ActiveDirectoryDetail objADDetails = objContext.ActiveDirectoryDetails.Where(x => x.Id == id).SingleOrDefault();
                objContext.ActiveDirectoryDetails.Remove(objADDetails);
                objContext.SaveChanges();
                return RedirectToAction("List");
            }
        }
    }
}