using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using AspNetIdentity.WebApi.Infrastructure;
using Microsoft.AspNet.Identity;

namespace AspNetIdentity.WebApi.Validators
{
    /// <summary>
    /// A fictitious user validation policy which will force users to have a gmail account
    /// </summary>
    public class CustomUserValidator : UserValidator<ApplicationUser>
    {
        public CustomUserValidator(ApplicationUserManager manager) : base(manager)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
        {
            // Check for other validations
            var validationResult = await base.ValidateAsync(user);
            if (validationResult.Succeeded)
            {
                var emailDomain = user.Email.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[1];
                var isGoogle = string.Equals(emailDomain, "google", StringComparison.OrdinalIgnoreCase);

                return isGoogle ? validationResult :
                    new IdentityResult("The user must provide a gmail account for email");
            }

            return validationResult;
        }
    }
}