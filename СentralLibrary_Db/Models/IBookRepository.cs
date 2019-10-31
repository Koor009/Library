namespace СentralLibrary_Db.Models
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;

    public interface IBookRepository
    {
        Task<ICollection<Book>> BookSearch(string name, string author);

        Task<ICollection<Book>> SortBooks(string sort, string search);

        Task CreateBook(Book book, HttpPostedFileBase uploadImage);

        Task<ICollection<Book>> GetBooks();

        Task<Book> InformationBook(string bookId);

        Task UpdateBook(Book book, HttpPostedFileBase uploadImage);
    }
}