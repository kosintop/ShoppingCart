using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{

    public class ResponseCartViewDTO
    {
        public double TotalAmount { get; set; }
        public List<ResponseProductCartDTO> ProductList { get; set; }

    }
}
