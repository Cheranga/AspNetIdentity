using System.Net.Http;
using System.Web.Http.Routing;
using AspNetIdentity.WebApi.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetIdentity.WebApi.Models
{
    public class ModelFactory
    {
        private readonly ApplicationUserManager _manager;
        private readonly UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager manager)
        {
            _urlHelper = new UrlHelper(request);
            _manager = manager;
        }

        public UserDto Create(ApplicationUser user)
        {
            return new UserDto
            {
                Url = _urlHelper.Link("GetUserById", new {id=user.Id}),
                Id = user.Id,
                UserName = user.UserName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Level = user.Level,
                JoinDate = user.JoinDate,
                Roles = _manager.GetRoles(user.Id),
                Claims = _manager.GetClaims(user.Id)
            };
        }

        public RoleReturnDto Create(IdentityRole appRole)
        {
            return new RoleReturnDto
            {
                Id = appRole.Id,
                Name = appRole.Name,
                Url = _urlHelper.Link("GetRoleById", new {id = appRole.Id})
            };
        }
    }
}