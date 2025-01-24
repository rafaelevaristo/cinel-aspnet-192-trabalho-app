using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using waap.Data;
using wapp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace waap.Controllers
{
    public class EncomendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;


        public EncomendarController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: SalesManagement
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clients;
            return View(await applicationDbContext.ToListAsync());
        }


        

        // GET: SalesManagement/Details/5
        public async Task<IActionResult> selectproductsforsale(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => ! p.IsInactive);

            ViewBag.CustomerId = id;

            return View(await products.ToListAsync());

            
        }

        
        public async Task<IActionResult> ConcludeSale(int? id, int[]? productIds)
        {
            // Check if the sale ID is provided
            //if (id == null)
            //{
            //    return NotFound();
            //}

            // Check if any product IDs are provided
            if (productIds == null || productIds.Length == 0)
            {
                return BadRequest("No products selected for the sale.");
            }

            // Retrieve the selected products from the database
            var selectedProducts = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();


            if (selectedProducts == null)
            {
                return NotFound();
            }

            return View(selectedProducts);
        }

        // GET: SalesManagement/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
            return View();
        }

        // POST: SalesManagement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create([Bind("Id,Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address", sale.ClientId);
            return View(sale);
        }

        // GET: SalesManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address", sale.ClientId);
            return View(sale);
        }

        // POST: SalesManagement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, [Bind("Id,Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address", sale.ClientId);
            return View(sale);
        }

        // GET: SalesManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: SalesManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
