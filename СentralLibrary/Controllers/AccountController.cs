namespace СentralLibrary.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using СentralLibrary_Db.Models;

    /// <summary>
    /// Class a AccountController.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="signInManager">Sign in manager.</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        /// <summary>
        /// Sign in manager.
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return this._signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set => this._signInManager = value;
        }

        /// <summary>
        /// User Manager.
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return this._userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this._userManager = value;
            }
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="returnUrl">Return Url.</param>
        /// <returns>View a Login.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="model">Model a LoginViewModel.</param>
        /// <param name="returnUrl">Return Url.</param>
        /// <returns>sdfv.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result = await this.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false).ConfigureAwait(false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
            }
        }

        /// <summary>
        /// VerifyCode.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="returnUrl">Return Url.</param>
        /// <param name="rememberMe">Remember Me.</param>
        /// <returns>View a VerifyCode.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await this.SignInManager.HasBeenVerifiedAsync().ConfigureAwait(false))
            {
                return this.View("Error");
            }

            return this.View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// Verify code.
        /// </summary>
        /// <param name="model">model a VerifyCodeViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            SignInStatus result = await this.SignInManager.TwoFactorSignInAsync(provider: model.Provider, code: model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser).ConfigureAwait(false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid code.");
                    return this.View(model);
            }
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <returns>View a Register.</returns>
        [AllowAnonymous]
        public ActionResult Register() => this.View();

        /// <summary>
        /// casdcdsc.
        /// </summary>
        /// <param name="model">dcasdsad.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { FirstName = model.FirstName, UserName = model.Email, Surname = model.Surname, Email = model.Email, Gender = model.Gender, PhoneNumber = model.PhoneNumber, DateOfBirth = model.DateOfBirth, Country = model.Country, StreetAddress = model.StreetAddress, Sity = model.Sity, StatusBlockedUser = false };
                var result = await this.UserManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    this.UserManager.AddToRole(user.Id, "user");
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                    return this.RedirectToAction("Index", "Home");
                }

                this.AddErrors(result);
            }

            return this.View(model);
        }

        /// <summary>
        /// Add a new moderator.
        /// </summary>
        /// <returns>View a AddModerator.</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddModerator()
        {
            return this.View();
        }

        /// <summary>
        /// Delete a moderator.
        /// </summary>
        /// <returns>View a DelModerator.</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DelModerator()
        {

            return this.View();
        }

        /// <summary>
        /// Post method a AddModerator.
        /// </summary>
        /// <param name="model">Model a AddModeratorViewModel.</param>
        /// <returns>View a AddModerator.</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult AddModerator(AddModeratorViewModel model)
        {

            var moderator = new ApplicationUser { UserName = model.Email, Email = model.Email, DateOfBirth = DateTime.UtcNow };
            var result = this.UserManager.Create(moderator, model.Password);

            if (result.Succeeded)
            {
                this.UserManager.AddToRole(moderator.Id, "moderator");

                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        /// <summary>
        /// Post method a DelModerator.
        /// </summary>
        /// <param name="email">Email of user.</param>
        /// <returns>View a DelModerator.</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult DelModerator(string email)
        {
            ApplicationUser user = this.UserManager.FindByEmail(email);
            if (user != null)
            {
                if (this.UserManager.GetRoles(user.Id).FirstOrDefault() == "moderator")
                {
                    IdentityResult result = this.UserManager.Delete(user);
                }
            }

            return this.View();
        }

        /// <summary>
        /// Reset Password.
        /// </summary>
        /// <param name="code">code.</param>
        /// <returns>View a ResetPassword.</returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? this.View("Error") : this.View();
        }

        /// <summary>
        /// Reset Password.
        /// </summary>
        /// <param name="model">Model a ResetPasswordViewModel.</param>
        /// <returns>View a ResetPassword.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByNameAsync(userName: model.Email).ConfigureAwait(false);
            if (user == null)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await this.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);

            return this.View();
        }

        /// <summary>
        /// Reset Password Confirmation.
        /// </summary>
        /// <returns>View a ResetPasswordConfirmation.</returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// Log Off.
        /// </summary>
        /// <returns>View a LogOff.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">parametr Dispose.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._userManager != null)
                {
                    this._userManager.Dispose();
                    this._userManager = null;
                }

                if (this._signInManager != null)
                {
                    this._signInManager.Dispose();
                    this._signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}