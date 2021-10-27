using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaxiServiceBD.Models;

namespace TaxiServiceBD.Controllers
{
    public class UsersController : Controller
    {
        private readonly TaxiServiceContext _context;

        public UsersController(TaxiServiceContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        { 
           var list = await _context.Users.FromSqlRaw<User>("spGetUsers").ToListAsync();
            return View(list);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FromSqlRaw<User>("spGetUserById {0}", id).ToListAsync();
    

            //var user = await _context.Users
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user.FirstOrDefault());
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,PhoneNumber")] User user)
        {
            if (ModelState.IsValid)
            {
                string sql = "EXEC spCreateUser @FullName, @PhoneNumber";

                List<SqlParameter> parms = new List<SqlParameter> { 
       
                    new SqlParameter { ParameterName = "@FullName", Value = user.FullName },
                    new SqlParameter { ParameterName = "@PhoneNumber", Value = user.PhoneNumber }
                 };

                _context.Database.ExecuteSqlRaw(sql, parms.ToArray());
          
                //_context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FromSqlRaw<User>("spGetUserById {0}", id).ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            return View(user.FirstOrDefault());
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,PhoneNumber")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string sql = "EXEC _spUpdateUser @UserId, @FullName,@PhoneNumber";

                    List<SqlParameter> parms = new List<SqlParameter> {
                    new SqlParameter { ParameterName = "@UserId", Value = user.Id },
                    new SqlParameter { ParameterName = "@FullName", Value = user.FullName },
                    new SqlParameter { ParameterName = "@PhoneNumber", Value = user.PhoneNumber }
                 };

                    _context.Database.ExecuteSqlRaw(sql, parms.ToArray());

                    //_context.Add(user);
                    await _context.SaveChangesAsync();
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string sql = "EXEC _spDeleteUser @UserId";
            var entityId = new SqlParameter("@UserId", id);

            _context.Database.ExecuteSqlRaw(sql, entityId);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
