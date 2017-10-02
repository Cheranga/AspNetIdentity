using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            var users = ApplicationUserManager.Users.ToList();
            var userDtos = users.Select(x => ModelFactory.Create(x));

            return Ok(userDtos);
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var user = await ApplicationUserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = ModelFactory.Create(user);
            return Ok(userDto);
        }

        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await ApplicationUserManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = ModelFactory.Create(user);
            return Ok(userDto);
        }

        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateUserDto user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            //
            // Create the user with the given password
            //
            var applicationUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date
            };

            var result = await ApplicationUserManager.CreateAsync(applicationUser, user.Password);

            if (result.Succeeded)
            {
                //
                // Set the confirmation email settings
                //
                var code = await ApplicationUserManager.GenerateEmailConfirmationTokenAsync(applicationUser.Id);
                var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new {userId = applicationUser.Id, code=code}));
                await ApplicationUserManager.SendEmailAsync(applicationUser.Id, "Confirm Your Account", $"Click this {callbackUrl} to confirm");


                var locationHeader = new Uri(Url.Link("GetUserById", new {id = applicationUser.Id}));
                var userDto = ModelFactory.Create(applicationUser);

                return Created(locationHeader, userDto);
            }

            return GetErrorResult(result);
        }

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            //
            // Get the current user id, and change the password
            //
            var result = await ApplicationUserManager.ChangePasswordAsync(User.Identity.GetUserId(), changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var user = await ApplicationUserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await ApplicationUserManager.DeleteAsync(user);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }


        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User id and code are required");
                return BadRequest(ModelState);
            }

            var result = await ApplicationUserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return Ok();
            }


            return GetErrorResult(result);
        }
    }
}