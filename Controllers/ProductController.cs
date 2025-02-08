using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Data;
using OnlineMarket.Models;

namespace OnlineMarket.Controllers;


public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProductController(
        ApplicationDbContext context,
        IWebHostEnvironment environment
        )
    {
        _context = context;
        _environment = environment;

    }

    public IActionResult Index()
    {
        return View();
    }


    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [Authorize(Roles = "Seller,Admin")]
    public IActionResult Create()
    {
        var model = new ProductViewModel
        {
            Categories = _context.Categories.ToList()
        };
        return View(model);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        model.Categories = await _context.Categories.ToListAsync(); // Reload categories
        if (ModelState.IsValid)
        {
            Console.WriteLine("validation ok");

            string? uniqueFileName = UploadedFile(model);


            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImagePath = uniqueFileName,
                SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CategoryId = model.CategoryId // Save selected category
            };

            Console.WriteLine(product);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{product.Name} yaratildi";
            return RedirectToAction("Index");
        }

        else
        {
            var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .SelectMany(ms => ms.Value.Errors)
                .ToList();

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }
        }

        return View(model);
    }

    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (product.SellerId != userId)
            return Forbid();

        var model = new ProductViewModel
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImagePath = product.ImagePath,
            CategoryId = product.CategoryId,
            Categories = _context.Categories.ToList()
        };
        return View(model);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit([FromRoute] int id, ProductViewModel model)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (product.SellerId != userId)
            return Forbid();

        model.Categories = await _context.Categories.ToListAsync(); // Reload categories
        model.ImagePath = product.ImagePath;

        if (ModelState.IsValid)
        {
            string? uniqueFileName = null;
            if (model.ImageFile != null)
                uniqueFileName = UploadedFile(model);


            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            if (uniqueFileName != null)
                product.ImagePath = uniqueFileName;
            product.CategoryId = model.CategoryId;



            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"{product.Name} Tahrirlandi";
            return RedirectToAction("Index");
        }

        else
        {
            var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .SelectMany(ms => ms.Value.Errors)
                .ToList();

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }
        }

        return View(model);
    }


    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (product.SellerId != userId)
            return Forbid();

        var model = new ProductViewModel
        {
            Name = product.Name,
            Categories = _context.Categories.ToList()
        };

        return View(model);

    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] int id, ProductViewModel model)
    {

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (product.SellerId != userId)
            return Forbid();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        TempData["Message"] = $"{product.Name} o'chirildi";
        return RedirectToAction("Index");
    }

    private string? UploadedFile(ProductViewModel model)
    {
        string? uniqueFileName = null;

        Console.WriteLine(model.ImageFile);
        Console.WriteLine("yes");
        if (model.ImageFile != null)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.ImageFile.CopyTo(fileStream);
            }
        }
        return uniqueFileName;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
