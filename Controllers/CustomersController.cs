using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        public CustomersController(ModelContext context, IWebHostEnvironment webHostEnvironment) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
        }

        // GET: Customers
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
							return _context.Customers != null ? 
                          View(await _context.Customers.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Customers'  is null.");
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
    

        // GET: Customers/Details/5
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
							if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
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

        // GET: Customers/Create
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
            var adminName = HttpContext.Session.GetString("AdminName");
            if (!string.IsNullOrEmpty(adminName))
            {
                HttpContext.Session.Remove("AdminName");
            }

            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fname,Lname,Email,Age,City,PhoneNumber,ImageFile")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + customer.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await customer.ImageFile.CopyToAsync(fileStream);
                    }
                    customer.ImagePath = fileName;
                }
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if ( checkUser != null )
			{
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

                 if (user != null)
                {
                    var userIdd1 = user.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userIdd1);

                    if (customerr != null)
                    {
                        ViewBag.Customer = customerr;
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd1).Count();
                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd1).Count();

                        if (id == null || _context.Customers == null)
                        {
                            return NotFound();
                        }

                        var customer = await _context.Customers.FindAsync(id);
                        var userLogin =  _context.UserLogins.Where(u => u.CustomerId == customer.Id).FirstOrDefault();

                        if (customer == null || userLogin == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            var customerUser = Tuple.Create<Customer, UserLogin>(customer, userLogin);
                            return View(customerUser);
                        }
                        


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


					




		public async Task<IActionResult> EditAdmin(decimal? id)
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
                    }
				}
			}
			if (id == null || _context.Customers == null)
			{
				return NotFound();
			}

            var customer = await _context.Customers.FindAsync(id);
            var userLogin = _context.UserLogins.Where(u => u.CustomerId == customer.Id).FirstOrDefault();

            if (customer == null || userLogin == null)
            {
                return NotFound();
            }
            else
            {
                var customerUser = Tuple.Create<Customer, UserLogin>(customer, userLogin);
                return View(customerUser);
            }
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Change the name
        [HttpPost]
        public async Task<IActionResult> ChangeName(decimal id, string Fname, string Lname)
        {

            var checkUser = HttpContext.Session.GetInt32("UserId");
            var checkUserr = HttpContext.Session.GetString("AdminName");

            if (checkUser != null || checkUserr != null)
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

                        customerr.Fname = Fname;
                        customerr.Lname = Lname;
                        _context.Update(customerr);
                        await _context.SaveChangesAsync();
                    }


                    return RedirectToAction("Index", "Home");

                }
                else if (admin != null)
                {
                    var userId = admin.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userId);

                    if (customerr != null)
                    {
                        ViewBag.Admin = customerr;
                        customerr.Fname = Fname;
                        customerr.Lname = Lname;
                        _context.Update(customerr);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Dashboard", "Admin");

                }
            }

            return RedirectToAction("Login", "LoginAndRegister");
        }

            //var checkUser = HttpContext.Session.GetInt32("UserId");

            //if (checkUser != null)
            //{
            //    var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

            //    if (user != null)
            //    {
            //        var userIdd = user.CustomerId;
            //        var customerr = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

            //        if (customerr != null)
            //        {
            //            ViewBag.Customer = customerr;
            //            ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();
            //            ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
            //        }
            //    }
            //}


            //if (ModelState.IsValid)
            //{
            //    var existingCustomer = await _context.Customers.FindAsync(id);

            //    if (existingCustomer != null)
            //    {
            //        if (existingCustomer.Fname != null || existingCustomer.Lname != null)
            //        {
            //            // Update only the name
            //            existingCustomer.Fname = Fname;
            //            existingCustomer.Lname = Lname;
            //            _context.Update(existingCustomer);
            //            await _context.SaveChangesAsync();

            //            if(id != 21)
            //            {
            //                return RedirectToAction("Index", "Home");
            //            }
            //            else
            //            {
            //                return RedirectToAction("Dashboard", "Admin");
            //            }

            //        }
            //    }
            //}

            //// Handle validation errors or if the customer is not found
            //return RedirectToAction("Login", "LoginAndRegister");
        //}


        // Change the photo
        //      [HttpPost]
        //public async Task<IActionResult> ChangePhoto(decimal id, [Bind("Id,ImageFile")] Customer customer)
        //{
        //          // Check if the customer is authenticated
        //          var checkUser = HttpContext.Session.GetInt32("UserId");

        //          if (checkUser != null)
        //          {
        //              var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

        //              if (user != null)
        //		{
        //			var userIdd = user.CustomerId;
        //			var customerr = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

        //			if (customerr != null)
        //			{
        //				ViewBag.Customer = customerr;
        //				ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();
        //				ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
        //			}
        //		}
        //	}

        //	if (id != customer.Id)
        //	{
        //		return NotFound();
        //	}

        //	if (ModelState.IsValid)
        //	{
        //		var existingCustomer = await _context.Customers.FindAsync(id);

        //		if (existingCustomer != null)
        //		{
        //			if (customer.ImageFile != null)
        //			{
        //				var fileName = Path.GetFileName(customer.ImageFile.FileName);
        //				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
        //				var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
        //				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //				using (var stream = new FileStream(filePath, FileMode.Create))
        //				{
        //					await customer.ImageFile.CopyToAsync(stream);
        //				}

        //				existingCustomer.ImagePath = $"{uniqueFileName}";

        //				_context.Update(existingCustomer);
        //				await _context.SaveChangesAsync();

        //                      if (id != 21)
        //                      {
        //                          return RedirectToAction("Index", "Home");
        //                      }
        //                      else
        //                      {
        //                          return RedirectToAction("Dashboard", "Admin");
        //                      }
        //                  }
        //		}
        //	}


        //	// Handle validation errors or if the customer is not found
        //	return RedirectToAction("Login", "LoginAndRegister");
        //}


            [HttpPost]
        public async Task<IActionResult> ChangePhoto(decimal id, IFormFile ImageFile)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");
            var checkUserr = HttpContext.Session.GetString("AdminName");

            if (checkUser != null || checkUserr != null)
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

                        if (ImageFile != null)
                        {
                            var fileName = Path.GetFileName(ImageFile.FileName);
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await ImageFile.CopyToAsync(stream);
                            }

                            customerr.ImagePath = $"{uniqueFileName}";

                            _context.Update(customerr);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else if (admin != null)
                {
                    var userId = admin.CustomerId;
                    var customerr = _context.Customers.FirstOrDefault(c => c.Id == userId);

                    if (customerr != null)
                    {
                        ViewBag.Admin = customerr;

                        if (ImageFile != null)
                        {
                            var fileName = Path.GetFileName(ImageFile.FileName);
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await ImageFile.CopyToAsync(stream);
                            }

                            customerr.ImagePath = $"{uniqueFileName}";

                            _context.Update(customerr);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Dashboard", "Admin");
                        }
                    }
                }
            }

            return RedirectToAction("Login", "LoginAndRegister");
        }




        // GET: Customers/Delete/5
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
							if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
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

		// POST: Customers/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ModelContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(decimal id)
        {
          return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
