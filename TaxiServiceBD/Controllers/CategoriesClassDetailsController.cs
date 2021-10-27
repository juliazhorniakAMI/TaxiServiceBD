using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaxiServiceBD.Models;

namespace TaxiServiceBD.Controllers
{
    public class CategoriesClassDetailsController : Controller
    {
        private readonly TaxiServiceContext _context;

        public CategoriesClassDetailsController(TaxiServiceContext context)
        {
            _context = context;
        }

        // GET: CategoriesClassDetails
        public async Task<IActionResult> Index()
        {
            var list = await _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToListAsync();
            foreach (var x in list) {
                var categ =  _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.Category = cat;
                x.TaxiClass = t;
            }
         
            return View(list);
        }

        // GET: CategoriesClassDetails/Details/5
     
        // GET: CategoriesClassDetails/Create
        public IActionResult Create()
        {
            ViewData["CategoryFullName"] = new SelectList(_context.Categories, "FullName", "FullName");
            ViewData["TaxiClassFullName"] = new SelectList(_context.TaxiClasses, "FullName", "FullName");
            return View();
        }

        // POST: CategoriesClassDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,TaxiClassId,IsActive")] CategoriesClassDetail categoriesClassDetail)
        {
            var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryByName {0}", categoriesClassDetail.Category.FullName).ToList();
            Category cat = categ.FirstOrDefault();
            var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("spGetTaxiByName {0}", categoriesClassDetail.TaxiClass.FullName).ToList();
            TaxiClass t = taxi.FirstOrDefault();
            categoriesClassDetail.Category = cat;
            categoriesClassDetail.TaxiClass = t;

            if (ModelState.IsValid)
            {
                _context.Add(categoriesClassDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", categoriesClassDetail.CategoryId);
            //ViewData["TaxiClassId"] = new SelectList(_context.TaxiClasses, "Id", "Id", categoriesClassDetail.TaxiClassId);
            return RedirectToAction(nameof(Index));
        }

        // GET: CategoriesClassDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriesClassDetail = await _context.CategoriesClassDetails.FindAsync(id);
            if (categoriesClassDetail == null)
            {
                return NotFound();
            }
            var categ = await _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", categoriesClassDetail.CategoryId).ToListAsync();
            Category cat = categ.FirstOrDefault();
            var taxi = await _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", categoriesClassDetail.TaxiClassId).ToListAsync();
            TaxiClass t = taxi.FirstOrDefault();
            ViewData["CategoryFullName"] = new SelectList(_context.Categories, "FullName", "FullName", cat.FullName);
            ViewData["TaxiClassFullName"] = new SelectList(_context.TaxiClasses, "FullName", "FullName", t.FullName);
            return View(categoriesClassDetail);
        }

        // POST: CategoriesClassDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,TaxiClassId,IsActive")] CategoriesClassDetail categoriesClassDetail)
        {
            var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryByName {0}", categoriesClassDetail.Category.FullName).ToList();
            Category cat = categ.FirstOrDefault();
            var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("spGetTaxiByName {0}", categoriesClassDetail.TaxiClass.FullName).ToList();
            TaxiClass t = taxi.FirstOrDefault();
            categoriesClassDetail.Category = cat;
            categoriesClassDetail.TaxiClass = t;
            if (id != categoriesClassDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriesClassDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesClassDetailExists(categoriesClassDetail.Id))
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
         
            return View(categoriesClassDetail);
        }

        // GET: CategoriesClassDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriesClassDetail = await _context.CategoriesClassDetails
                .Include(c => c.Category)
                .Include(c => c.TaxiClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriesClassDetail == null)
            {
                return NotFound();
            }

            return View(categoriesClassDetail);
        }

        // POST: CategoriesClassDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriesClassDetail = await _context.CategoriesClassDetails.FindAsync(id);
            _context.CategoriesClassDetails.Remove(categoriesClassDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriesClassDetailExists(int id)
        {
            return _context.CategoriesClassDetails.Any(e => e.Id == id);
        }
    }
}
