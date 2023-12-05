using Microsoft.AspNetCore.Mvc;
using Project.Models;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MailKit.Search;
using System.DirectoryServices;


namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

		private readonly ModelContext _context;

        private readonly EmailService _emailService;
        public HomeController(ILogger<HomeController> logger, ModelContext context, EmailService emailService)
        {
			_context = context;
			_logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            
            ViewBag.Categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToList();
            var testimonial = _context.Testimonials.Where(t => t.TestimonialsStatus == "Approved").ToList();
            var customers = _context.Customers.ToList();
            var testimonialUser = from t in testimonial
                                  join c in customers
                                  on t.CustomerId equals c.Id
                                  select new TestimonialsJoin
                                  {
                                      Testimonial = t,
                                      Customer = c
                                  };

            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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

            return View(testimonialUser);
        }
        public IActionResult ChildCategories(int id)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

                if (user != null)
                {
                    var userIdd = user.CustomerId;
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

                    if (customer != null)
                    {
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();
                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
                        ViewBag.Customer = customer;
                    }
                }
            }
            var childCategories = _context.Categories.Where(c => c.ParentCategoryId == id).ToList();


            return View(childCategories);
        }

        public IActionResult Shop(string? sorting, string? priceFilter, string searchString)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

                if (user != null)
                {
                    var userIdd = user.CustomerId;
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

                    if (customer != null)
                    {
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();
                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
                        ViewBag.Customer = customer;
                    }
                }
            }
            var products = _context.Products.ToList();
            if (string.IsNullOrEmpty(searchString))
            {
                ViewBag.found = true;

                if (sorting != null && priceFilter == null)
                {
                    if (sorting == "low-high")
                    {
                        products = _context.Products.OrderBy(p => p.Price).ToList();
                    }
                    else if (sorting == "high-low")
                    {
                        products = _context.Products.OrderByDescending(p => p.Price).ToList();
                    }
                }
                else if (priceFilter != null && sorting == null)
                {
                    if (priceFilter == "greaterThan50")
                    {
                        products = _context.Products.Where(p => p.Price > 50).OrderBy(p => p.Price).ToList();
                    }
                    else if (priceFilter == "lessThanOrEqual50")
                    {
                        products = _context.Products.Where(p => p.Price <= 50).OrderBy(p => p.Price).ToList();
                    }
                }
                else if (priceFilter != null && sorting != null)
                {
                    if (sorting == "low-high" && priceFilter == "greaterThan50")
                    {
                        products = _context.Products.Where(p => p.Price > 50).OrderBy(p => p.Price).ToList();
                    }
                    else if (sorting == "low-high" && priceFilter == "lessThanOrEqual50")
                    {
                        products = _context.Products.Where(p => p.Price <= 50).OrderBy(p => p.Price).ToList();
                    }
                    else if (sorting == "high-low" && priceFilter == "greaterThan50")
                    {
                        products = _context.Products.Where(p => p.Price > 50).OrderByDescending(p => p.Price).ToList();
                    }
                    else if (sorting == "high-low" && priceFilter == "lessThanOrEqual50")
                    {
                        products = _context.Products.Where(p => p.Price <= 50).OrderByDescending(p => p.Price).ToList();
                    }
                }

            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.search = searchString;
                ViewBag.found = true;
                {
                    if (sorting == null && priceFilter == null)
                    {
                        searchString = searchString.ToLower();
                        var searchResults = _context.Products.Where(p => p.Name.ToLower().Contains(searchString)).ToList();

                        if (searchResults.Any())
                        {
                            products = searchResults;
                            ViewBag.found = true;
                        }
                        else
                        {
                            ViewBag.found = false;
                        }
                    }
                    else if (sorting != null && priceFilter == null)
                    {
                        if (sorting == "low-high")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Name.ToLower().Contains(searchString)).OrderBy(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                        else if (sorting == "high-low")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Name.ToLower().Contains(searchString)).OrderByDescending(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                    }
                    else if (priceFilter != null && sorting == null)
                    {
                        if (priceFilter == "greaterThan50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price > 50 && p.Name.ToLower().Contains(searchString)).OrderBy(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                        else if (priceFilter == "lessThanOrEqual50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price <= 50 && p.Name.ToLower().Contains(searchString)).OrderBy(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                    }
                    else if (priceFilter != null && sorting != null)
                    {
                        if (sorting == "low-high" && priceFilter == "greaterThan50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price > 50 && p.Name.ToLower().Contains(searchString)).OrderBy(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                        else if (sorting == "low-high" && priceFilter == "lessThanOrEqual50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price <= 50 && p.Name.ToLower().Contains(searchString)).OrderBy(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                        else if (sorting == "high-low" && priceFilter == "greaterThan50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price > 50 && p.Name.ToLower().Contains(searchString)).OrderByDescending(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                        else if (sorting == "high-low" && priceFilter == "lessThanOrEqual50")
                        {
                            searchString = searchString.ToLower();
                            var searchResults = _context.Products.Where(p => p.Price <= 50 && p.Name.ToLower().Contains(searchString)).OrderByDescending(p => p.Price).ToList();
                            if (searchResults.Any())
                            {
                                products = searchResults;
                                ViewBag.found = true;
                            }
                            else
                            {
                                ViewBag.found = false;
                            }
                        }
                    }

                }
            }


            else
            {
                ViewBag.found = true;

            }


            var categories = _context.Categories.Where(c => c.ParentCategoryId != null).ToList();
            ViewBag.productsCount = _context.Products.Count();
            var productCategoryModel = Tuple.Create<IEnumerable<Category>, IEnumerable<Product>>(categories, products);
            return View(productCategoryModel);
        }
     


        public IActionResult ProductsByCategory(int? id)
        {
            
            string referringUrl = Request.Headers["Referer"].ToString();
            ViewBag.returnUrl = referringUrl;
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            var products = _context.Products.Where(p => p.CategoryId == id).ToList();

            return View(products);
        }

        public IActionResult Coupon()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            var coupons = _context.Coupons.Where(c => c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now).ToList();
            ViewBag.Coupon = coupons;

            return View();
        }
        
        public IActionResult ProductDetails(int id, string searchString)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

                if (user != null)
                {
                    var userIdd = user.CustomerId;
                    
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == userIdd);

                    if (customer != null)
                    {
                        ViewBag.Customer = customer;
                        ViewBag.wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).Count();

                        ViewBag.cartItems = _context.ShoppingCarts.Where(w => w.CustomerId == userIdd).Count();
                        var wishlistItems = _context.Wishlists.Where(w => w.CustomerId == userIdd).ToList();

                        if (wishlistItems != null)
                        {
                            ViewBag.check = wishlistItems.Any(wish => wish.ProductId == id);
                        }

                    }
                    
                }

            }
            var product = _context.Products.Where(p => p.Id == id).ToList();
            var relatedProduct = _context.Products.Where(p => p.CategoryId == product[0].CategoryId && p.Id != id).Take(3);
            
            var customers = _context.Customers.ToList();
            var reviews = _context.Reviews.Where(r => r.ProductId == id).ToList();

            var userReview = from c in customers
                         join r in reviews
                         on c.Id equals r.CustomerId
                         select new ReviewJoin{
                             Customer = c,
                             Review = r
                        };
            ViewBag.reviewsAvg = _context.Reviews.Where(review => review.ProductId == id)
                .Average(review => review.Stars);


            ViewBag.reviewsCount = _context.Reviews.Where(r => r.ProductId == id).Count();
            var productDetailsl = Tuple.Create<IEnumerable<Product>, IEnumerable<Product>, IEnumerable<ReviewJoin>>(relatedProduct, product, userReview);
            return View(productDetailsl);
        }

        public IActionResult TermsAndConditions()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
        }
        public IActionResult AboutUs()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
        }
        public IActionResult AvailableServices()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
        }
        public IActionResult FAndQ()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
        }
        [Authorize(AuthenticationSchemes = "MyCustomAuthScheme")]
        public IActionResult WishList(int? id, string? returnUrl)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");
            string referringUrl = Request.Headers["Referer"].ToString();

            var referringUri = new Uri(referringUrl);

            var pathBeforeReturnUrl = referringUri.GetLeftPart(UriPartial.Path);

            var loginUrl = Url.Action("Login", "LoginAndRegister");

            if (pathBeforeReturnUrl.Contains(loginUrl))
            {
                var homeUrl = Url.Action("Index", "Home");
                ViewBag.ReturnUrl = homeUrl;
            }
            else
            {
                ViewBag.ReturnUrl = pathBeforeReturnUrl;
            }
            

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;
                ViewBag.UserId = userId;

                bool isProductInWishlist = _context.Wishlists
                    .Any(w => w.CustomerId == userId && w.ProductId == id);

                if (!isProductInWishlist && id != null)
                {
                    Wishlist wishlist = new Wishlist
                    {
                        CustomerId = userId,
                        ProductId = id
                    };

                    _context.Wishlists.Add(wishlist);
                    _context.SaveChanges();
                    if(returnUrl != null)
                    {
                        return RedirectToAction("WisList", new { returnUrl = ViewBag.ReturnUrl });
                    }
                   
                    //return RedirectToAction("ProductDetails", new { id = id});
                    // return Redirect(ViewBag.ReturnUrl);
                }

                var wishlists = _context.Wishlists.Where(w => w.CustomerId == userId).ToList();
                List<Product> products = new List<Product>();

                foreach (var item in wishlists)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }

                return View(products);
            }
            else
            {

                var ReturnUrl = HttpContext.Request.Path.ToString();
                return RedirectToAction("Login", "LoginAndRegister", new { ReturnUrl = ReturnUrl });
                //return RedirectToAction("Login", "LoginAndRegister");
            }
        }

        public IActionResult RemoveFromWishlist(int id)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                var itemToRemove = _context.Wishlists.FirstOrDefault(w => w.CustomerId == userId && w.ProductId == id);

                if (itemToRemove != null)
                {
                    _context.Wishlists.Remove(itemToRemove);
                    _context.SaveChanges();
                }
            }

            // Redirect back to the wishlist view
            return RedirectToAction("WishList");
        }
        [Authorize(AuthenticationSchemes = "MyCustomAuthScheme")]
        public IActionResult ShoppingCart(int? id, string? CouponCode, int total, string returnUrl, int quantity = 1)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");
            string referringUrl = Request.Headers["Referer"].ToString();

            var referringUri = new Uri(referringUrl);

            var pathBeforeReturnUrl = referringUri.GetLeftPart(UriPartial.Path);

            var loginUrl = Url.Action("Login", "LoginAndRegister");

            if (pathBeforeReturnUrl.Contains(loginUrl))
            {
                var homeUrl = Url.Action("Index", "Home");
                ViewBag.ReturnUrl = homeUrl;
            }
            else
            {
                ViewBag.ReturnUrl = pathBeforeReturnUrl;
            }


            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);


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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;
                ViewBag.UserId = userId;

                bool isProductInCart = _context.ShoppingCarts
                    .Any(s => s.CustomerId == userId && s.ProductId == id);

                if (!isProductInCart && id != null)
                {
                    ShoppingCart shippingCart = new ShoppingCart
                    {
                        CustomerId = userId,
                        ProductId = id,
                        Quantity = quantity,
                        CartDate = DateTime.Now,
                        CouponCode = CouponCode
                    };

                    _context.ShoppingCarts.Add(shippingCart);
                    _context.SaveChanges();
                    return RedirectToAction("ShoppingCart", new { returnUrl = ViewBag.ReturnUrl });
                    //return Redirect(referringUrl);
                }

                var shippingCarts = _context.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();
                List<Product> products = new List<Product>();

                foreach (var item in shippingCarts)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null && product.Quantityinstock > 0)
                    {
                        products.Add(product);
                    }
                    
                    
                }

                

                var shoppingProducts = Tuple.Create<IEnumerable<Product>, IEnumerable<ShoppingCart>>(products, shippingCarts);
                ViewData["CouponCode"] = CouponCode;
                DiscountCalculation(total, CouponCode);
                return View(shoppingProducts);
            }
            else
            {  var ReturnUrl = HttpContext.Request.Path.ToString();
                    return RedirectToAction("Login", "LoginAndRegister", new { ReturnUrl = ReturnUrl });
                
                //return RedirectToAction("Login", "LoginAndRegister");
            }
        }
        public IActionResult UpdateQuantity(int id, int quantity = 1)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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

            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                var cartItem = _context.ShoppingCarts
                    .FirstOrDefault(s => s.CustomerId == userId && s.ProductId == id);

                if (cartItem != null)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == id);

                    if (product != null)
                    {
                        if (quantity <= product.Quantityinstock)
                        {
                            
                            cartItem.Quantity = quantity;
                            _context.SaveChanges();
                        }
                        else
                        {

                            cartItem.Quantity = product.Quantityinstock;
                            _context.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("ShoppingCart");
            }
            else
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
        }

        public IActionResult RemoveFromCart(int? id)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                var itemToRemove = _context.ShoppingCarts.FirstOrDefault(s => s.CustomerId == userId && s.ProductId == id);

                if (itemToRemove != null)
                {
                    _context.ShoppingCarts.Remove(itemToRemove);
                    _context.SaveChanges();
                }

                
            }

            // Redirect back to the wishlist view
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult DiscountCalculation(int total, string? CouponCode)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            if (total != null && !string.IsNullOrEmpty(CouponCode))
            {
                var coupon = _context.Coupons.SingleOrDefault(c => c.CouponCode.ToLower() == CouponCode.ToLower() &&  c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);

                if (coupon != null)
                {
                    decimal discountAmount = Convert.ToDecimal(total * coupon.DiscountValue / 100);
                    decimal discountedTotal = total - discountAmount;

                    // Pass the discounted total to the view
                    ViewBag.DiscountedTotal = discountedTotal;

                    return View("ShoppingCart"); // Return the ShoppingCart view
                }
            }

            // Handle invalid coupon or missing data
            return RedirectToAction("ShoppingCart");
        }
        [HttpGet]
        public IActionResult CheckOutOrder ( decimal total, string couponCode)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");
            
            

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;
                ViewBag.UserId = userId;

                var shippingCarts = _context.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();
                List<Product> productss = new List<Product>();

                foreach (var item in shippingCarts)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        productss.Add(product);
                    }
                }

                int delivery = 0;
               
                if (total > 100)
                {
                    delivery = 0;
                    HttpContext.Session.SetString("Delivery", delivery.ToString());
                    ViewBag.delivery = HttpContext.Session.GetString("Delivery");
                }
                else
                {
                    delivery = 5;
                    HttpContext.Session.SetString("Delivery", delivery.ToString());
                    ViewBag.delivery = HttpContext.Session.GetString("Delivery");
                   
                }
                
                if (couponCode != null)
                {
                    var coupon = _context.Coupons.Where(c => c.CouponCode.ToLower() == couponCode.ToLower()).FirstOrDefault();
                    if (coupon != null)
                    {
                        var totalPrice = Convert.ToDecimal(total - total * coupon.DiscountValue / 100) + delivery;
                        HttpContext.Session.SetString("TotalPrice", totalPrice.ToString());
                        ViewBag.totalPrice = HttpContext.Session.GetString("TotalPrice");
                        ViewBag.CouponCode = couponCode;
                    }
                   
                }
                else
                {
                    var totalPrice = total + delivery;
                    HttpContext.Session.SetString("TotalPrice", totalPrice.ToString());
                    ViewBag.totalPrice = HttpContext.Session.GetString("TotalPrice");
                }
                

                //-----------------------------------------------------------------

                var userOrder = _context.OrderInfos.Where(o => o.CustomerId == userId).ToList();

                List<Product> products = new List<Product>();

                foreach (var item in userOrder)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }
                var shoppingProducts = Tuple.Create<IEnumerable<Product>, IEnumerable<ShoppingCart>>(productss, shippingCarts);
                //var userOrderProducts = Tuple.Create<IEnumerable<OrderInfo>, IEnumerable<Product>>(userOrder, products);
                return View(shoppingProducts);
            }
            else
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
        }

        [HttpPost]
        public IActionResult CustomerInformation(decimal totalPrice, string? CouponCode, string fName, string lName, string email, decimal phoneNumber, string addressLine, string city, string cardFirstName, string cardFLastName, string cardNumber, string expirationDate, string cvv)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;
                var shippingCarts = _context.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();
                
                decimal addressId;

                var existingAddress = _context.Addresses.FirstOrDefault(a =>
                    a.CustomerId == userId &&
                    a.FirstName == fName &&
                    a.LastName == lName &&
                    a.Email == email &&
                    a.PhoneNumber == phoneNumber &&
                    a.StreetAddress == addressLine &&
                    a.City == city
                );

                if (existingAddress != null)
                {
                    addressId = existingAddress.Id;
                }
                else
                {
                    Address userAddress = new Address
                    {
                        CustomerId = userIdCheck.Value,
                        City = city,
                        StreetAddress = addressLine,
                        PhoneNumber = phoneNumber,
                        FirstName = fName,
                        LastName = lName,
                        Email = email,
                    };

                    _context.Addresses.Add(userAddress);
                    _context.SaveChanges();

                    // Get the generated ID
                    addressId = userAddress.Id;
                }
                var card = _context.Payments.SingleOrDefault(p => p.FirstName == cardFirstName && p.LastName == cardFLastName && p.CardNumber == cardNumber && p.ExpiryDate == expirationDate && p.Cvv == cvv);

                if (card != null)
                {
                    if (card.Balance >= totalPrice)
                    {
                        card.Balance -= totalPrice;
                        _context.SaveChanges();

                        int quantity = 0;
                        foreach (var item in shippingCarts)
                        {
                            quantity += Convert.ToInt32(item.Quantity);
                        }
                        ProductCustomer orders = new ProductCustomer
                        {
                            CustomerId = userId,
                            Quantity = quantity,
                            OrderDate = DateTime.Now,
                            CouponCode = CouponCode,
                            TotalPrice = totalPrice,
                            OrderStatus = "PENDING",
                            AddressId   = addressId,
                            Isemailsent = "FALSE"
                        };
                        _context.ProductCustomers.Add(orders);
                        _context.SaveChanges();


                        foreach (var item in shippingCarts)
                        {
                            OrderInfo order = new OrderInfo
                            {
                                OrderId = orders.Id,
                                ProductId = item.ProductId,
                                TotalPrice = totalPrice,
                                CustomerId = userId,
                                Quantity = item.Quantity,
                            };
                            _context.OrderInfos.Add(order);
                            _context.SaveChanges();

                            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

                            if (product != null)
                            {
                                product.Quantityinstock -= item.Quantity;
                                _context.SaveChanges();
                            }
                        }

                        var cartClear = _context.ShoppingCarts.Where(c => c.CustomerId == userId).ToList();
                        foreach (var cart in cartClear)
                        {
                            _context.ShoppingCarts.Remove(cart);
                        }
                        _context.SaveChanges();



                        return RedirectToAction("MyOrders");



                    }
                    else
                    {
                        TempData["InsufficientFunds"] = "Card amount is not enough.";
                        return RedirectToAction("CheckOutOrder");
                    }
                }
                else
                {
                    TempData["CardNotFound"] = "Card not found!!";
                    return RedirectToAction("CheckOutOrder");
                }

            }

            return RedirectToAction("Login", "LoginAndRegister");
        }
        public IActionResult MyOrders()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;
                ViewBag.userId = userId;
                var orders = _context.ProductCustomers.Where(o => o.CustomerId == userId).OrderByDescending(o => o.OrderDate).ToList();
                
                var orderViewModels = new List<OrdersJoin>();

                foreach (var order in orders)
                {
                    var orderInfo = _context.OrderInfos
                        .Where(oi => oi.OrderId == order.Id)
                        .ToList();

                    var address = _context.Addresses.Where(a => a.Id == order.AddressId).FirstOrDefault();

                    var products = new List<Product>();

                    foreach (var orderItem in orderInfo)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == orderItem.ProductId);

                        if (product != null)
                        {
                            products.Add(product);
                        }
                    }
                    var orderViewModel = new OrdersJoin(_context)
                    {
                        Order = order,
                        OrderInfo = orderInfo,
                        Products = products,
                        Address = address
                    };

                    orderViewModels.Add(orderViewModel);
                }


                return View(orderViewModels);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public IActionResult Reviews(int id, string reviewText, int starsRating, int order)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                    GetUserReviewForProduct(id);
                    Review review = new Review()
                    {
                        ReviewDate = DateTime.Now,
                        ReviewText = reviewText, 
                        ProductId = id,
                        CustomerId = userId,
                        Stars = starsRating,
                        OrderId = order
                    };

                    _context.Reviews.Add(review);
                    _context.SaveChanges();

               

                return RedirectToAction("MyOrders");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult GetUserReviewForProduct(int id)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                var existingReview = _context.Reviews.FirstOrDefault(r => r.CustomerId == userId && r.ProductId == id);

                if (existingReview != null)
                {
                    ViewBag.check = true;
                    ViewBag.review = existingReview;
                    return RedirectToAction("MyOrders");
                }
                else
                {
                    ViewBag.check = false;
                    return RedirectToAction("MyOrders");
                }
            }

            return RedirectToAction("Login");
        }

		public IActionResult ContactUs()
		{
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
		}


        [HttpPost]
        public IActionResult ContactUs(ContactFormModel model)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            if (ModelState.IsValid)
            {
                string to = "onlinebookaddress@gmail.com"; 
                string subject = "She & He Online Store Contact Form";
                string body = $"User Name: {model.SenderName}\nUser Email: {model.Sender}\nMessage: {model.Message}";

                _emailService.SendEmail(to, subject, body);

                TempData["Message"] = "Your message has been sent. Thank you!";
                return RedirectToAction("ContactUs");
            }

            return View(model);
        }
        public IActionResult Testimonial()
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            return View();
        }
        [HttpPost]
        public IActionResult Testimonial(decimal id, string testimonialText)
        {
            var checkUser = HttpContext.Session.GetInt32("UserId");

            if (checkUser != null)
            {
                var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == checkUser);

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
            int? userIdCheck = HttpContext.Session.GetInt32("UserId");

            if (userIdCheck != null)
            {
                int userId = userIdCheck.Value;

                // Create a new Testimonial object
                Testimonial testimonial = new Testimonial()
                {
                    CustomerId = userId,  // Assuming you want to associate the testimonial with the current user
                    TestimonialsText = testimonialText,
                    TestimonialsStatus = "Ignored"
                };

                // Add the testimonial to the database context and save changes
                _context.Testimonials.Add(testimonial);
                _context.SaveChanges();

                TempData["Message"] = "Your testimonial has been sent. Thank you!";
                return RedirectToAction("Testimonial");
            }

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}