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
    using Microsoft.AspNetCore.Authorization;
    using wapp.Services;
    using waap.Services;
    using Microsoft.AspNetCore.Identity.UI.Services;

    [Authorize]
    public class ProcessarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailservice;
        private readonly ViewRenderService _viewRenderService;


        public ProcessarController(ApplicationDbContext context, IEmailSender emailservice, ViewRenderService viewRenderService)
        {
            _context = context;
            _emailservice = emailservice;
            _viewRenderService = viewRenderService;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {   
            return RedirectToAction("Index", "Sales");
        }



        public async Task<IActionResult> StartProcessing(int? id)
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

        public async Task<IActionResult> ProcessAndVerify(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            foreach (var saleProduct in sale.SaleProducts)
            {
                if (saleProduct.Quantity > saleProduct.Product.StockQuantity)
                {
                    saleProduct.Quantity = 0;
                    saleProduct.OrderPrice = 0;

                    sale.Observations = sale.Observations + $" O produto {saleProduct.Product.Description} tem um stoque inferior ao requerido e foi colocad a 0 na encomenda .";
                }
                
            }

            var totalSale = sale.SaleProducts?.Sum(sp => sp.OrderPrice) ?? 0m;
            
            sale.FinalValue = totalSale;

            sale.State = SaleState.Processing;

            if (sale == null) return NotFound();

            await _context.SaveChangesAsync();

            return RedirectToAction("StartProcessing", new { id = id });
        }



        public async Task<IActionResult> Picking(int? id)
        {


            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            foreach (var saleProduct in sale.SaleProducts)
            {
                if (saleProduct.Quantity !=0 && saleProduct.Quantity <= saleProduct.Product.StockQuantity)
                {
                    saleProduct.Product.StockQuantity = saleProduct.Product.StockQuantity - saleProduct.Quantity;                                      
                }
            }

            sale.State = SaleState.Processed;

            await _context.SaveChangesAsync();

            return RedirectToAction("StartProcessing", new { id = id });
        }


        


    }



}
