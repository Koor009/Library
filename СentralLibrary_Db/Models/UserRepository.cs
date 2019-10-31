namespace СentralLibrary_Db.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Register a book to an user by bookId and userId.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <param name="bookId">Id of book.</param>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public async Task AddABookToUser(string userId, string bookId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Book book = await db.Books.Where(b => b.Id.ToString() == bookId).FirstAsync().ConfigureAwait(false);

                    if (book.CountOfBooks > 0 && book.Blocked == false)
                    {
                        --book.CountOfBooks;
                        Book currentBook = db.Books.Where(b => b.Id.ToString() == bookId).First();
                        ApplicationUser currentUser = db.Users.Where(u => u.Id.ToString() == userId && u.StatusBlockedUser == false).First();
                        if (currentUser != null)
                        {
                            db.Registrations.Add(new Registration() { Date = DateTime.UtcNow, Availability = true, UserId = userId, BookId = bookId, GetBook = currentBook, ApplicationUser = currentUser });

                            await db.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Blok a user by Id.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public async Task BlockAUser(string userId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ApplicationUser user = await db.Users.Where(u => u.Id.ToString() == userId).FirstAsync().ConfigureAwait(false);
                    if (user != null)
                    {
                        user.StatusBlockedUser = true;

                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Unblok a user by Id.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public async Task UnblockAUser(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = await db.Users.Where(u => u.Id.ToString() == userId).FirstAsync().ConfigureAwait(false);
                if (user != null)
                {
                    user.StatusBlockedUser = false;

                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Searches for books that are registered to the user by id.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns>Registered book on the user.</returns>
        public async Task<ICollection<Registration>> GetRegistrations(string userId, string sort, string search)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    List<Registration> registrations;

                    if (!string.IsNullOrEmpty(search))
                    {
                        registrations = await db.Registrations.Include(b => b.GetBook).Include(u => u.ApplicationUser).Where(u => u.UserId == userId && u.Availability == true).Where(n => n.GetBook.Name.IndexOf(search) > -1).ToListAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        registrations = await db.Registrations.Include(b => b.GetBook).Include(u => u.ApplicationUser).Where(u => u.UserId == userId && u.Availability == true).ToListAsync().ConfigureAwait(false);
                    }

                    switch (sort)
                    {
                        case "name A-Z":
                            return registrations.OrderBy(n => n.GetBook.Name).ToList();
                        case "name Z-A":
                            return registrations.OrderByDescending(n => n.GetBook.Name).ToList();
                        case "author A-Z":
                            return registrations.OrderBy(n => n.GetBook.Author).ToList();
                        case "author Z-A":
                            return registrations.OrderByDescending(n => n.GetBook.Author).ToList();
                        case "registration old-new":
                            return registrations.OrderBy(n => n.Date).ToList();
                        case "registration new-old":
                            return registrations.OrderByDescending(n => n.Date).ToList();
                        default:
                            return registrations.OrderBy(n => n.GetBook.Name).ToList();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get all users in system.
        /// </summary>
        /// <returns>List of users.</returns>
        public async Task<ICollection<ApplicationUser>> AllUsers(string sort, string search)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ICollection<ApplicationUser> users;

                    if (!string.IsNullOrEmpty(search))
                    {
                        users = await db.Users.Where(u => u.FirstName != null).Where(u=>u.Surname == search).ToListAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        users = await db.Users.Where(u => u.FirstName != null).ToListAsync().ConfigureAwait(false);
                    }

                    switch (sort)
                    {
                        case "name A-Z":
                            return users.OrderBy(n => n.FirstName).ToList();
                        case "name Z-A":
                            return users.OrderByDescending(n => n.FirstName).ToList();
                        case "surname A-Z":
                            return users.OrderBy(n => n.Surname).ToList();
                        case "surname Z-A":
                            return users.OrderByDescending(n => n.Surname).ToList();
                        case "email A-Z":
                            return users.OrderBy(n => n.Email).ToList();
                        case "email Z-A":
                            return users.OrderByDescending(n => n.Email).ToList();
                        default:
                            return users.OrderBy(n => n.FirstName).ToList();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Book return to library and unregistration.
        /// </summary>
        /// <param name="registrationId">Id of registration.</param>
        /// <param name="bookId">Id of book.</param>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public async Task BookUnregistration(string registrationId, string bookId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Book book = await db.Books.Where(b => b.Id.ToString() == bookId).FirstAsync().ConfigureAwait(false);
                    Registration registration = await db.Registrations.Where(r => r.Id.ToString() == registrationId).FirstAsync().ConfigureAwait(false);

                    ++book.CountOfBooks;
                    registration.Availability = false;

                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception) { throw; }
        }
    }
}