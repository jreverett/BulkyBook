using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this._emailSender = emailSender;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }

            public string Street { get; set; }

            public string City { get; set; }

            public string County { get; set; }

            public string Postcode { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Company ID")]
            public int? CompanyId { get; set; }

            public string Role { get; set; }

            public IEnumerable<SelectListItem> CompanyList { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // populate dropdowns
            Input = new InputModel()
            {
                CompanyList = unitOfWork.Company.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                RoleList = roleManager.Roles.Where(x => x.Name != SD.Role_User_Individual).Select(x => x.Name).Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                })
            };

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Street = Input.Street,
                    County = Input.County,
                    Postcode = Input.Postcode,
                    PhoneNumber = Input.PhoneNumber,
                    CompanyId = Input.CompanyId,
                    Role = Input.Role
                };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    // --- TEMP --- Create roles if they dont already exist
                    if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }

                    if (!await roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }

                    if (!await roleManager.RoleExistsAsync(SD.Role_User_Individual))
                    {
                        await roleManager.CreateAsync(new IdentityRole(SD.Role_User_Individual));
                    }

                    if (!await roleManager.RoleExistsAsync(SD.Role_User_Company))
                    {
                        await roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company));
                    }

                    ///////////////////////////////////////////////////////////////

                    if (user.Role == null)
                    {
                        await userManager.AddToRoleAsync(user, SD.Role_User_Individual);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await userManager.AddToRoleAsync(user, SD.Role_User_Company);
                        }
                        else
                        {
                            await userManager.AddToRoleAsync(user, user.Role);
                        }
                    }

                    // --- TODO --- Configure registration email

                    //var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            // Admin is registering a new user
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
