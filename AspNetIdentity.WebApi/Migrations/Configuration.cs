using AspNetIdentity.WebApi.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetIdentity.WebApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AspNetIdentity.WebApi.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AspNetIdentity.WebApi.Infrastructure.ApplicationDbContext context)
        {
            AddDefaultUser(context);
        }

        private void AddDefaultUser(ApplicationDbContext context)
        {   
            var administrator = new ApplicationUser
            {
                UserName = "cheranga",
                Email = "cheranga@gmail.com",
                EmailConfirmed = true,
                FirstName = "Cheranga",
                LastName = "Hatangala",
                Level = 1,
                JoinDate = DateTime.Now.AddDays(-1)
            };
            //
            // Create the user manager instance, and use it to save the administrator
            //
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            userManager.Create(administrator);
        }
    }
}
