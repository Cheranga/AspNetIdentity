using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRoleAsync(string id)
        {
            var identityRole = await ApplicationRoleManager.FindByIdAsync(id);
            if (identityRole == null)
            {
                return NotFound();
            }

            return Ok(ModelFactory.Create(identityRole));
        }

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var identityRoles = ApplicationRoleManager.Roles.ToList();
            
            return Ok(identityRoles.Select(x=>ModelFactory.Create(x)));
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateAsync(CreateRoleBindingDto model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var role = new IdentityRole {Name = model.Name};
            var result = await ApplicationRoleManager.CreateAsync(role);

            return result.Succeeded ? Created(new Uri(Url.Link("GetRoleById", new {role.Id})), role) : GetErrorResult(result);
        }

        public async Task<IHttpActionResult> DeleteRoleAsync(string id)
        {
            var role = await ApplicationRoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await ApplicationRoleManager.DeleteAsync(role);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [Route("manageusers")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleDto model)
        {
            var role = await ApplicationRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (var user in model.EnrolledUsers)
            {
                var appUser = await ApplicationUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                if (ApplicationUserManager.IsInRole(user, role.Name) == false)
                {
                    var result = await ApplicationUserManager.AddToRoleAsync(user, role.Name);

                    if (result.Succeeded == false)
                    {
                        ModelState.AddModelError("", $"User: {user} could not be added to role");
                    }

                }
            }

            foreach (var user in model.RemovedUsers)
            {
                var appUser = await ApplicationUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                var result = await ApplicationUserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be removed from role");
                }
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}