using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Project.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Humanizer;

namespace Project.Controllers
{
    public class ProductCustomersController : Controller
    {
        private readonly ModelContext _context;
		private readonly EmailService _emailService;
        

        public ProductCustomersController(ModelContext context, EmailService emailService)
        {
            _context = context;
			_emailService = emailService;
        }

        // GET: ProductCustomers
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
                            var modelContext = _context.ProductCustomers.Include(p => p.Address).Include(p => p.CouponCodeNavigation).Include(p => p.Customer);

                            var orders = _context.ProductCustomers.Where(o => (o.OrderStatus == "PROCESSING" || o.OrderStatus == "IN TRANSIT" || o.OrderStatus == "DELIVERED") && o.Isemailsent == "FALSE").ToList();

                            foreach (var order in orders)
                            {

                                //var users = _context.UserLogins.Where(u => u.CustomerId == order.CustomerId).ToList();
                                var users = _context.Addresses.Where(u => u.Id == order.AddressId).ToList();
                                if (users != null)
                                {
                                    foreach (var userr in users)
                                    {
                                        var customers = _context.Customers.Where(c => c.Id == userr.CustomerId).ToList();

                                        foreach (var customer in customers)
                                        {

                                            string to = userr.Email;
                                            string subject = "She & He Online Store Order Status Updated";
                                            string body = $"Dear {customer.Fname} {customer.Lname},\n\n" +
                                               "We want to inform you that the status of your order has been updated. Here are the details:\n\n" +
                                               $"Order Number: {order.Id}\n" +
                                               $"New Status: {order.OrderStatus}\n" +
                                               $"Date of Update: {DateTime.Now}\n" +
                                               $"Order Products Quantity: {order.Quantity}\n" +
                                               $"Order Total Price: {order.TotalPrice}\n\n" +
                                               "If you have any questions or need further assistance, please feel free to contact our customer support team at Contact Us Form.\n\n" +
                                               "Thank you for choosing She & He for your watch and bag needs. We appreciate your business and hope you are enjoying your shopping experience with us.\n\n" +
                                               "Best regards,\n" +
                                               "The She & He Team";


                                            order.Isemailsent = "TRUE";
                                            _context.SaveChanges();
                                            _emailService.SendEmail(to, subject, body);

                                        }
                                    }
                                }

                            }

                            return View(await modelContext.ToListAsync());

                        }
                        return View();
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        HttpContext.SignOutAsync("MyCustomAuthScheme");
                        return RedirectToAction("Login", "LoginAndRegister");
                    }
                }
                HttpContext.Session.Clear();
                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

        // GET: ProductCustomers/Details/5
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
							if (id == null || _context.ProductCustomers == null)
            {
                return NotFound();
            }

            var productCustomer = await _context.ProductCustomers
                .Include(p => p.Address)
                .Include(p => p.CouponCodeNavigation)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCustomer == null)
            {
                return NotFound();
            }

            return View(productCustomer);
        }

					}
				}
				else
				{
					HttpContext.Session.Clear();
					HttpContext.SignOutAsync("MyCustomAuthScheme");
					return RedirectToAction("Login", "LoginAndRegister");
				}
				HttpContext.Session.Clear();
				HttpContext.SignOutAsync("MyCustomAuthScheme");
				return RedirectToAction("Login", "LoginAndRegister");
			}
			HttpContext.Session.Clear();
			HttpContext.SignOutAsync("MyCustomAuthScheme");
			return RedirectToAction("Login", "LoginAndRegister");
		}

		// GET: ProductCustomers/Create
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
							ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Id");
            ViewData["CouponCode"] = new SelectList(_context.Coupons, "CouponCode", "CouponCode");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            return View();
        }

					}
				}
				else
				{
					HttpContext.Session.Clear();
					HttpContext.SignOutAsync("MyCustomAuthScheme");
					return RedirectToAction("Login", "LoginAndRegister");
				}
				HttpContext.Session.Clear();
				HttpContext.SignOutAsync("MyCustomAuthScheme");
				return RedirectToAction("Login", "LoginAndRegister");
			}
			HttpContext.Session.Clear();
			HttpContext.SignOutAsync("MyCustomAuthScheme");
			return RedirectToAction("Login", "LoginAndRegister");
		}

		// POST: ProductCustomers/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,Quantity,OrderDate,TotalPrice,CouponCode,OrderStatus,Isemailsent,AddressId")] ProductCustomer productCustomer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCustomer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Id", productCustomer.AddressId);
            ViewData["CouponCode"] = new SelectList(_context.Coupons, "CouponCode", "CouponCode", productCustomer.CouponCode);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", productCustomer.CustomerId);
            return View(productCustomer);
        }

        // GET: ProductCustomers/Edit/5
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
							if (id == null || _context.ProductCustomers == null)
            {
                return NotFound();
            }

            var productCustomer = await _context.ProductCustomers.FindAsync(id);
            if (productCustomer == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Id", productCustomer.AddressId);
            ViewData["CouponCode"] = new SelectList(_context.Coupons, "CouponCode", "CouponCode", productCustomer.CouponCode);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", productCustomer.CustomerId);
            ViewBag.OrderStatusOptions = new List<string> { "PENDING", "PROCESSING", "IN TRANSIT", "DELIVERED" };
            ViewBag.IsemailsentOptions = new List<string> { "TRUE", "FALSE" };

            return View(productCustomer);
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

		// POST: ProductCustomers/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,CustomerId,Quantity,OrderDate,TotalPrice,CouponCode,OrderStatus,Isemailsent,AddressId")] ProductCustomer productCustomer)
        {
            if (id != productCustomer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCustomerExists(productCustomer.Id))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Id", productCustomer.AddressId);
            ViewData["CouponCode"] = new SelectList(_context.Coupons, "CouponCode", "CouponCode", productCustomer.CouponCode);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", productCustomer.CustomerId);

            return View(productCustomer);
        }

        // GET: ProductCustomers/Delete/5
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
							if (id == null || _context.ProductCustomers == null)
            {
                return NotFound();
            }

            var productCustomer = await _context.ProductCustomers
                .Include(p => p.Address)
                .Include(p => p.CouponCodeNavigation)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCustomer == null)
            {
                return NotFound();
            }

            return View(productCustomer);
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

        // POST: ProductCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.ProductCustomers == null)
            {
                return Problem("Entity set 'ModelContext.ProductCustomers'  is null.");
            }
            var productCustomer = await _context.ProductCustomers.FindAsync(id);
            if (productCustomer != null)
            {
                _context.ProductCustomers.Remove(productCustomer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCustomerExists(decimal id)
        {
          return (_context.ProductCustomers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
