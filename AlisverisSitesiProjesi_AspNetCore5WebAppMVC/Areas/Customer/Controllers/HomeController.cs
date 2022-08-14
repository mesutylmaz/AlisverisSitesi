using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Data;
using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var products = _applicationDbContext.Products.Where(i => i.IsHome && i.IsStock).ToList();//Anasayfaya IsHome'u ve IsStock'u true olanları getirecek.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _applicationDbContext.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Diger.SessionShoppingCart, count);
            }
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _applicationDbContext.Products.FirstOrDefault(i=>i.ProductID==id);
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.ProductID
            };
            return View(cart);
        }

        [HttpPost][ValidateAntiForgeryToken][Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.CartID = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;
                ShoppingCart cart = _applicationDbContext.ShoppingCarts.FirstOrDefault(
                    u => u.ApplicationUserId == shoppingCart.ApplicationUserId && u.ProductId == shoppingCart.ProductId);
                if(cart==null)
                {
                    _applicationDbContext.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    cart.Count += shoppingCart.Count;
                }
                _applicationDbContext.SaveChanges();
                var count = _applicationDbContext.ShoppingCarts.Where(i => i.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count(); //Bu 2 satır kullanıcıları session'da tutmak için
                HttpContext.Session.SetInt32(Diger.SessionShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _applicationDbContext.Products.FirstOrDefault(i => i.ProductID == shoppingCart.CartID);
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.ProductID
                };
            }
            
            return View(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
