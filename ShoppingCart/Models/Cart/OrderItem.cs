using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Display(Name = "Product ID")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Amount")]
        public double Amount { get; set; }
    }
}
