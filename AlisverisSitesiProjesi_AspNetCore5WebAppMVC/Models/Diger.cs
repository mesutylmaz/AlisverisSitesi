using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models
{
    public static class Diger       //Rol-Sipariş-Session İşlemleri burada olacak
    {
        public const string Role_Birey = "Birey";       //Facebook ve Google ile giriş yapanların rolü
        public const string Role_Admin = "Admin";       
        public const string Role_User = "User";
        public const string SessionShoppingCart = "Shopping Cart Session";
    }
}
