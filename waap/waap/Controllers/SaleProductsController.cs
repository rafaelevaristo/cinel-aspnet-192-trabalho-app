using Microsoft.AspNetCore.Mvc;
using wapp.Models;
using waap.Data;
using Microsoft.EntityFrameworkCore;

namespace wapp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    public class SaleProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SaleProducts
        public async Task<IActionResult> Index()
        {
            var saleProducts = _context.SaleProducts.Include(sp => sp.Sale).Include(sp => sp.Product);
            return View(await saleProducts.ToListAsync());
        }

        // GET: SaleProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var saleProduct = await _context.SaleProducts
                .Include(sp => sp.Sale)
                .Include(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (saleProduct == null) return NotFound();

            return View(saleProduct);
        }

        // GET: SaleProducts/Create
        public IActionResult Create()
        {
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Identifier");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View();
        }

        // POST: SaleProducts/Create
        [HttpPost]
        
        public async Task<IActionResult> Create([Bind("SaleId,ProductId,Quantity")] SaleProduct saleProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saleProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sale product added successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Identifier", saleProduct.SaleId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", saleProduct.ProductId);
            return View(saleProduct);
        }

        // GET: SaleProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var saleProduct = await _context.SaleProducts.FindAsync(id);
            if (saleProduct == null) return NotFound();

            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Identifier", saleProduct.SaleId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", saleProduct.ProductId);
            return View(saleProduct);
        }

        // POST: SaleProducts/Edit/5
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, [Bind("Id,SaleId,ProductId,Quantity")] SaleProduct saleProduct)
        {
            if (id != saleProduct.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saleProduct);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sale product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleProductExists(saleProduct.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Identifier", saleProduct.SaleId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", saleProduct.ProductId);
            return View(saleProduct);
        }

        // GET: SaleProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var saleProduct = await _context.SaleProducts
                .Include(sp => sp.Sale)
                .Include(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (saleProduct == null) return NotFound();

            return View(saleProduct);
        }

        // POST: SaleProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saleProduct = await _context.SaleProducts.FindAsync(id);
            if (saleProduct != null)
            {
                _context.SaleProducts.Remove(saleProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sale product deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SaleProductExists(int id)
        {
            return _context.SaleProducts.Any(e => e.Id == id);
        }
    }



}
