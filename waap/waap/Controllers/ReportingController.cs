namespace waap.Controllers
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
    using waap.ViewModels.Reporting;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class ReportingController : Controller
    {


        private readonly ApplicationDbContext _context;

        public ReportingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReportingController
        public async Task<IActionResult> Saldo()
        {

            var saldoModel = new SaldoViewModel();

            var salesQuery = _context.Sales.Include(s => s.Client).Include(s => s.SaleProducts).ThenInclude(sp => sp.Product);
            
            var sales = await salesQuery.ToListAsync();

            saldoModel.TotalSalesValue = sales.Sum(s => s.FinalValue);
            saldoModel.TotalPaidValue= sales.Where(s => s.IsPaid).Sum(s => s.FinalValue);
            saldoModel.TotalNonPaidValue = sales.Where(s => !s.IsPaid).Sum(s => s.FinalValue);

            return View(saldoModel);
        }

        // GET: ReportingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReportingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReportingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
