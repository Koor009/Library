namespace СentralLibrary_Db.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// class.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// </summary>
        public BookRepository()
        {
        }

        /// <summary>
        /// Add a book with the whole description.
        /// </summary>
        /// <param name="book">Fields of instance class Book.</param>
        public async Task CreateBook(Book book, HttpPostedFileBase uploadImage)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    book.Image = this.ConvertImageToByte(uploadImage);

                    db.Books.Add(book);

                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get the whole sort list of books.
        /// </summary>
        /// <param name="sort">sort by item.</param>
        /// <returns>List sort of books.</returns>
        public async Task<ICollection<Book>> SortBooks(string sort, string search)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Book> books;
                if (!string.IsNullOrEmpty(search))
                {
                    books = await db.Books.Where(n => n.Name.IndexOf(search) > -1).ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    books = await db.Books.ToListAsync().ConfigureAwait(false);
                }

                switch (sort)
                {
                    case "name A-Z":
                        return books.OrderBy(n => n.Name).ToList();
                    case "name Z-A":
                        return books.OrderByDescending(n => n.Name).ToList();
                    case "author A-Z":
                        return books.OrderBy(n => n.Author).ToList();
                    case "author Z-A":
                        return books.OrderByDescending(n => n.Author).ToList();
                    case "publication 0-9":
                        return books.OrderBy(n => n.Publication).ToList();
                    case "publication 9-0":
                        return books.OrderByDescending(n => n.Publication).ToList();
                    case "date old-new":
                        return books.OrderBy(n => n.DateOfPublication).ToList();
                    case "date new-old":
                        return books.OrderByDescending(n => n.DateOfPublication).ToList();
                    default:
                        return books.OrderBy(n => n.Name).ToList();
                }
            }
        }

        /// <summary>
        /// Change parameters and descriptions a book.
        /// </summary>
        /// <param name="book">Fields of instance class Book.</param>
        /// <returns><see cref="Task"/>Representing the asynchronous operation.</returns>
        public async Task UpdateBook(Book book, HttpPostedFileBase uploadImage)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    Book upBook = await db.Books.Where(b => b.Id == book.Id).FirstAsync().ConfigureAwait(false);

                    if (uploadImage != null)
                    {
                        book.Image = this.ConvertImageToByte(uploadImage);
                        upBook.Image = book.Image;
                    }

                    upBook.Name = book.Name;
                    upBook.Author = book.Author;
                    upBook.Publication = book.Publication;
                    upBook.CountOfBooks = book.CountOfBooks;
                    upBook.DateOfPublication = book.DateOfPublication;

                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex) { throw ex; }
            }
        }

        /// <summary>
        /// Get the whole list of books.
        /// </summary>
        /// <returns>List of books.</returns>
        public async Task<ICollection<Book>> GetBooks()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Books.ToListAsync().ConfigureAwait(false);
                }
                catch (Exception ex) { throw ex; }
            }
        }

        /// <summary>
        /// Get information about a book.
        /// </summary>
        /// <param name="bookId">Pass an id of the book.</param>
        /// <returns>Book with description.</returns>
        public async Task<Book> InformationBook(string bookId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return await db.Books.Where(b => b.Id.ToString() == bookId).FirstAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Search by author and book name.
        /// </summary>
        /// <param name="name">Name book.</param>
        /// <param name="author">Author's name.</param>
        /// <returns>List of books by search criteria.</returns>
        public async Task<ICollection<Book>> BookSearch(string name, string author)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return await db.Books.Where(n => n.Name == name).Where(a => a.Author == author).ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private byte[] ConvertImageToByte(HttpPostedFileBase uploadImage)
        {
            byte[] imageData = null;
            if (uploadImage != null)
            {
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                return imageData;
            }

            return imageData;
        }
    }
}