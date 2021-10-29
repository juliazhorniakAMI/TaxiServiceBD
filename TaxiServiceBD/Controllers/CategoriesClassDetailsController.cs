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

   
        public async Task<IActionResult> Index()
        {
            var list = await _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToListAsync();
            foreach (var x in list) {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.Category = cat;
                x.TaxiClass = t;
            }

            return View(list);
        }

  
        public IActionResult Create()
        {
            ViewData["CategoryFullName"] = new SelectList(_context.Categories, "FullName", "FullName");
            ViewData["TaxiClassFullName"] = new SelectList(_context.TaxiClasses, "FullName", "FullName");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryName,TaxiName")] CategoriesClassDetail categoriesClassDetail)
        {
            using var transaction = _context.Database.BeginTransaction();
          
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryByName {0}", categoriesClassDetail.CategoryName).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("spGetTaxiByName {0}", categoriesClassDetail.TaxiName).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                categoriesClassDetail.Category = cat;
                categoriesClassDetail.TaxiClass = t;

                if (ModelState.IsValid)
                {
                try
                {

                    _context.Add(categoriesClassDetail);
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction completed");
                  
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    throw;
                }

                finally
                {
                    transaction.Dispose();
                }
                return RedirectToAction(nameof(Index));
            }
            
            return RedirectToAction(nameof(Index));
        }


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

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName,TaxiName,IsActive")] CategoriesClassDetail categoriesClassDetail)
        {
          

            using var transaction = _context.Database.BeginTransaction();

            var categ = await _context.Categories.FromSqlRaw<Category>("_spGetCategoryByName {0}", categoriesClassDetail.CategoryName).ToListAsync();
                Category cat = categ.FirstOrDefault();
                var taxi =await _context.TaxiClasses.FromSqlRaw<TaxiClass>("spGetTaxiByName {0}", categoriesClassDetail.TaxiName).ToListAsync();
                TaxiClass t = taxi.FirstOrDefault();
              categoriesClassDetail.Category = cat;
         
            categoriesClassDetail.TaxiClass = t;

            if (ModelState.IsValid)
                {
                try
                {

                 
                    _context.Update(categoriesClassDetail);
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction succeeded");

                }
                catch (DbUpdateConcurrencyException)
                {
                   
                    transaction.Rollback();
                    Console.WriteLine("Transaction failed");

                    if (!CategoriesClassDetailExists(categoriesClassDetail.Id))
                    {
                        return NotFound();
                    }

                    else
                    {

                        throw;
                    }
                }
                finally
                {
                    transaction.Dispose();
                }
                return RedirectToAction(nameof(Index));
            }


            return View(categoriesClassDetail);
        }


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


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var categoriesClassDetail = await _context.CategoriesClassDetails.FindAsync(id);
          
                _context.CategoriesClassDetails.Remove(categoriesClassDetail);
                await transaction.CommitAsync();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw;
            }

            finally
            {
                transaction.Dispose();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriesClassDetailExists(int id)
        {
            return _context.CategoriesClassDetails.Any(e => e.Id == id);
        }
    }
}
