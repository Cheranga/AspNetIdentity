using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/claims")]
    public class ClaimsController : BaseApiController
    {
        [Route("")]
        [Authorize]
        public IHttpActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            var claims = identity.Claims.Select(x => new
            {
                Subject = x.Subject.Name,
                Type = x.Type,
                Value = x.Value
            });

            return Ok(claims);
        }
    }
}