using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelperController : Controller
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

        public HelperController() { }

        public HelperController(ApplicationCustomerManager customerManager, ApplicationRoleManager roleManager)
        {
            _customerManager = customerManager;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> FillBaseRoles()
        {
            if (RoleManager.Roles.Count() > 0)
            {
                return Content("Base roles already exist");
            }

            var result = await RoleManager.CreateAsync(new IdentityRole("administrator"));
            result = result.Succeeded ? await RoleManager.CreateAsync(new IdentityRole("customer")) : result;
            result = result.Succeeded ? await RoleManager.CreateAsync(new IdentityRole("operator")) : result;
            result = result.Succeeded ? await RoleManager.CreateAsync(new IdentityRole("manager")) : result;

            if (!result.Succeeded)
            {
                return Content(string.Join("<br/>", result.Errors));
            }

            return Content("Default roles have been added successfully");
        }

        public async Task<ActionResult> AddDefaultAdministrator()
        {
            if (CustomerManager.Users.Count() > 0)
            {
                return Content("The user list is not empty");
            }

            var user = new Customer
            {
                UserName = "admin",
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@email.com",
                PhoneNumber = "+380987654321",
                IsDisabled = false,
            };

            var result = await CustomerManager.CreateAsync(user, "Admin_1234", new[] { "administrator" });

            if (!result.Succeeded)
            {
                return Content(string.Join("<br/>", result.Errors));
            }

            return Content("The default admin was added successfully");
        }

        public async Task<ActionResult> GenerateCustomers()
        {
            for (int i = 0; i < 20; i++)
            {
                var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

                var user = new Customer
                {
                    UserName = uid,
                    FirstName = "User",
                    LastName = "User",
                    Email = "user@email.com",
                    PhoneNumber = "+380987654321",
                    IsDisabled = false,
                };

                var result = await CustomerManager.CreateAsync(user, "User_1234", new[] { "administrator" });

                if (!result.Succeeded)
                {
                    return Content(string.Join("<br/>", result.Errors));
                }
            }

            return Content("The users was added successfully");
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