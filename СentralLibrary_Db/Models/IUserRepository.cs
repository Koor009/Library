namespace СentralLibrary_Db.Models
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task AddABookToUser(string userId, string bookId);

        Task<ICollection<Registration>> GetRegistrations(string userId, string sort, string search);

        Task<ICollection<ApplicationUser>> AllUsers(string sort, string search);

        Task UnblockAUser(string userId);

        Task BlockAUser(string userId);

        Task BookUnregistration(string registrationId, string bookId);
    }
}