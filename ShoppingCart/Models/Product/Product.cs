using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public Category Category { get; set; }

        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Product Price")]
        public double Price { get; set; }

        [Display(Name = "Image")]
        public string ImgUrl { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImgFile { get; set; }
    }
}
