using Microsoft.AspNetCore.Mvc;
using Products_API.Model;

namespace Products_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getCategories")]
        public async Task<ActionResult> getCategories()
        {
            var Categories = _context.Categories.ToList();
            return Ok(Categories);
        }

        [HttpPost]
        [Route("addCategory")]
        public async Task<ActionResult> addcategory(Category C)
        {
            _context.Categories.Add(C);
            _context.SaveChanges();
            return Ok("Category Added");
        }


    }
}
