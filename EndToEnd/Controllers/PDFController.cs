using EndToEnd.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EndToEnd.Controllers
{
    public class WorkflowSearch
    {
        public string ControlName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ActionBy { get; set; }
        public string StatusFrom { get; set; }
        public string StatusTo { get; set; }
        public int PageNumber { get; set; }
    }
    public class PDFController : Controller
    {
        private AudissEntities db = new AudissEntities();
        [HttpPost]
        public ActionResult CurrentPageDownload(WorkflowSearch objWorkFlow)
        {
            return new ActionAsPdf("DownloadPagePDF", new
            {
                ControlName = objWorkFlow.ControlName,
                FromDate = objWorkFlow.FromDate,
                ToDate = objWorkFlow.ToDate,
                ActionBy = objWorkFlow.ActionBy,
                StatusFrom = objWorkFlow.StatusFrom,
                StatusTo = objWorkFlow.StatusTo,
                page = objWorkFlow.PageNumber

            })
            { FileName = "Reports.pdf" };
        }

    
        [HttpGet]
        public ActionResult DownloadPagePDF(string ControlName = "", string FromDate = "", string ToDate = "", string ActionBy = "", string StatusFrom = "", string StatusTo = "", int page = 1)
        {
            List<Workflow> listWorkflow = null;
            DateTime? dtStartDate = null;
            DateTime? dtToDate = null;
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                dtStartDate = Convert.ToDateTime(FromDate).Date;
                dtToDate = Convert.ToDateTime(ToDate).Date;
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
            
            return View(listWorkflow);
        }
    }
}