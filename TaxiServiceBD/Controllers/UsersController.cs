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

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,PhoneNumber")] User user)
        {
            using var transaction = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {
                try {

                    string sql = "EXEC spCreateUser @FullName, @PhoneNumber";

                    List<SqlParameter> parms = new List<SqlParameter> {

                    new SqlParameter { ParameterName = "@FullName", Value = user.FullName },
                    new SqlParameter { ParameterName = "@PhoneNumber", Value = user.PhoneNumber }
                 };

                    _context.Database.ExecuteSqlRaw(sql, parms.ToArray());

               
                    await  transaction.CommitAsync();
                    Console.WriteLine("Transaction completed");
                 
                  
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction failed");
                    Console.WriteLine(ex.Message);
                    throw;
                }

                finally
                {
                    transaction.Dispose();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


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

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,PhoneNumber")] User user)
        {
            using var transaction = _context.Database.BeginTransaction();
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
                    new SqlParameter { ParameterName = "@UserId", Value = user.Id /* Value = null*/},
                    new SqlParameter { ParameterName = "@FullName", Value = user.FullName },
                    new SqlParameter { ParameterName = "@PhoneNumber", Value = user.PhoneNumber }
                 };

                    _context.Database.ExecuteSqlRaw(sql, parms.ToArray());
             
                   await  transaction.CommitAsync();
                    Console.WriteLine("Transaction completed");

                }

                catch (Exception ex)
              {
                transaction.Rollback();
                Console.WriteLine("Transaction failed");
                    Console.WriteLine(ex.Message);
                    throw;
              }

            finally
            {
                transaction.Dispose();
            }
               
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


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


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                string sql = "EXEC _spDeleteUser @UserId";
                var entityId = new SqlParameter("@UserId", id);

                _context.Database.ExecuteSqlRaw(sql, entityId);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Transaction failed");
                Console.WriteLine(ex.Message);
                throw;
            }

            finally
            {
                transaction.Dispose();
            }

            return RedirectToAction(nameof(Index));
        
      
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
