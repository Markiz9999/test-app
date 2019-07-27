using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Providers;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationCustomerManager _customerManager;
        private ApplicationRoleManager _roleManager;

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

        public HomeController() { }

        public HomeController(ApplicationCustomerManager customerManager, ApplicationRoleManager roleManager)
        {
            _customerManager = customerManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Customers", "Home");
        }

        [HttpGet]
        public ActionResult Customers()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CustomerRoles()
        {
            return View();
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
    }
}