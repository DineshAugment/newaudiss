#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using EndToEnd.Models;
using PagedList;

#endregion Includes

namespace EndToEnd.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationUserManager _userManager;
        private AudissEntities db = new AudissEntities();

        #region public ApplicationUserManager UserManager
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
        #endregion
        // GET: Search
        #region public ActionResult Index(string searchString)
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
           try
            {
                ViewBag.searchString = searchString;
                int intPage = 1;
                int intPageSize = 5;
                int intTotalPageCount = 0;

                if (searchString != null)
                {
                    intPage = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchString = currentFilter;
                        intPage = page ?? 1;
                    }
                    else
                    {
                        searchString = "";
                        intPage = page ?? 1;
                    }
                }

                ViewBag.CurrentFilter = searchString;

                var ctrl = db.Controls.OrderBy(q => q.ControlID).ToList();

                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();
                List<ControlDTO> col_ControlrDTO = new List<ControlDTO>();
                int intSkip = (intPage - 1) * intPageSize;

                intTotalPageCount =ctrl.Count();

                var result = ctrl.Where(x => x.Description.Contains(searchString))
                    .OrderBy(x => x.ControlID)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();

                foreach (var item in result)
                {
                    ControlDTO objUserDTO = new ControlDTO();

                    objUserDTO.CustomerName = item.CustomerName;
                    objUserDTO.Description = item.Description;
                    objUserDTO.Status = item.Status;
                    objUserDTO.ControlType = item.ControlType;
                    objUserDTO.CreatedDate = item.CreatedDate;

                    col_ControlrDTO.Add(objUserDTO);

                }

                // Set the number of pages
                var _UserDTOAsIPagedList =
                    new StaticPagedList<ControlDTO>
                    (
                      col_ControlrDTO, intPage, intPageSize, intTotalPageCount
                        );

                return View(_UserDTOAsIPagedList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();

                return View(col_UserDTO.ToPagedList(1, 25));
            }
        }
        #endregion
    }

    
}