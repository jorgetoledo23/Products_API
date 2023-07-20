using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_API.Model;
using System.Data;

namespace Products_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Free")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getProducts")]
        public async Task<ActionResult> getProducts()
        {
            var Products = _context.Products.Include(p => p.Category).ToList();
            return Ok(Products);
        }


        [HttpGet]
        [Route("getProductsByCat")]
        public async Task<ActionResult> getProductsByCat(int categoryId)
        {
            var Products = _context.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).ToList();
            return Ok(Products);
        }

        [HttpPost]
        [Route("addProduct")]
        public async Task<ActionResult> addProduct(AddProductDTO pDTO)
        {
            var P = new Product()
            {
                Name = pDTO.Name,
                Price = pDTO.Price,
                CategoryId = pDTO.CategoryId,
                ImageUrl = pDTO.ImageUrl,
                Stock = pDTO.Stock
            };
            _context.Products.Add(P);
            _context.SaveChanges();
            return Ok("Producto Agregado");
        }


    }
}
