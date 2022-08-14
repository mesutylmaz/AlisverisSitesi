using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Data;
using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;        //Rol Yöneticisi için ekledim
        private readonly ApplicationDbContext _db;                      //Database'e Bağlanmak için ekledim

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,      //Ekledim
            ApplicationDbContext db)                    //Ekledim
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;                 //Ekledim
            _db = db;                                   //Ekledim
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="E-mail alanı boş bırakılamaz!")]
            [EmailAddress(ErrorMessage = "Girilen veri E-Mail tipinde değil!")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
            [StringLength(100, ErrorMessage = "{0}, en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "Şifre belirtilen şartları sağlamıyor!")]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
            [DataType(DataType.Password, ErrorMessage ="Şifre belirtilen şartları sağlamıyor!")]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Şifre ve onay şifresi eşleşmiyor.")]
            public string ConfirmPassword { get; set; }


            [Required(ErrorMessage = "İsim alanı boş bırakılamaz!")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Soyisim alanı boş bırakılamaz!")]
            public string Surname { get; set; }

            public string Adres { get; set; }
            public string Sehir { get; set; }
            public string Semt { get; set; }
            public string PostaKodu { get; set; }
            public string TelefonNo { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem>RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Where(x => x.Name != Diger.Role_Birey)
                .Select(y => y.Name)
                .Select(z => new SelectListItem
                {
                    Text = z,
                    Value = z
                })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Adres = Input.Adres,
                    Sehir = Input.Sehir,
                    Semt = Input.Semt,
                    Name = Input.Name,
                    Surname = Input.Surname,
                    PostaKodu = Input.PostaKodu,
                    PhoneNumber = Input.TelefonNo,
                    Role = Input.Role
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı parole ile yeni bir hesap oluşturdu.");

                    if(!await _roleManager.RoleExistsAsync(Diger.Role_Admin))       //Admin Rolü Yoksa
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Admin));     //Admin Rolü Oluştur
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Birey))       //Birey Rolü Yoksa
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Birey));     //Birey Rolü Oluştur
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_User))       //User Rolü Yoksa
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_User));     //User Rolü Oluştur
                    }
                    //await _userManager.AddToRoleAsync(user, Diger.Role_Admin);      //Sisteme ilk giriş yapan Admin olacak ve Kayıt Ol diyip Admin kaydı yapıldıktan sonra bu satır silinecek

                    if (user.Role == null)  
                    {
                        await _userManager.AddToRoleAsync(user, Diger.Role_User);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user,user.Role);      //Kullanıcının rolü varsa rolünü ver
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);      //Email Doğrulama İşlemleridir
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
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
