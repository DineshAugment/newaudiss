using EndToEnd.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EndToEnd.Controllers
{
    public class ControlTypeController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                return View(objContext.ControlTypes.ToList());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ControlType objControlType)
        {
            if (ModelState.IsValid)
            {
                using (AudissEntities objContext = new AudissEntities())
                {
                    objContext.ControlTypes.Add(objControlType);
                    objContext.SaveChanges();
                }
            }
            else
            {
                return View(objControlType);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                return View(objContext.ControlTypes.Where(model=>model.Id==id).SingleOrDefault());
            }
        }

        [HttpPost]
        [ActionName("Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int? id)
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                ControlType objControlType = objContext.ControlTypes.Find(id);
                if (TryUpdateModel(objControlType))
                {
                    objContext.Entry(objControlType).State = EntityState.Modified;
                    objContext.SaveChanges();
                }
                else
                {
                    return View(objControlType);
                }
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (AudissEntities objContext = new AudissEntities())
            {
                ControlType objControlType = objContext.ControlTypes.Find(id);
                objContext.Entry(objControlType).State = EntityState.Deleted;
                objContext.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}