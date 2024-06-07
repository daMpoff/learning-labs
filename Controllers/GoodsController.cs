using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMarketplaceProject.Models;

namespace MyMarketplaceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : Controller
    {
        private readonly MarketplaceContext context;

        public GoodsController(MarketplaceContext context)
        {
            this.context = context;
        }
        [HttpGet]
        // GET: Goods
        public async Task<IActionResult> Goods()
        {
            var goods = await context.Goods.Include(g => g.GoodParameters).ThenInclude(gp => gp.Parameter).ToListAsync();
            //создали объект DTO
            var result = goods.Select(g => new
            {
                g.GoodId,
                g.SubcategoryId,
                g.ShopId,
                Parameters = g.GoodParameters.Select(gp => new
                {
                    gp.ParameterId,
                    ParameterName = gp.Parameter.Name,
                    gp.Value
                })
            });
            return Ok(result);
        }

        private bool GoodExists(int id)
        {
            return context.Goods.Any(e => e.GoodId == id);
        }
    }
}
