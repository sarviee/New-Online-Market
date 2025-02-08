using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Data;
using OnlineMarket.Models;

namespace OnlineMarket.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? category, string? search)
        {
            var products = await _context.Products
                .Where(p =>
                        (!category.HasValue || p.CategoryId == category) &&
                        (
                            string.IsNullOrEmpty(search) || (p.Name != null &&
                            p.Name.ToLower().Contains(search.ToLower()))
                        )
                )
                .ToListAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserIsAdmin = User.IsInRole("Admin");
            var response = new
            {
                userId = currentUserId,
                data = products,
                isAdmin = currentUserIsAdmin,
            };
            return Ok(response);
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImagePath = model.ImagePath, // You need a file upload API for images
                SellerId = userId,
                CategoryId = model.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
                return Forbid();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.CategoryId = model.CategoryId;
            product.ImagePath = model.ImagePath; // Handle image separately

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
                return Forbid();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
