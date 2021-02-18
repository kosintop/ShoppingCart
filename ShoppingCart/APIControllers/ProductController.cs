using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.APIControllers
{
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("list_category")]
        public async Task<IActionResult> ListCategory()
        {
            var result = await _context.Category
                .AsNoTracking()
                .Select(m => new ResponseCategoryDTO
                {
                    CategoryId = m.Id,
                    CategoryName = m.Name,
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("list_product_by_category")]
        public async Task<IActionResult> ListProductByCategory(RequestProductByCategoryDTO request)
        {
            var result = await _context.Product
                .AsNoTracking()
                .Where(m=>m.CategoryId == request.CategoryId)
                .Select(m => new ResponseProductDTO
                {
                    Id = m.Id,
                    CategoryName = m.Category.Name,
                    ProductName = m.Name,
                    ProductImageUrl = m.ImgUrl,
                    Price = m.Price,
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("search_product")]
        public async Task<IActionResult> SearchProduct(RequestProductSearchDTO request)
        {
            var result = await _context.Product
                .AsNoTracking()
                .Where(m=>m.Name.Contains(request.SearchString))
                .Select(m => new ResponseProductDTO
                {
                    Id = m.Id,
                    CategoryName = m.Category.Name,
                    ProductName = m.Name,
                    ProductImageUrl = m.ImgUrl,
                    Price = m.Price,
                }).ToListAsync();

            return Ok(result);
        }
    }
}
