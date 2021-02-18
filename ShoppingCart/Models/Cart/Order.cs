using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Order Datetime")]
        public DateTime OrderDateTime { get; set; }

        [Display(Name = "User ID")]
        public string ApplicationUserId { get; set; }

        [Display(Name = "User ID")]
        public ApplicationUser ApplicationUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
