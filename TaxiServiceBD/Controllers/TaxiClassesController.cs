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
    public class TaxiClassesController : Controller
    {
        private readonly TaxiServiceContext _context;

        public TaxiClassesController(TaxiServiceContext context)
        {
            _context = context;
        }

        // GET: TaxiClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaxiClasses.ToListAsync());
        }

        // GET: TaxiClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxiClass = await _context.TaxiClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taxiClass == null)
            {
                return NotFound();
            }

            return View(taxiClass);
        }

        // GET: TaxiClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaxiClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName")] TaxiClass taxiClass)
        {
            using var transaction = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {
                try {

                    _context.Add(taxiClass);
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
            return View(taxiClass);
        }

        // GET: TaxiClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxiClass = await _context.TaxiClasses.FindAsync(id);
            if (taxiClass == null)
            {
                return NotFound();
            }
            return View(taxiClass);
        }

        // POST: TaxiClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName")] TaxiClass taxiClass)
        {
            using var transaction = _context.Database.BeginTransaction();
            if (id != taxiClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taxiClass);
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
            return View(taxiClass);
        }

        // GET: TaxiClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxiClass = await _context.TaxiClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taxiClass == null)
            {
                return NotFound();
            }

            return View(taxiClass);
        }

        // POST: TaxiClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var taxiClass = await _context.TaxiClasses.FindAsync(id);
                _context.TaxiClasses.Remove(taxiClass);
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

        private bool TaxiClassExists(int id)
        {
            return _context.TaxiClasses.Any(e => e.Id == id);
        }
    }
}
