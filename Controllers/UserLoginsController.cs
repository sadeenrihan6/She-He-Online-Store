using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class UserLoginsController : Controller
    {
        private readonly ModelContext _context;

        public UserLoginsController(ModelContext context)
        {
            _context = context;
        }

        // GET: UserLogins
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.UserLogins.Include(u => u.Customer).Include(u => u.Role);
            return View(await modelContext.ToListAsync());
        }

        // GET: UserLogins/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.UserLogins == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.Customer)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // GET: UserLogins/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: UserLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,RoleId,CustomerId")] UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", userLogin.CustomerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userLogin.RoleId);
            return View(userLogin);
        }

        // GET: UserLogins/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            var checkUserr = HttpContext.Session.GetString("AdminName");
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (!string.IsNullOrEmpty(checkUserr) || checkUser != null)
            {
                var userr = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUserr);
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

                if (userr != null)
                {
                    var userIdd = userr.CustomerId;
                    var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);


                    if (admin != null)
                    {
                        ViewBag.Admin = admin;
                        ViewBag.AdminUser = userr;

                        if (id == null || _context.Customers == null)
                        {
                            return NotFound();
                        }

                        var customer = await _context.Customers.FindAsync(id);

                        if (customer == null)
                        {
                            return NotFound();
                        }
                        return View(customer);


                    }
                }
                else if (user != null)
                {
                    var userIdd1 = user.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userIdd1);

                    if (customerr != null)
                    {
                        ViewBag.Customer = customerr;
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd1).Count();
                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd1).Count();

                        if (id == null || _context.UserLogins == null)
                        {
                            return NotFound();
                        }

                        var userLogin = await _context.UserLogins.FindAsync(id);
                        if (userLogin == null)
                        {
                            return NotFound();
                        }
                        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", userLogin.CustomerId);
                        ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userLogin.RoleId);
                        return View(userLogin);



                    }
                }
                else
                {
                    return RedirectToAction("Login", "LoginAndRegister");
                }
                return RedirectToAction("Login", "LoginAndRegister");
            };
            return RedirectToAction("Login", "LoginAndRegister");
        }
        // POST: UserLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, string UserName, string Password)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");
            var checkUserr = HttpContext.Session.GetString("AdminName");

            if (checkUser != null || checkUserr !=null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);
                var admin = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUserr);
                if (user != null)
                {
                    var userIdd = user.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

                    if (customerr != null)
                    {
                        ViewBag.Customer = customerr;
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();
                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
                    }
                    user.UserName = UserName;
                    user.Password = Password;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                   
                }
                else if (admin != null)
                {
                    var userId = admin.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userId);

                    if (customerr != null)
                    {
                        ViewBag.Admin = customerr;
                    }
                    admin.UserName = UserName;
                    admin.Password = Password;
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                        return RedirectToAction("Dashboard", "Admin");
                    
                }
            }
          
            return RedirectToAction("Login", "LoginAndRegister");

        }

        // GET: UserLogins/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.UserLogins == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.Customer)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // POST: UserLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.UserLogins == null)
            {
                return Problem("Entity set 'ModelContext.UserLogins'  is null.");
            }
            var userLogin = await _context.UserLogins.FindAsync(id);
            if (userLogin != null)
            {
                _context.UserLogins.Remove(userLogin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserLoginExists(decimal id)
        {
          return (_context.UserLogins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
