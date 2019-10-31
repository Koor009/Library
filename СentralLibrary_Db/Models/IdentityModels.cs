namespace СentralLibrary_Db.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Interaction of objects with databases.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Generate user identity.
        /// </summary>
        /// <param name="manager">Pass user parameters.</param>
        /// <returns><see cref="Task{TResult}"/> Representing the result of the asynchronous operation.</returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie).ConfigureAwait(false);
            return userIdentity;
        }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public bool? StatusBlockedUser { get; set; }

        public string Country { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Sity { get; set; }

        public string StreetAddress { get; set; }

        public ICollection<Registration> RegistrationOfBook { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUser"/> class.
        /// </summary>
        public ApplicationUser() => this.RegistrationOfBook = new List<Registration>();
    }

    /// <summary>
    /// Get access to the data of the corresponding models.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Book> Books { get; set; }

        public DbSet<Registration> Registrations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// Signin manager to use a single instance per request.
        /// </summary>
        /// <returns>Context database.</returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}