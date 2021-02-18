using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}
