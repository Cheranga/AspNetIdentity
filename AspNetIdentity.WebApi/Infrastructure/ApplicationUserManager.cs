using System;
using AspNetIdentity.WebApi.Services;
using AspNetIdentity.WebApi.Validators;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace AspNetIdentity.WebApi.Infrastructure
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var applicationContext = context.Get<ApplicationDbContext>();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(applicationContext));

            /*
             * In here you can register custom user/password policies as follows
             *             
            userManager.UserValidator = new CustomUserValidator(userManager)
            {
                RequireUniqueEmail = false
            };

            userManager.PasswordValidator = new CustomPasswordValidator
            {
                RequireUppercase = true,
                RequireDigit = true,
                RequiredLength = 8,
                CompanyDomain = "CCHat Solutions"
            };
            */

            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                RequireUniqueEmail = false
            };
            //
            // Setup email service
            //
            SetupEmail(userManager, options);

            return userManager;
        }

        private static void SetupEmail(ApplicationUserManager userManager, IdentityFactoryOptions<ApplicationUserManager> options)
        {
            userManager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = TimeSpan.FromHours(6)
                };
            }
        }
    }
}