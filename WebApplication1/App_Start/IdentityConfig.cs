using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WebApplication1.Models;

namespace WebApplication1
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationCustomerManager : UserManager<Customer>
    {
        public ApplicationCustomerManager(IUserStore<Customer> store): base(store) { }

        public static ApplicationCustomerManager Create(IdentityFactoryOptions<ApplicationCustomerManager> options, IOwinContext context) 
        {
            var manager = new ApplicationCustomerManager(new UserStore<Customer>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<Customer>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireDigit = true,
                RequireNonLetterOrDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            return manager;
        }

        public async Task<IdentityResult> CreateAsync(Customer user, string password, string[] roles)
        {
            try
            {
                // create customer
                var result = await CreateAsync(user, password);

                //  add roles for customer
                if (result.Succeeded)
                {
                    result = await AddToRolesAsync(user.Id, roles);
                }

                return result;
            }
            catch (DbEntityValidationException e)
            {
                return IdentityResult.Failed(e.EntityValidationErrors.SelectMany(error => error.ValidationErrors.Select(err => err.ErrorMessage)).ToArray());
            }
        }

        public async Task<IdentityResult> UpdateAsync(Customer user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userToUpdate = await FindByIdAsync(user.Id);

            userToUpdate.UserName = user.UserName;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Email = user.Email;
            userToUpdate.PhoneNumber = user.PhoneNumber;
            userToUpdate.IsDisabled = user.IsDisabled;
            userToUpdate.ChangedDate = user.ChangedDate;
            userToUpdate.ChangerName = user.ChangerName;

            var result = await UpdateAsync(userToUpdate);

            if (result.Succeeded && password != null)
            {
                // update customer password
                result = await UpdatePasswordAsync(user, password);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(Customer user, string password, string[] roles)
        {
            // update customer
            var result = await UpdateAsync(user, password);

            // update customer roles
            if (result.Succeeded)
            {
                result = await UpdateRolesAsync(user.Id, roles);
            }

            return result;
        }

        public async Task<IdentityResult> UpdatePasswordAsync(Customer user, string newPassword)
        {
            var store = Store as UserStore<Customer>;

            if (PasswordValidator != null)
            {
                var passwordResult = await PasswordValidator.ValidateAsync(newPassword);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }
            }

            var newPasswordHash = PasswordHasher.HashPassword(newPassword);

            await store.SetPasswordHashAsync(user, newPasswordHash);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateRolesAsync(string userId, string[] roles)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var oldUserRoles = await GetRolesAsync(/*????????????*/userId);
            var rolesToRemove = oldUserRoles.Where(role => !roles.Contains(role)).ToArray();
            var rolesToAdd = roles.Where(role => !oldUserRoles.Contains(role)).ToArray();

            var result = await RemoveFromRolesAsync(userId, rolesToRemove);
            if (result.Succeeded)
            {
                result = await AddToRolesAsync(userId, rolesToAdd);
            }

            return result;
        }

        public async Task<IdentityResult> DeleteAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await DeleteAsync(await FindByIdAsync(userId));
        }

        public IEnumerable<SimpleCustomerViewModel> GetCustomersSimple(bool idAllowed)
        {
            return Users.ToList().Select(user => new SimpleCustomerViewModel()
            {
                CustomerId = idAllowed ? user.Id : null,
                Login = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Active = !user.IsDisabled,
            });
        }

    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<Customer, string>
    {
        public ApplicationSignInManager(ApplicationCustomerManager userManager, IAuthenticationManager authenticationManager): base(userManager, authenticationManager) { }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationCustomerManager>(), context.Authentication);
        }
    }

    // Configure the application role manager used in this application.
    public class ApplicationRoleManager: RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store): base(store) { }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}
