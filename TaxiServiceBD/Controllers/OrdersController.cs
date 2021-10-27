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
        public async Task<IActionResult> Index()
        {
            var taxiServiceContext = _context.Orders.Include(o => o.CategoryClass).Include(o => o.Driver).Include(o => o.User);
            return View(await taxiServiceContext.ToListAsync());
        }

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
            ViewData["CategoryClassId"] = new SelectList(_context.CategoriesClassDetails, "Id", "Id");
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
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryClassId"] = new SelectList(_context.CategoriesClassDetails, "Id", "Id", order.CategoryClassId);
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
            ViewData["CategoryClassId"] = new SelectList(_context.CategoriesClassDetails, "Id", "Id", order.CategoryClassId);
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
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CategoryClassId"] = new SelectList(_context.CategoriesClassDetails, "Id", "Id", order.CategoryClassId);
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
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
