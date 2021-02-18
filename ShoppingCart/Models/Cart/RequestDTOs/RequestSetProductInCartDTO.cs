using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Models
{
    public class RequestSetProducInCartDTO
    {
        public int ProductId { get; set; }

        [Range(0, 99)]
        public int Quantity { get; set; }
    }
}
