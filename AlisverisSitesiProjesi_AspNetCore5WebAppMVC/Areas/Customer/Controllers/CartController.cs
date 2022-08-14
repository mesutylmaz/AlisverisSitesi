using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Data;
using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        
        

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public CartController(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext db)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Doğrulama E-Maili Boş...");
            }


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);      //Email Doğrulama İşlemleridir
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "E-Mail Doğrulandı",
                $"Hesabınızı onaylamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>burayı tıklayın</a>.");
            ModelState.AddModelError(string.Empty, "E-Mail doğrulama kodu gönder...");
            return RedirectToAction("Success");
        }


        public IActionResult Success()
        {
            return View();
        }


        public IActionResult Add(int cartID)        //Artırma işlemini yapıyor
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.CartID == cartID);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Decrease(int cartID)       //Azaltma işlemini yapıyor
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.CartID == cartID);
            if (cart.Count == 1)
            {
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.SessionShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Remove(int cartID)     //Silme işlemini yapıyor
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.CartID == cartID);

            var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.SessionShoppingCart, count - 1);

            return RedirectToAction(nameof(Index));
        }
    }
}
