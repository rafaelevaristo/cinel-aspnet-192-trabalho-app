
namespace wapp.Controllers
{
    using wapp.Models;
    using waap.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using NToastNotify;
    using Microsoft.AspNetCore.Mvc.Localization;
    using waap;
    using wapp.Services;
    using Microsoft.VisualBasic;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class CategoriesController : Controller
    {

        private readonly ILogger<CategoriesController> _logger;
        private readonly IToastNotification _toastNotification;
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;


        public CategoriesController(ILogger<CategoriesController> logger,
                                    IToastNotification toastNotification,
                                    IHtmlLocalizer<Resource> localizer,
                                    ApplicationDbContext context)
        {
            _logger = logger;
            _toastNotification = toastNotification;
            _sharedLocalizer = localizer;
            _context = context;
        }


        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                //TempData["Success"] = "Category created successfully!";
                var sucessMsg = _sharedLocalizer["msgCategoryCrtSucess"].Value;
                _toastNotification.AddSuccessToastMessage($"{sucessMsg} # {category.Name}");
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    //TempData["Success"] = "Category updated successfully!";

                    var sucessMsg = _sharedLocalizer["msgCategoryUpdSucess"].Value;

                    _toastNotification.AddSuccessToastMessage($"# {category.Name} # {sucessMsg} ");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                //TempData["Success"] = "Category deleted successfully!";
                var sucessMsg = _sharedLocalizer["msgCategoryDelSucess"].Value;
                _toastNotification.AddSuccessToastMessage($"# {category.Name} # {sucessMsg} ");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }


        [HttpGet]
        // GET: Categories/ProductsByCategory/5
        public async Task<IActionResult> ProductsByCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var productListQuery = _context.Categories
                .Include(p => p.Products)   
                .Where(m => m.Id == id);
                      
            return View(await productListQuery.ToListAsync());
        }

    }
}
