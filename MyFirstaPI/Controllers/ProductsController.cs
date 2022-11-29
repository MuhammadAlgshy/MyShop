using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstaPI.Models;
using System.Linq;

namespace MyFirstaPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return _context.Products.ToArray();
        //}
        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery]ProductQueryParameters query)
        {
            IQueryable<Product> products = _context.Products;

            if (query.MinPrice != null)
            {
                products.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice != null)
            {
                products.Where(p => p.Price <= query.MaxPrice.Value);
            }
            products = products
                .Skip(query.Size * (query.Page - 1))
                .Take(query.Size);

            return Ok(await products.ToArrayAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                "GetProduct", new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var editedproduct = await _context.Products.FindAsync(id);
            return Ok(editedproduct);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
            {
            var product = await _context.Products.FindAsync(id);
            if (product == null) { return NotFound(); }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<Product>> DeleteMultiple([FromQuery]int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            { 
                var product = await _context.Products.FindAsync(id);
                if (product == null) { return NotFound(); }
                products.Add(product);

            }
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
            return Ok(products);
        }
    }
}
