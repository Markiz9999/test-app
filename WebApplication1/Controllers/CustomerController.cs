using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Providers;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private ApplicationCustomerManager _customerManager;
        private ApplicationRoleManager _roleManager;

        private bool isEditAllowed => User.IsInRole("administrator") || User.IsInRole("manager");

        public ApplicationCustomerManager CustomerManager
        {
            get => _customerManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationCustomerManager>();
            private set => _customerManager = value;
        }

        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }

        public CustomerController() { }

        public CustomerController(ApplicationCustomerManager customerManager, ApplicationRoleManager roleManager)
        {
            _customerManager = customerManager;
            _roleManager = roleManager;
        }

        public ActionResult Get()
        {
            return Json(CustomerManager.GetCustomersSimple(isEditAllowed), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPage(int id, PaginationViewModel model)
        {
            var items = CustomerManager.GetCustomersSimple(isEditAllowed);
            int pageSize = AppSettings.PageSize;

            if (!string.IsNullOrEmpty(model.SearchString))
            {
                items = items.Where(x =>
                    x.Login.ToLower().Contains(model.SearchString.ToLower())
                    || x.FirstName.ToLower().Contains(model.SearchString.ToLower())
                    || x.LastName.ToLower().Contains(model.SearchString.ToLower())
                    || x.Email.ToLower().Contains(model.SearchString.ToLower())
                    || x.Phone.ToLower().Contains(model.SearchString.ToLower()));
            }

            int itemsCount = items.Count();
            // check that page in range, otherwise return first or last page 
            int pageNumber = Math.Min(Math.Max(id, 1), (int)Math.Ceiling((double)itemsCount / pageSize));

            switch(model.SortBy)
            {
                case "login":
                    items = model.Desc ? items.OrderByDescending(x => x.Login) : items.OrderBy(x => x.Login);
                    break;
                case "name":
                    items = model.Desc ? items.OrderByDescending(x => x.FirstName).ThenBy(x => x.LastName) : items.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
                    break;
                case "email":
                    items = model.Desc ? items.OrderByDescending(x => x.Email) : items.OrderBy(x => x.Email);
                    break;
                case "phone":
                    items = model.Desc ? items.OrderByDescending(x => x.Phone) : items.OrderBy(x => x.Phone);
                    break;
                case "active":
                    items = model.Desc ? items.OrderByDescending(x => x.Active) : items.OrderBy(x => x.Active);
                    break;
            }

            return Json(new CustomersPageModel {
                PageSize = pageSize,
                ItemsCount = itemsCount,
                PageNumber = pageNumber,
                Customers = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            ViewBag.Roles = GetRoles();
            return View("Customer");
        }

        // POST: Customer/Create
        [HttpPost]
        public async Task<ActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = CustomerManager.FindById(User.Identity.GetUserId());
                var user = new Customer
                {
                    UserName = model.Login,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    IsDisabled = !model.Active,
                    CreatedDate = DateTime.UtcNow,
                    CreatorName = currentUser.FirstName,
                };
                IdentityResult result = await CustomerManager.CreateAsync(user, model.Password, model.Roles);
                if (result.Succeeded)
                {
                    return RedirectToAction("Customers", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Roles = GetRoles(model.Roles);
            return View("Customer", model);
        }

        [HttpGet]
        [Authorize(Roles = "administrator, manager")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id != null)
            {
                var user = await CustomerManager.FindByIdAsync(id);
                if (user != null)
                {
                    var viewModel = new CustomerViewModel
                    {
                        CustomerId = user.Id,
                        Login = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        Active = !user.IsDisabled,
                        CreatedDate = user.CreatedDate,
                        CreatorName = user.CreatorName,
                        ChangedDate = user.ChangedDate,
                        ChangerName = user.ChangerName,
                    };

                    ViewBag.Roles = GetRoles(user.Roles);
                    return View("Customer", viewModel);
                }
            }
            return RedirectToAction("Create", "Customer");
        }

        [HttpPost]
        [Authorize(Roles = "administrator, manager")]
        public async Task<ActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = CustomerManager.FindById(User.Identity.GetUserId());
                var user = new Customer
                {
                    Id = model.CustomerId,
                    UserName = model.Login,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    IsDisabled = !model.Active,
                    ChangedDate = DateTime.UtcNow,
                    ChangerName = User.Identity.GetUserId() == model.CustomerId
                        ? model.FirstName
                        : currentUser.FirstName,
                };
                IdentityResult result = await CustomerManager.UpdateAsync(user, model.Password, model.Roles);
                if (result.Succeeded)
                {
                    return RedirectToAction("Customers", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Roles = GetRoles(model.Roles);
            return View("Customer", model);
        }

        [HttpPost]
        [Authorize(Roles = "administrator, manager")]
        public async Task<JsonResult> Delete(string id)
        {
            if (User.Identity.GetUserId() == id)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest), "You cannot delete yourself");
            }

            var result = await CustomerManager.DeleteAsync(id);

            if (!result.Succeeded)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest), string.Join("\n", result.Errors));
            }

            return Json(new HttpStatusCodeResult(HttpStatusCode.OK));
        }

        [HttpPost]
        [Authorize(Roles = "administrator, manager")]
        public async Task<JsonResult> DeleteRange(string[] ids)
        {
            List<string> errors = new List<string>();

            foreach(var id in ids)
            {
                if (User.Identity.GetUserId() == id)
                {
                    errors.Add("You cannot delete yourself");
                    continue;
                }

                var result = await CustomerManager.DeleteAsync(id);

                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }

            if (errors.Count > 0)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest), string.Join("\n", errors));
            }

            return Json(new HttpStatusCodeResult(HttpStatusCode.OK));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_customerManager != null)
                {
                    _customerManager.Dispose();
                    _customerManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private List<SelectListItem> GetRoles()
        {
            return RoleManager.Roles.ToList().Select(role => new SelectListItem()
            {
                Value = role.Name,
                Text = role.Name,
            }).ToList();
        }

        private List<SelectListItem> GetRoles(IEnumerable<IdentityUserRole> userRoles)
        {
            if (userRoles == null)
            {
                return GetRoles();
            }

            return RoleManager.Roles.ToList().Select(role => new SelectListItem()
            {
                Value = role.Name,
                Text = role.Name,
                Selected = userRoles.Any(r => r.RoleId == role.Id),
            }).ToList();
        }

        private List<SelectListItem> GetRoles(IEnumerable<string> userRoles)
        {
            if (userRoles == null)
            {
                return GetRoles();
            }

            return RoleManager.Roles.ToList().Select(role => new SelectListItem()
            {
                Value = role.Name,
                Text = role.Name,
                Selected = userRoles.Any(r => r == role.Name),
            }).ToList();
        }

        #endregion
    }
}
