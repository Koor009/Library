namespace СentralLibrary.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Ninject;
    using PagedList;
    using СentralLibrary_Db.Models;

    /// <summary>
    /// Class controller a HomeController.
    /// </summary>
    public class HomeController : Controller
    {
        private IBookRepository _bookRepository;
        private IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController()
        {
            IKernel ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IBookRepository>().To<BookRepository>();
            this._bookRepository = ninjectKernel.Get<IBookRepository>();
            ninjectKernel.Bind<IUserRepository>().To<UserRepository>();
            this._userRepository = ninjectKernel.Get<IUserRepository>();
        }

        /// <summary>
        /// Main Page.
        /// </summary>
        /// <returns>View a Index.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Get all books.
        /// </summary>
        /// <returns>Books.</returns>
        [HttpGet]
        public async Task<ActionResult> Books(int? page, string sort, string search)
        {
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.Search = search;

            int _pageSize = 5;
            int _pageNumber = page ?? 1;

            ICollection<Book> sortBooks = await this._bookRepository.SortBooks(sort, search).ConfigureAwait(false);

            return this.View(sortBooks.ToPagedList(_pageNumber, _pageSize));
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns>1.</returns>
        [HttpGet]
        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }

        /// <summary>
        /// Post method, book Registration Per Person.
        /// </summary>
        /// <param name="bookId">book id.</param>
        /// <returns>View Index.</returns>
        [HttpPost]
        [Authorize(Roles ="user")]
        public async Task<RedirectToRouteResult> BookRegistrationPerPerson(string bookId)
        {
            await this._userRepository.AddABookToUser(this.User.Identity.GetUserId(), bookId).ConfigureAwait(false);

            return this.RedirectToRoute(new { controller = "Home", action = "Books" });
        }

        /// <summary>
        /// Manage a Book.
        /// </summary>
        /// <param name="bookId">Id of book.</param>
        /// <returns>View a ManageBook.</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ManageBook(string bookId)
        {
            this.ViewBag.Id = bookId;
            var book = await this._bookRepository.InformationBook(bookId).ConfigureAwait(false);

            return this.View(book);
        }

        /// <summary>
        /// Post method a ManageBook.
        /// </summary>
        /// <param name="form">FormCollection.</param>
        /// <param name="uploadImage">HttpPostedFileBase.</param>
        /// <returns>View a ManageBook.</returns>
        [HttpPost]
        public async Task<RedirectToRouteResult> ManageBook(FormCollection form, HttpPostedFileBase uploadImage)
        {
            string _id = form["Id"];
            Guid id = Guid.Parse(_id);
            var book = new Book() { Id = id, Name = form["Name"], Author = form["Author"], Blocked = false, DateOfPublication = DateTime.Parse(form["DateOfPublication"]), CountOfBooks = Int32.Parse(form["CountOfBooks"]), Genre = form["Genre"], Publication = Int32.Parse(form["Publication"]) };
            await this._bookRepository.UpdateBook(book, uploadImage).ConfigureAwait(false);
            return this.RedirectToRoute(new { controller = "Home", action = "Books" });
        }
    }
}