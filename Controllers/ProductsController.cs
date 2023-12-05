using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        public ProductsController(ModelContext context, IWebHostEnvironment webHostEnvironment) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
        }

        // GET: Products
        //public async Task<IActionResult> Index()
        //{
        //    var modelContext = _context.Products.Include(p => p.Category);
        //    return View(await modelContext.ToListAsync());
        //}

        public IActionResult Index(string searchString)
        {
			var checkUserr = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUserr))
			{
				var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUserr);

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
							var checkUser = HttpContext.Session.GetString("AdminName");

			if (!string.IsNullOrEmpty(checkUser))
			{
				var userr = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

				if (userr != null)
				{
					var userIdd1 = userr.CustomerId;
					var admin1 = _context.Customers.FirstOrDefault(c => c.Id == userIdd1);

					if (admin1 != null)
					{
						ViewBag.Admin = admin1;
						ViewBag.AdminUser = userr;
						if (user.RoleId == 1)
						{
							var product = from p in _context.Products
                          select p;

                        if (!String.IsNullOrEmpty(searchString))
                        {
                            product = product.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
                        }

                        return View(product);
                                    }
                                    return View();
                                }
                                else
                                {
                                        var adminName11 = HttpContext.Session.GetString("AdminName");
                                        if (!string.IsNullOrEmpty(adminName11))
                                        {
                                            HttpContext.Session.Remove("AdminName");
                                        }

                                        HttpContext.SignOutAsync("MyCustomAuthScheme");
                                        return RedirectToAction("Login", "LoginAndRegister");
                                    }
                                }
                                var adminName22 = HttpContext.Session.GetString("AdminName");
                                if (!string.IsNullOrEmpty(adminName22))
                                {
                                    HttpContext.Session.Remove("AdminName");
                                }

                                HttpContext.SignOutAsync("MyCustomAuthScheme");
                                return RedirectToAction("Login", "LoginAndRegister");
                            }
                            var adminName33 = HttpContext.Session.GetString("AdminName");
                            if (!string.IsNullOrEmpty(adminName33))
                            {
                                HttpContext.Session.Remove("AdminName");
                            }

                            HttpContext.SignOutAsync("MyCustomAuthScheme");
                            return RedirectToAction("Login", "LoginAndRegister");
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
		// GET: Products/Details/5
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
							if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
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

		// GET: Products/Create
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
							ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
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


		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Sale,Price,ImageFile,CategoryId,ProductDescription,Quantityinstock")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImagePath = fileName;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }


        // GET: Products/Edit/5
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
							if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId); // Use "CategoryName" here
            return View(product);
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

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(decimal? id, [Bind("Id,Name,Sale,Price,ImageFile,CategoryId,ProductDescription,Quantityinstock")] Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (product.ImageFile != null)
        //            {
        //                var fileName = Path.GetFileName(product.ImageFile.FileName);
        //                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
        //                var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
        //                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await product.ImageFile.CopyToAsync(stream);
        //                }

        //                product.ImagePath = $"{uniqueFileName}";

        //            }
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
        //    return View(product);
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, string Name, decimal Price, int CategoryId, string ProductDescription, decimal Quantityinstock)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (ModelState.IsValid)
            {
                existingProduct = await _context.Products.FindAsync(id);

                if (existingProduct != null)
                {
                    existingProduct.Name = Name;
                    existingProduct.Price = Price;
                    existingProduct.CategoryId = CategoryId;
                    existingProduct.ProductDescription = ProductDescription;
                    existingProduct.Quantityinstock = Quantityinstock;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", CategoryId);
            return RedirectToAction("Index", "Products");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(decimal id, [Bind("Id,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct != null)
                {
                    if (product.ImageFile != null)
                    {
                        var fileName = Path.GetFileName(product.ImageFile.FileName);
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(stream);
                        }

                        existingProduct.ImagePath = $"{uniqueFileName}";

                        _context.Update(existingProduct);
                        await _context.SaveChangesAsync();
                    }
                }

                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
                return RedirectToAction("Index", "Products");
            }

            return RedirectToAction("Dashboard", "Admin");
        }


        // GET: Products/Delete/5
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
							if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
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

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ModelContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(decimal id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
