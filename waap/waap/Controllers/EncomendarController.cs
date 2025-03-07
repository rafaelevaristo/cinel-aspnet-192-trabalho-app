﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using waap.Data;
using wapp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using waap.Services;
using wapp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using NToastNotify;
using wapp.Controllers;

namespace waap.Controllers
{

    [Authorize]
    public class EncomendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailservice;

        private readonly ViewRenderService _viewRenderService;

        private readonly ILogger<CategoriesController> _logger;
        private readonly IToastNotification _toastNotification;        
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;

        public EncomendarController(ApplicationDbContext context, 
                                    IEmailSender emailservice, 
                                    ViewRenderService viewRenderService, 
                                    ILogger<CategoriesController> logger,
                                    IToastNotification toastNotification,
                                   IHtmlLocalizer<Resource> localizer)
        {
            _context = context;
            _emailservice = emailservice;
            _viewRenderService = viewRenderService;

            _logger = logger;
            _toastNotification = toastNotification;
            _sharedLocalizer = localizer;
        }

        // GET: SalesManagement
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clients;
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: SalesManagement/Details/5
        public async Task<IActionResult> SelectProductsForSale(int? id)
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


            public async Task<IActionResult> SaleDetails(int? id, int[]? productIds)
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


            var newSaleProducts = new List<SaleProduct>();


            foreach (var selectedProduct in selectedProducts)
            {
                // TODO Protect from empty stuff 


                try
                {
                    int quantityStr = int.Parse(HttpContext.Request.Query[$"quantity_for_product_{selectedProduct.Id}_stuff"]);

                    var newSaleProduct = new SaleProduct
                    {
                        Quantity = quantityStr,
                        Product = selectedProduct,
                        OrderPrice = quantityStr * selectedProduct.FinalPrice
                    };

                    newSaleProducts.Add(newSaleProduct);
                }
                catch (Exception)
                {
                    // TODO return to the main view and pop a error message via toastr .
                    return BadRequest($"Bad quantity select for sale for product {selectedProduct.Description}");
                }



            }

            var newSale = new Sale();

            newSale.ClientId = (int)id;

            newSale.SaleProducts = newSaleProducts;

            newSale.FinalValue = 1;
            newSale.Observations = "testes e tal";


            if (selectedProducts == null)
            {
                return NotFound();
            }

            newSale.Identifier = Guid.NewGuid().ToString();

            return View(newSale);
        }



        public async Task<IActionResult> ConcludeSale(Sale sale)
        {
            var newSaleProducts = new List<SaleProduct>();

            foreach (var saleproduct in sale.SaleProducts)
            {
                // TODO Protect from empty stuff 

                try
                {

                    var newSaleProduct = new SaleProduct
                    {
                        Quantity = saleproduct.Quantity,
                        ProductId = saleproduct.Product.Id,
                        OrderPrice = saleproduct.OrderPrice

                        // TODO : Não gosto de fazer isto isto deveria ser revalidado mas sinceramente depois vê-se
                        //OrderPrice = saleproduct.Quantity * saleproduct.Product.FinalPrice



                    };


                    newSaleProducts.Add(newSaleProduct);
                }
                catch (Exception)
                {
                    // TODO return to the main view and pop a error message via toastr .
                    return BadRequest($"Bad quantity select for sale for product {saleproduct.Product.Description}");
                }



            }

            var newSale = new Sale();

            newSale.ClientId = sale.ClientId;

            newSale.SaleProducts = newSaleProducts;

            var totalSale = newSale.SaleProducts?.Sum(sp => sp.OrderPrice) ?? 0m;

            newSale.FinalValue = totalSale;
            newSale.Observations = sale.Observations;
            newSale.Identifier = sale.Identifier;
            newSale.State = SaleState.Ordered;

            _context.Sales.Add(newSale);
            await _context.SaveChangesAsync();

            var saleData = await _context.Sales
           .Include(s => s.Client)
           .Include(s => s.SaleProducts)
           .ThenInclude(sp => sp.Product)
           .FirstOrDefaultAsync(m => m.Id == newSale.Id);

            if (sale != null) {
                try
                {
                    string htmlEmail = await _viewRenderService.RenderViewToStringAsync(ControllerContext, "_EmailTemplate", saleData);
                    await _emailservice.SendEmailAsync(saleData.Client.Email, $"Encomenda {newSale.Identifier}", htmlEmail);
                }
                catch (Exception)
                {
                    var msgFalledToSendEmail = _sharedLocalizer["msgFalledToSendEmail"].Value;

                    _toastNotification.AddErrorToastMessage($" # {msgFalledToSendEmail} : {saleData.Identifier}");

                }

           

            }
            // Redirect or do something with newSaleId
            return RedirectToAction("Details", "Sales", new { id = newSale.Id });            
        }

               
    }
}
