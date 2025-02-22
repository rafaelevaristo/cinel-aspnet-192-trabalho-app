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
    using wapp.Models;
    using waap.Data;
    using Microsoft.AspNetCore.Authorization;
    using static wapp.waapConstants.POLICIES;
    using static wapp.waapConstants.USERS;
    using static wapp.waapConstants;
    using Microsoft.AspNetCore.Mvc.Localization;
    using NToastNotify;
    using waap.Services;
    using waap;
    using Microsoft.AspNetCore.Identity.UI.Services;

    [Authorize]
    [Authorize(Policy = APP_POLICY_SALESAREAS.NAME)]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IToastNotification _toastNotification;
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;

       

        public SalesController(ApplicationDbContext context,                               
                                    ILogger<CategoriesController> logger,
                                    IToastNotification toastNotification,
                                   IHtmlLocalizer<Resource> localizer)
        {
            

            _context = context;            
            _logger = logger;
            _toastNotification = toastNotification;
            _sharedLocalizer = localizer;
        }

        // GET: Sales
        public async Task<IActionResult> Index(SaleState saleState, Boolean onlyNonPayed, int clientId)
        {


            bool isAdmin = User.IsInRole(ROLES.ADMIN);
            bool isSales = User.IsInRole(ROLES.SALESMAN);
            bool isLogistics = User.IsInRole(ROLES.LOGISTICS);


            if (isSales && saleState != SaleState.None && saleState != SaleState.Ordered)
            {
                return View(new List<Sale>());
            }


            if (isLogistics && saleState != SaleState.None && (saleState != SaleState.Processing && saleState != SaleState.Processed && saleState != SaleState.Ordered))
            {
                return View(new List<Sale>());
            }


            IQueryable<Sale> sales = _context.Sales.Include(s => s.Client).Include(s => s.SaleProducts).ThenInclude(sp => sp.Product);


            if (saleState != SaleState.None)
            {
                sales = sales.Where(s => s.State == saleState);
            }

            if (isSales && saleState == SaleState.None)
            {
                sales = sales.Where(s => s.State == SaleState.Ordered);
            }

            if (isLogistics && saleState == SaleState.None )
            {
                sales = sales.Where(s => s.State == SaleState.Processing || s.State == SaleState.Processed || s.State == SaleState.Ordered);                
            }


            if (onlyNonPayed) {
                sales = sales.Where(s => s.IsPaid == false); 
            }

            if (clientId > 0)
            {
                sales = sales.Where(s => s.ClientId == clientId);
            }

            var salesViewData = await sales.ToListAsync();

            if (salesViewData.Count == 0)
            {
                this.AddMessage("NoRecordsToDisplay");
            }


            return View(salesViewData);
        }

        private void AddMessage(string msgID)
        {

            var msgFailled = _sharedLocalizer[msgID].Value;
            _toastNotification.AddWarningToastMessage($" # {msgFailled} ");

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
        
        public async Task<IActionResult> Create([Bind("Id,Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
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

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null) return NotFound();

            
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, [Bind("Id,Identifier,Date,Time,ClientId,Observations,FinalValue,State,IsPaid")] Sale sale)
        {
            if (id != sale.Id) return NotFound();

            var saleToUpdate = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            saleToUpdate.Identifier = sale.Identifier;
            saleToUpdate.Date = sale.Date;
            saleToUpdate.Time = sale.Time;
            saleToUpdate.Observations = sale.Observations;
            saleToUpdate.FinalValue = sale.FinalValue;
            saleToUpdate.State = sale.State;
            saleToUpdate.IsPaid = sale.IsPaid;


           
                try
                {
                    _context.Update(saleToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sale updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id)) return NotFound();
                    throw;
                }
           
           

            return RedirectToAction(nameof(Details), new { id = id }); 
        }


        public async Task<IActionResult> DeleteSalesProduct(int? id, int salesProductId)
        {
            var saleProduct = await _context.SaleProducts.FindAsync(salesProductId);
            if (saleProduct != null)
            {
                _context.SaleProducts.Remove(saleProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sale product deleted successfully!";
            }

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            decimal totalSaleValue = 0;

            foreach (SaleProduct updatedSaleLine in sale.SaleProducts)
            {
                totalSaleValue =+ updatedSaleLine.OrderPrice;
            }


            sale.FinalValue = totalSaleValue;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new {id = id});

            
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
