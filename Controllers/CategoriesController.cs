using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMarketplaceProject.Models;

namespace MyMarketplaceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly MarketplaceContext context;

        public CategoriesController(MarketplaceContext context)
        {
            this.context = context;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await context.Categories.Include(c => c.Subcategories).ToListAsync();
            //создали объект DTO
            var result = categories.Select(c => new
            {
                c.CategoryId,
                c.Name,
                Subcategories = c.Subcategories.Select(s => new
                {
                    s.SubcategoryId,
                    s.Name
                })
            });
            return Ok(result);
        }
    }
}
