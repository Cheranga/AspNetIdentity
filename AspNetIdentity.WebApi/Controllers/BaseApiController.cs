using System.Net.Http;
using System.Web.Http;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AspNetIdentity.WebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _applicationUserManager;
        private ModelFactory _modelFactory;

        protected ApplicationUserManager ApplicationUserManager
        {
            get { return _applicationUserManager ?? (_applicationUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>()); }
        }

        protected ModelFactory ModelFactory
        {
            get { return _modelFactory ?? (_modelFactory = new ModelFactory(Request, ApplicationUserManager)); }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.Succeeded)
            {
                return null;
            }

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return BadRequest(ModelState);
            }

            return ModelState.IsValid ? BadRequest() : null;
        }
    }
}