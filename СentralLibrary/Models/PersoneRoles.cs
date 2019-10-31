namespace СentralLibrary.Models
{
    using System;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using СentralLibrary_Db.Models;

    /// <summary>
    /// The role of man in the website.
    /// </summary>
    public class PersoneRoles : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        /// <inheritdoc/>
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            roleManager.Create(new IdentityRole { Name = "admin" });
            roleManager.Create(new IdentityRole { Name = "user" });
            roleManager.Create(new IdentityRole { Name = "moderator" });

            var admin = new ApplicationUser { Email = "admin@gmail.com", UserName = "admin@gmail.com", DateOfBirth = DateTime.UtcNow };
            string password = "ad46D_ew2r3w";
            var result = userManager.Create(admin, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(admin.Id, "admin");
            }

            base.Seed(context);
        }
    }
}