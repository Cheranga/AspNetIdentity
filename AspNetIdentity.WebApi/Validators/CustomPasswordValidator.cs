using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNetIdentity.WebApi.Validators
{
    /// <summary>
    ///     A fictitious password validator, which does not allow simple passwords
    /// </summary>
    public class CustomPasswordValidator : PasswordValidator
    {
        public string CompanyDomain { get; set; }

        public override async Task<IdentityResult> ValidateAsync(string password)
        {
            var validationResult = await base.ValidateAsync(password);
            if (validationResult.Succeeded)
            {
                var isPasswordWeak = string.Equals(password, "abcdef", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(password, "123456");

                var isCompanyDomainExcluded = string.IsNullOrWhiteSpace(CompanyDomain) ||
                                              new[] {"aaa", "bbb"}.Any(x => string.Equals(x, CompanyDomain, StringComparison.OrdinalIgnoreCase));


                return isPasswordWeak ? new IdentityResult("Password is too weak") :
                    isCompanyDomainExcluded ? new IdentityResult("Company domain is excluded") :
                        validationResult;
            }

            return validationResult;
        }
    }
}