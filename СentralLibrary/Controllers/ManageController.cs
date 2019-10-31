namespace СentralLibrary.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Ninject;
    using PagedList;
    using СentralLibrary_Db.Models;

    /// <summary>
    /// Manage account.
    /// </summary>
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IBookRepository _bookRepository;
        private IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class.
        /// </summary>
        public ManageController()
        {
            IKernel ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IBookRepository>().To<BookRepository>();
            this._bookRepository = ninjectKernel.Get<IBookRepository>();
            ninjectKernel.Bind<IUserRepository>().To<UserRepository>();
            this._userRepository = ninjectKernel.Get<IUserRepository>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="signInManager">sign in manager.</param>
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => this._signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => this._signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => this._userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => this._userManager = value;
        }

        /// <summary>
        /// Library personal account.
        /// </summary>
        /// <returns>View a ManageLibrary.</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ManageLibrary(int? page, string sort, string search)
        {
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.Search = search;

            int _pageSize = 5;
            int _pageNumber = page ?? 1;

            ICollection<Registration> booksOfUser = await this._userRepository.GetRegistrations(this.User.Identity.GetUserId(), sort, search).ConfigureAwait(false);

            return this.View(booksOfUser.ToPagedList(_pageNumber, _pageSize));
        }

        /// <summary>
        /// Get method add a book to the library.
        /// </summary>
        /// <returns>View an AddBook.</returns>
        [HttpGet]
        [Authorize(Roles = "admin, moderator")]
        public ActionResult AddBook()
        {
            return this.View();
        }

        /// <summary>
        /// Post method add a book to the library.
        /// </summary>
        /// <param name="createBook">parameters in a book.</param>
        /// <returns>View Index.</returns>
        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<RedirectToRouteResult> AddBook(CreateBookViewModels createBook, HttpPostedFileBase uploadImage)
        {
            var book = new Book() { Name = createBook.Name, Author = createBook.Author, Blocked = false, DateOfPublication = createBook.DateOfPublication, CountOfBooks = createBook.CountOfBooks, Genre = createBook.Genre, Publication = createBook.Publication };
            await this._bookRepository.CreateBook(book, uploadImage).ConfigureAwait(false);

            return this.RedirectToRoute(new { controller = "Home", action = "Books" });
        }

        /// <summary>
        /// Iterates over all users.
        /// </summary>
        /// <returns>View with users.</returns>
        public async Task<ActionResult> AllUsers(int? page, string sort, string search)
        {
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.Search = search;

            int _pageSize = 10;
            int _pageNumber = page ?? 1;

            ICollection<ApplicationUser> users = await this._userRepository.AllUsers(sort, search).ConfigureAwait(false);

            return this.View(users.ToPagedList(_pageNumber, _pageSize));
        }

        /// <summary>
        /// Unbluk a user by Id.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns>View a AllUser.</returns>
        [HttpPost]
        [Authorize(Roles ="admin, moderator")]
        public async Task<ActionResult> UnblockUser(string userId)
        {
            await this._userRepository.UnblockAUser(userId).ConfigureAwait(false);
            return this.RedirectToRoute(new { controller = "Manage", action = "AllUsers" });
        }

        /// <summary>
        /// Block a user by Id.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns>View a AllUser.</returns>
        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> BlockUser(string userId)
        {
            await this._userRepository.BlockAUser(userId).ConfigureAwait(false);
            return this.RedirectToRoute(new { controller = "Manage", action = "AllUsers" });
        }

        /// <summary>
        /// Book unregistration per person.
        /// </summary>
        /// <param name="registrationId">Id of user.</param>
        /// <param name="bookId">Id of book.</param>
        /// <returns>View a ManageLibrary.</returns>
        [HttpPost]
        [Authorize]
        public async Task<RedirectToRouteResult> BookUnregistrationPerPerson(string registrationId, string bookId)
        {
            if (string.IsNullOrEmpty(registrationId) || string.IsNullOrEmpty(bookId))
            {
                return this.RedirectToRoute(new { controller = "Manage", action = "ManageLibrary" });
            }
            else
            {
                await this._userRepository.BookUnregistration(registrationId, bookId).ConfigureAwait(false);
                return this.RedirectToRoute(new { controller = "Manage", action = "ManageLibrary" });
            }
        }

        /// <summary>
        /// Manage Moderator.
        /// </summary>
        /// <returns>View a ManageModerator.</returns>
        [HttpGet]
        [Authorize(Roles ="admin")]
        public ActionResult ManageModerator()
        {
            return this.View();
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="message">Manage Message of Id.</param>
        /// <returns>View a Index.</returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : string.Empty;

            var userId = this.User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = this.HasPassword(),
                Logins = await this.UserManager.GetLoginsAsync(userId).ConfigureAwait(false),
                BrowserRemembered = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId).ConfigureAwait(false),
            };
            return this.View(model);
        }
        /// <summary>
        /// Remove Login.
        /// </summary>
        /// <param name="loginProvider">Login provider.</param>
        /// <param name="providerKey">Provider key.</param>
        /// <returns>View a RemoveLogin.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await this.UserManager.RemoveLoginAsync(this.User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey)).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId()).ConfigureAwait(false);
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                }

                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return this.RedirectToAction("ManageLogins", new { Message = message });
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <returns>View a ChangePassword.</returns>
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        /// <summary>
        /// Change Password.
        /// </summary>
        /// <param name="model">Model a ChangePasswordViewModel.</param>
        /// <returns>View a ChangePassword.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = await this.UserManager.ChangePasswordAsync(this.User.Identity.GetUserId(), model.OldPassword, model.NewPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId()).ConfigureAwait(false);
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                }

                return this.RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            this.AddErrors(result);
            return this.View(model);
        }

        /// <summary>
        /// Set a password.
        /// </summary>
        /// <returns>View a SetPassword.</returns>
        public ActionResult SetPassword()
        {
            return this.View();
        }

        /// <summary>
        /// Set password.
        /// </summary>
        /// <param name="model">Model a SetPasswordViewModel.</param>
        /// <returns>View a SetPassword.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.UserManager.AddPasswordAsync(this.User.Identity.GetUserId(), model.NewPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId()).ConfigureAwait(false);
                    if (user != null)
                    {
                        await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                    }

                    return this.RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }

                this.AddErrors(result);
            }

            return this.View(model);
        }

        /// <summary>
        /// Manage logins.
        /// </summary>
        /// <param name="message">Manage message of Id.</param>
        /// <returns>View a ManageLogins.</returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : string.Empty;
            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId()).ConfigureAwait(false);
            if (user == null)
            {
                return this.View("Error");
            }

            var userLogins = await this.UserManager.GetLoginsAsync(this.User.Identity.GetUserId()).ConfigureAwait(false);
            var otherLogins = this.AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            this.ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
            });
        }

        /// <summary>
        /// Link login.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <returns>View a LinkLogin.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            return new AccountController.ChallengeResult(provider, this.Url.Action("LinkLoginCallback", "Manage"), this.User.Identity.GetUserId());
        }

        /// <summary>
        /// Link Login Call back.
        /// </summary>
        /// <returns>View a LinkLoginCallback.</returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, this.User.Identity.GetUserId()).ConfigureAwait(false);
            if (loginInfo == null)
            {
                return this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }

            var result = await this.UserManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login).ConfigureAwait(false);
            return result.Succeeded ? this.RedirectToAction("ManageLogins") : this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this._userManager != null)
            {
                this._userManager.Dispose();
                this._userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}