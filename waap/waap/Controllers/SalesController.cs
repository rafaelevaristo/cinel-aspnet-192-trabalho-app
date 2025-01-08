using Microsoft.AspNetCore.Mvc;
using wapp.Models;
using waap.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace wapp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Net.Mail;
    using System.Net;

    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = _context.Sales.Include(s => s.Client).Include(s => s.SaleProducts).ThenInclude(sp => sp.Product);
            return View(await sales.ToListAsync());
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale, List<int> selectedProductIds, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                sale.Date = DateTime.Now;
                sale.Time = DateTime.Now;
                sale.Identifier = $"V{DateTime.Now.Year}/A/{_context.Sales.Count() + 1:D3}";

                // Calculate final value based on selected products and quantities
                decimal finalValue = 0;
                for (int i = 0; i < selectedProductIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(selectedProductIds[i]);
                    finalValue += product.FinalPrice * quantities[i];
                    sale.SaleProducts.Add(new SaleProduct
                    {
                        ProductId = selectedProductIds[i],
                        Quantity = quantities[i]
                    });
                }

                sale.FinalValue = finalValue;
                sale.State = SaleState.Ordered;
                sale.IsPaid = false;

                _context.Add(sale);
                await _context.SaveChangesAsync();

                // Send Email to client
                await SendSaleEmail(sale);

                TempData["Success"] = "Sale created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View(sale);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale)
        {
            if (id != sale.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sale updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sale deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to send an email to the client
        private async Task SendSaleEmail(Sale sale)
        {
            var client = await _context.Clients.FindAsync(sale.ClientId);

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("your-email@example.com"); // Sender's email address
            mailMessage.To.Add(client.Email);
            mailMessage.Subject = "Sale Confirmation";
            mailMessage.Body = $"Dear {client.FullName},\n\n" +
                                $"Your sale (ID: {sale.Identifier}) has been successfully created. " +
                                $"The total amount is {sale.FinalValue:C}. " +
                                $"State: {sale.State.ToString()}.\n\n" +
                                $"Thank you for your purchase!";
            mailMessage.IsBodyHtml = false;

            using (var smtpClient = new SmtpClient("smtp.example.com", 587)) // Configure SMTP settings
            {
                smtpClient.Credentials = new NetworkCredential("your-email@example.com", "your-email-password");
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        // GET: Sales/Process/5
        public async Task<IActionResult> Process(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.State = SaleState.Processing;
            _context.Update(sale);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sale is now processing!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Complete/5
        public async Task<IActionResult> Complete(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.State = SaleState.Processed;
            _context.Update(sale);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sale has been processed!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Ship/5
        public async Task<IActionResult> Ship(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.State = SaleState.Sent;
            _context.Update(sale);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sale has been shipped!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Cancel/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.State = SaleState.Canceled;
            _context.Update(sale);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sale has been canceled!";
            return RedirectToAction(nameof(Index));
        }
    }



}
