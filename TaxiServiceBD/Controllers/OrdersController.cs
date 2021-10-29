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
    public class OrdersController : Controller
    {
        private readonly TaxiServiceContext _context;

        public OrdersController(TaxiServiceContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string SearchString)
        {
           
            var taxiServiceContext = await _context.Orders.Include(o => o.CategoryClass).Include(o => o.Driver).Include(o => o.User).ToListAsync();
            var list = await _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToListAsync();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.CategoryName = cat.FullName;
                x.TaxiName = t.FullName;
            }
            for (int i=0;i< taxiServiceContext.Count;i++)
            {

                for (int j = 0; j < list.Count; j++) {

                    if (taxiServiceContext[i].CategoryClassId == list[j].Id) {

                        taxiServiceContext[i].CategoryClass = list[j];
                    }
                }
             
            }
            ViewData["CurrentFilter"] = SearchString;
          
            if (!String.IsNullOrEmpty(SearchString))
            {
                taxiServiceContext =  taxiServiceContext.Where(s => s.DateOfCreation.ToString().StartsWith(SearchString)).ToList();
            }
            return View(taxiServiceContext);
        }
        public async Task<IActionResult> SortKey(string sortOrder)
        {

            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["DriverSortParm"] = sortOrder == "DriverAsc" ? "driver_desc" : "DriverAsc";

            var taxiServiceContext =  await _context.Orders.Include(o => o.CategoryClass).Include(o => o.Driver).Include(o => o.User).ToListAsync();
            var list = await _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToListAsync();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.CategoryName = cat.FullName;
                x.TaxiName = t.FullName;
            }
            for (int i = 0; i < taxiServiceContext.Count; i++)
            {

                for (int j = 0; j < list.Count; j++)
                {

                    if (taxiServiceContext[i].CategoryClassId == list[j].Id)
                    {

                        taxiServiceContext[i].CategoryClass = list[j];
                    }
                }

            }
            switch (sortOrder)
            {
                case "date_desc":
                    taxiServiceContext = taxiServiceContext.OrderByDescending(s => s.DateOfCreation).ToList();
                    break;
                case "DriverAsc":
                    taxiServiceContext = taxiServiceContext.OrderBy(s => s.CategoryClassId).ToList();
                    break;
                case "driver_desc":
                    taxiServiceContext = taxiServiceContext.OrderByDescending(s => s.CategoryClassId).ToList();
                    break;
                default:
                    taxiServiceContext = taxiServiceContext.OrderBy(s => s.DateOfCreation).ToList();
                    break;
            }
            return View("Index", taxiServiceContext);


        }
        //TASK8
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.CategoryClass)
                .Include(o => o.Driver)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var list =  _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToList();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.CategoryName = cat.FullName;
                x.TaxiName = t.FullName;
            }
            var stands =
               list
             .Select(s => new
           {
           Id = s.Id,
           Description = string.Format("{0}-- £{1}", s.CategoryName, s.TaxiName)
         })
          .ToList();

            ViewData["CategoryClassId"] = new SelectList(stands, "Id", "Description");
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DriverId,CategoryClassId,DateOfCreation")] Order order)
        {
            using var transaction = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {
                try {
                    _context.Add(order);
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction succeeded");
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
            var list = _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToList();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.Category = cat;
                x.TaxiClass = t;
            }
            var stands =
               list
          .Select(s => new
          {
              Id = s.Id,
              Description = string.Format("{0}-- £{1}", s.CategoryName, s.TaxiName)
          })
       .ToList();
            ViewData["CategoryClassId"] = new SelectList(stands, "Id", "Description", order.CategoryClassId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", order.DriverId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var list = _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToList();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.CategoryName = cat.FullName;
                x.TaxiName = t.FullName;
            }
            var stands =
               list
           .Select(s => new
           {
               Id = s.Id,
               Description = string.Format("{0}-- £{1}", s.CategoryName, s.TaxiName)
           })
        .ToList();
            ViewData["CategoryClassId"] = new SelectList(stands, "Id", "Description", order.CategoryClassId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", order.DriverId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DriverId,CategoryClassId,DateOfCreation")] Order order)
        {
            using var transaction = _context.Database.BeginTransaction();
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction succeeded");
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
            var list = _context.CategoriesClassDetails.FromSqlRaw<CategoriesClassDetail>("spGetCategoriesTaxi").ToList();
            foreach (var x in list)
            {
                var categ = _context.Categories.FromSqlRaw<Category>("_spGetCategoryById {0}", x.CategoryId).ToList();
                Category cat = categ.FirstOrDefault();
                var taxi = _context.TaxiClasses.FromSqlRaw<TaxiClass>("_spGetTaxiById {0}", x.TaxiClassId).ToList();
                TaxiClass t = taxi.FirstOrDefault();
                x.CategoryName = cat.FullName;
                x.TaxiName = t.FullName;
            }
            var stands =
               list
           .Select(s => new
           {
               Id = s.Id,
               Description = string.Format("{0}-- £{1}", s.CategoryName, s.TaxiName)
           })
        .ToList();
            ViewData["CategoryClassId"] = new SelectList(stands, "Id", "Description", order.CategoryClassId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", order.DriverId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.CategoryClass)
                .Include(o => o.Driver)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                var order = await _context.Orders.FindAsync(id);
                _context.Orders.Remove(order);
               await _context.SaveChangesAsync();
               transaction.Commit();
                Console.WriteLine("Transaction succeeded");
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

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
