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
    public class CategoriesController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        public CategoriesController(ModelContext context, IWebHostEnvironment webHostEnvironment) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
        }

        // GET: Categories
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
                            var modelContext = _context.Categories.Include(c => c.ParentCategory);

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

        // GET: Categories/Details/5
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
                            if (id == null || _context.Categories == null)
                            {
                                return NotFound();
                            }

                            var category = await _context.Categories
                                .Include(c => c.ParentCategory)
                                .FirstOrDefaultAsync(m => m.Id == id);
                            if (category == null)
                            {
                                return NotFound();
                            }

                            return View(category);
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
        
    



	// GET: Categories/Create
	public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryName,ImageFile,ParentCategoryId")] Category category)
        {

            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(fileStream);
                    }
                    category.ImagePath = fileName;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);

        }
        // GET: Categories/Edit/5
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
                            if (id == null || _context.Categories == null)
                            {
                                return NotFound();
                            }

                            var category = await _context.Categories.FindAsync(id);
                            if (category == null)
                            {
                                return NotFound();
                            }
                            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
                            return View(category);

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
                var adminName3 = HttpContext.Session.GetString("AdminName");
                if (!string.IsNullOrEmpty(adminName3))
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

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id,  string CategoryName, decimal ParentCategoryId)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (ModelState.IsValid)
            {
                existingCategory = await _context.Categories.FindAsync(id);

                if (existingCategory != null)
                {
                    existingCategory.CategoryName = CategoryName;
                    existingCategory.ParentCategoryId = ParentCategoryId;

                    _context.Update(existingCategory);
                    await _context.SaveChangesAsync();
                }
            }

            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", ParentCategoryId);
            return RedirectToAction("Index", "Categories");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(decimal id, [Bind("Id,ImageFile")] Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory != null)
                {
                    if (category.ImageFile != null)
                    {
                        var fileName = Path.GetFileName(category.ImageFile.FileName);
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await category.ImageFile.CopyToAsync(stream);
                        }

                        existingCategory.ImagePath = $"{uniqueFileName}";

                        _context.Update(existingCategory);
                        await _context.SaveChangesAsync();
                    }
                }

                ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
                return RedirectToAction("Index", "Categories");
            }

            return RedirectToAction("Dashboard", "Admin");
        }



        //[HttpPost]
        //     [ValidateAntiForgeryToken]
        //     public async Task<IActionResult> Edit(decimal id, [Bind("Id,CategoryName,ImageFile,ParentCategoryId")] Category category)
        //     {
        //         if (id != category.Id)
        //         {
        //             return NotFound();
        //         }

        //         if (ModelState.IsValid)
        //         {
        //             try
        //             {
        //                 if (category.ImageFile != null)
        //                 {
        //                     var fileName = Path.GetFileName(category.ImageFile.FileName);
        //                     var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
        //                     var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
        //                     var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //                     using (var stream = new FileStream(filePath, FileMode.Create))
        //                     {
        //                         await category.ImageFile.CopyToAsync(stream);
        //                     }

        //                     category.ImagePath = $"{uniqueFileName}";

        //                 }
        //                 _context.Update(category);
        //                 await _context.SaveChangesAsync();
        //             }
        //             catch (DbUpdateConcurrencyException)
        //             {
        //                 if (!CategoryExists(category.Id))
        //                 {
        //                     return NotFound();
        //                 }
        //                 else
        //                 {
        //                     throw;
        //                 }
        //             }
        //             return RedirectToAction(nameof(Index));
        //         }
        //         ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
        //         return View(category);
        //     }


        // GET: Categories/Delete/5
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
							if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
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

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ModelContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(decimal id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
