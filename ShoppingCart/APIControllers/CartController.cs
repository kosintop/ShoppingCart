using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;

namespace ShoppingCart.APIControllers
{
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add_cart")]
        public async Task<IActionResult> AddProductToCart(RequestAddProductToCartDTO request)
        {
            var userId = User.FindFirst("Id").Value;
            if (userId == null) return Unauthorized();

            //check for existing item in cart
            var cartItem = _context.ShoppingCartItem.FirstOrDefault(x=>x.ApplicationUserId==userId && x.ProductId == request.ProductId);

            //create new record if null
            if (cartItem == null)
            {
                var newCartItem = new ShoppingCartItem();
                newCartItem.ApplicationUserId = userId;
                newCartItem.ProductId = request.ProductId;
                newCartItem.Quantity = request.Quantity;
                _context.Add(newCartItem);
            }
            else //update quantity if exist
            {
                cartItem.Quantity += request.Quantity;
            }

            //save
            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpPost]
        [Route("remove_cart")]
        public async Task<IActionResult> RemoveProductFromCart(RequestRemoveProductFromCartDTO request)
        {
            var userId = User.FindFirst("Id").Value;
            if (userId == null) return Unauthorized();

            //check for existing item in cart
            var cartItem = _context.ShoppingCartItem.FirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId);

            if (cartItem == null) return NotFound();

            //update quantity
            cartItem.Quantity -= request.Quantity;

            //if less than 1, remove record
            if (cartItem.Quantity < 1)
            {
                _context.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpPost]
        [Route("set_cart")]
        public async Task<IActionResult> SetProductInCart(RequestSetProducInCartDTO request)
        {
            var userId = User.FindFirst("Id").Value;
            if (userId == null) return Unauthorized();

            //check for existing item in cart
            var cartItem = _context.ShoppingCartItem.FirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId);

            //create new record if not exist
            if (cartItem == null)
            {
                var newCartItem = new ShoppingCartItem();
                newCartItem.ApplicationUserId = userId;
                newCartItem.ProductId = request.ProductId;
                newCartItem.Quantity = request.Quantity;
                _context.Add(newCartItem);
            }
            else //update quantity if already exist
            {
                cartItem.Quantity = request.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpPost]
        [Route("list_cart")]
        public async Task<IActionResult> ListCart()
        {
            var userId = User.FindFirst("Id").Value;

            if (userId == null) return Unauthorized();

            //query all item in cart
            var cartItems = await _context.ShoppingCartItem
                .AsNoTracking()
                .Where(x => x.ApplicationUserId == userId)
                .Select(m => new ResponseProductCartDTO
                {
                    Id = m.ProductId,
                    CategoryName = m.Product.Category.Name,
                    ProductName = m.Product.Name,
                    ProductImageUrl = m.Product.ImgUrl,
                    Price = m.Product.Price,
                    Quantity = m.Quantity,
                    Amount = m.Product.Price * m.Quantity,
                }).ToListAsync();

            //sum amount
            var sumCart = new ResponseCartViewDTO
            {
                TotalAmount = cartItems.Sum(m => m.Amount),
                ProductList = cartItems
            };

            return Ok(sumCart);

        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var userId = User.FindFirst("Id").Value;

            if (userId == null) return Unauthorized();

            //create new order
            var newOrder = new Order();
            newOrder.ApplicationUserId = userId;
            newOrder.OrderDateTime = DateTime.Now;

            //query items in cart
            var cartItems = await _context.ShoppingCartItem
                .Where(x => x.ApplicationUserId == userId)
                .Include(x=>x.Product)
                .ToListAsync();

            //map item in cart to new order item
            var addList = new List<OrderItem>();
            foreach(var item in cartItems)
            {
                var newOrderItem = new OrderItem();
                newOrderItem.Order = newOrder;
                newOrderItem.ProductId = item.ProductId;
                newOrderItem.Quantity = item.Quantity;
                newOrderItem.Price = item.Product.Price;
                newOrderItem.Amount = newOrderItem.Quantity * newOrderItem.Price;
                addList.Add(newOrderItem);
            }

            //sum order amount
            newOrder.TotalAmount = addList.Sum(x => x.Amount);

            _context.Add(newOrder);
            _context.AddRange(addList);
            _context.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
