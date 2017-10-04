using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AspNetIdentity.WebApi.Infrastructure
{
    public class ExtendedClaimsProvider
    {
        public static IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            if (user == null)
            {
                return null;
            }

            var isFullTimeEmployee = (DateTime.UtcNow - user.JoinDate.ToUniversalTime()).TotalDays > 90 ? "1" : "0";
            
            return new List<Claim>
            {
                CreateClaim("FTE", isFullTimeEmployee)
            };
        }

        public static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }
    }
}