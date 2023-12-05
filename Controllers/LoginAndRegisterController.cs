using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;

namespace Project.Controllers
{

    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnvironment) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }



		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register([Bind("Id,Fname,Lname,ImageFile")] Customer customer, string UserName, string Password)
		{
			var userExists = _context.UserLogins.Any(u => u.UserName == UserName);

			if (userExists)
			{
				ModelState.AddModelError("UserName", "Username already exists.");
				return View("Register");
			}

			if (ModelState.IsValid)
			{
				using (var transaction = _context.Database.BeginTransaction())
				{
					try
					{
						if (customer.ImageFile != null)
						{
							string wwwRootPath = _webHostEnvironment.WebRootPath;
							string fileName = Guid.NewGuid().ToString() + "_" + customer.ImageFile.FileName;
							string path = Path.Combine(wwwRootPath, "Images", fileName);

							using (var fileStream = new FileStream(path, FileMode.Create))
							{
								await customer.ImageFile.CopyToAsync(fileStream);
							}

							customer.ImagePath = fileName;
						}

						_context.Add(customer);
						await _context.SaveChangesAsync();

						UserLogin login = new UserLogin
						{
							RoleId = 2,
							UserName = UserName,
							Password = Password,
							CustomerId = customer.Id
						};

						_context.Add(login);
						await _context.SaveChangesAsync();

						transaction.Commit();
						return RedirectToAction("Login");
					}
					catch (Exception)
					{
						transaction.Rollback();
						ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
						return View("Register");
					}
				}
			}

			return View("Register");
		}



		[HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl)
        {
            var checkUser = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(checkUser))
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.UserName == checkUser);

                if (user != null)
                {
                    var userIdd = user.CustomerId;

                    var customer = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

                    if (customer != null)
                    {
                        ViewBag.Customer = customer;
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();

                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();


                    }

                }

            }
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName", "Password")] UserLogin userLogin, string ReturnUrl)
        {

            var auth = _context.UserLogins
                .Where(x => x.UserName == userLogin.UserName && x.Password == userLogin.Password)
                .SingleOrDefault();

            if (auth != null)
            {

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userLogin.UserName),
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60), 
                };

                await HttpContext.SignInAsync("MyCustomAuthScheme", principal, authProperties);



                switch (auth.RoleId)
                {
                    case 1:
                        HttpContext.Session.SetString("AdminName", auth.UserName);
                        ViewBag.UserName = HttpContext.Session.GetString("AdminName");
                        return RedirectToAction("Dashboard", "Admin");

                    case 2:
                        HttpContext.Session.SetInt32("UserId", (int)auth.CustomerId);
                        HttpContext.Session.SetString("UserName", auth.UserName);
                        ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                        ViewBag.UserName = HttpContext.Session.GetString("UserName");

                        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            return LocalRedirect(ReturnUrl); // Redirect to the original URL
                        }

                        return RedirectToAction("Index", "Home"); // Default action if ReturnUrl is not provided or is not local
                }
            }

            return View();
        }



        public IActionResult Logout()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var adminName = HttpContext.Session.GetString("AdminName");
            if (!string.IsNullOrEmpty(userName))
            {
                HttpContext.Session.Remove("UserName");
            }
            else if (!string.IsNullOrEmpty(adminName))
            {
                HttpContext.Session.Remove("AdminName");
            }

            HttpContext.SignOutAsync("MyCustomAuthScheme");
            return RedirectToAction("Login", "LoginAndRegister");
        }


    }
}
