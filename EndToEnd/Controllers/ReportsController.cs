using EndToEnd.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EndToEnd.Controllers
{
    public class ReportsController : Controller
    {

        private AudissEntities db = new AudissEntities();

        [HttpGet]
        public PartialViewResult Index_Partial(string ControlName = "", string FromDate = "", string ToDate = "", string ActionBy = "", string StatusFrom = "", string StatusTo = "", string searchStringUserNameOrEmail = null, string currentFilter = null, int? page = null)
        {
            List<Workflow> listWorkflow = null;
            DateTime? dtStartDate = null;
            DateTime? dtToDate = null;
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                DateTime test = DateTime.Now;
                dtStartDate = Convert.ToDateTime(FromDate).Date;
                dtToDate = Convert.ToDateTime(ToDate).Date;
                //dtStartDate = Convert.ToDateTime(DateTime.Parse(FromDate, CultureInfo.GetCultureInfo("en-US")).ToString("dd-MM-yyyy"));
                //dtStartDate = Convert.ToDateTime(DateTime.Parse(FromDate, CultureInfo.GetCultureInfo("en-US")).ToString("dd-MM-yyyy"));
            }
            listWorkflow = (from controls in db.Controls
                            join workflows in db.Workflows on controls.ControlID
                            equals workflows.ControlID
                            where (string.IsNullOrEmpty(ControlName) ||
                                controls.ControlName.Contains(ControlName)) &&

                                (string.IsNullOrEmpty(ActionBy) ||
                                workflows.ActionBy.Contains(ActionBy)) &&

                                 (string.IsNullOrEmpty(StatusFrom) ||
                                workflows.StatusFrom.Contains(StatusFrom)) &&

                                (string.IsNullOrEmpty(StatusTo) ||
                                workflows.StatusTo.Contains(StatusTo)) &&

                             (string.IsNullOrEmpty(FromDate) || string.IsNullOrEmpty(ToDate) ||
                                   (workflows.ActionDate >= dtStartDate &&
                                    workflows.ActionDate <= dtToDate
                                    ))
                            select workflows).ToList();

            IPagedList<Workflow> model = listWorkflow.ToPagedList(page ?? 1, 10);
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetControlList(string searchText = null)
        {
            try
            {
                var data = (from controls in db.Controls
                            join workflows in db.Workflows on controls.ControlID
                            equals workflows.ControlID
                            group controls by controls.ControlID into g
                            select g.FirstOrDefault()).ToList();

                var listControls = data.Where(x => x.ControlName.Contains(searchText.Trim())).Select(x => new
                {
                    ControlID = x.ControlID,
                    ControlName = x.ControlName
                }).ToList();

                return Json(listControls, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}