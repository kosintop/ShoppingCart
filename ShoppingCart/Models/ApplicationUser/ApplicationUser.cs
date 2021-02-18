using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Order> Orders { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
