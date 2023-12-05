using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        public ReviewsController(ModelContext context, IWebHostEnvironment webHostEnvironment) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
			var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (user != null)
				{
					var userIdd = user.CustomerId;
					var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

					if (admin != null)
					{
						ViewBag.Admin = admin;
						ViewBag.AdminUser = user;
						if (user.RoleId == 1)
						{
							var modelContext = _context.Reviews.Include(r => r.Customer).Include(r => r.Product);
            return View(await modelContext.ToListAsync());
        }
                        return View();
    }
                    else
                    {
                        var adminName1 = HttpContext.Session.GetString("AdminName");
                        if (!string.IsNullOrEmpty(adminName1))
                        {
                            HttpContext.Session.Remove("AdminName");
                        }

                        HttpContext.SignOutAsync("MyCustomAuthScheme");
                        return RedirectToAction("Login", "LoginAndRegister");
                    }
                }
                var adminName2 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName2))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var adminName = HttpContext.Session.GetString("AdminName");
            if (!string.IsNullOrEmpty(adminName))
            {
                HttpContext.Session.Remove("AdminName");
            }

            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
			var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (user != null)
				{
					var userIdd = user.CustomerId;
					var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

					if (admin != null)
					{
						ViewBag.Admin = admin;
						ViewBag.AdminUser = user;
						if (user.RoleId == 1)
						{
							if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

					}
				}
				else
				{
                    var adminName1 = HttpContext.Session.GetString("AdminName");
                    if (!string.IsNullOrEmpty(adminName1))
                    {
                        HttpContext.Session.Remove("AdminName");
                    }

                    HttpContext.SignOutAsync("MyCustomAuthScheme");
                    return RedirectToAction("Login", "LoginAndRegister");
                }
                var adminName2 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName2))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

		// GET: Reviews/Create
		public IActionResult Create()
        {
			var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (user != null)
				{
					var userIdd = user.CustomerId;
					var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

					if (admin != null)
					{
						ViewBag.Admin = admin;
						ViewBag.AdminUser = user;
						if (user.RoleId == 1)
						{
							ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

					}
				}
				else
				{
                    var adminName1 = HttpContext.Session.GetString("AdminName");
                    if (!string.IsNullOrEmpty(adminName1))
                    {
                        HttpContext.Session.Remove("AdminName");
                    }

                    HttpContext.SignOutAsync("MyCustomAuthScheme");
                    return RedirectToAction("Login", "LoginAndRegister");
                }
                var adminName2 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName2))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
			HttpContext.Session.Clear();
			HttpContext.SignOutAsync("MyCustomAuthScheme");
			return RedirectToAction("Login", "LoginAndRegister");
		}

		// POST: Reviews/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,CustomerId,ReviewText,Stars,ReviewDate,OrderId")] Review review)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
			var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (user != null)
				{
					var userIdd = user.CustomerId;
					var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

					if (admin != null)
					{
						ViewBag.Admin = admin;
						ViewBag.AdminUser = user;
						if (user.RoleId == 1)
						{
							if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
			ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id", review.OrderId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);
            ViewBag.StarsRating = new List<decimal> { 1, 2, 3, 4, 5 };
            return View(review);
        }

					}
				}
				else
                {
                    var adminName1 = HttpContext.Session.GetString("AdminName");
                    if (!string.IsNullOrEmpty(adminName1))
                    {
                        HttpContext.Session.Remove("AdminName");
                    }

                    HttpContext.SignOutAsync("MyCustomAuthScheme");
                    return RedirectToAction("Login", "LoginAndRegister");
				}
                var adminName2 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName2))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var adminName = HttpContext.Session.GetString("AdminName");
            if (!string.IsNullOrEmpty(adminName))
            {
                HttpContext.Session.Remove("AdminName");
            }

            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

		// POST: Reviews/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ProductId,CustomerId,ReviewText,Stars,ReviewDate,OrderId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id", review.OrderId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);

            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
			var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (user != null)
				{
					var userIdd = user.CustomerId;
					var admin = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

					if (admin != null)
					{
						ViewBag.Admin = admin;
						ViewBag.AdminUser = user;
						if (user.RoleId == 1)
						{
							if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

					}
				}
				else
				{
                    var adminName1 = HttpContext.Session.GetString("AdminName");
                    if (!string.IsNullOrEmpty(adminName1))
                    {
                        HttpContext.Session.Remove("AdminName");
                    }

                    HttpContext.SignOutAsync("MyCustomAuthScheme");
                    return RedirectToAction("Login", "LoginAndRegister");
                }
                var adminName2 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName2))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var adminName = HttpContext.Session.GetString("AdminName");
            if (!string.IsNullOrEmpty(adminName))
            {
                HttpContext.Session.Remove("AdminName");
            }

            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

		// POST: Reviews/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'ModelContext.Reviews'  is null.");
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(decimal id)
        {
          return (_context.Reviews?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
