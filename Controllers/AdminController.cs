using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Drawing;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;

        public readonly IWebHostEnvironment _webHostEnvironment; //declare variable

        private readonly EmailService _emailService;

        public AdminController(ModelContext context, IWebHostEnvironment webHostEnvironment, EmailService emailService) //declare IWebHostEnvironment Parameter
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; //Dependency Injection
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
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
							ViewBag.users = _context.Customers.ToList();
                            ViewBag.orders = _context.ProductCustomers.ToList();
                            ViewBag.reviews = _context.Reviews.ToList();
                            ViewBag.minPrice = _context.Products.Min(p => p.Price);
                            ViewBag.maxPrice = _context.Products.Max(p => p.Price);
                            ViewBag.products = _context.Products.ToList();
                            ViewBag.coupons = _context.Coupons.ToList();
                            ViewBag.testimonials = _context.Testimonials.ToList();

                            var totalOrders = _context.ProductCustomers.Count();
                            var deliveredOrders = _context.ProductCustomers.Count(pc => pc.OrderStatus == "DELIVERED");

                            int percentage = (deliveredOrders * 100) / totalOrders;

                            ViewBag.NumberOfDeliveredOrders = percentage;

                            ViewBag.totalOrders = totalOrders;
                            ViewBag.deliveredOrders = deliveredOrders;

                            var product_customer = _context.ProductCustomers.ToList();
                            var addresses = _context.Addresses.ToList();

                            var multiTable1 = from pc in product_customer
                                              join a in addresses on pc.AddressId equals a.Id
                                              select new
                                              {
                                                  City = a.City,
                                                  OrderId = pc.Id
                                              };
                            ViewBag.reviewsAvg = _context.Reviews.Average(review => review.Stars);

                            var cityOrderCounts = multiTable1
                                .GroupBy(item => item.City)
                                .Select(group => new
                                {
                                    City = group.Key,
                                    OrderCount = group.Count()
                                })
                                .ToList();

                            var cityLabels = cityOrderCounts.Select(item => item.City).ToArray();
                            var orderCounts = cityOrderCounts.Select(item => item.OrderCount).ToArray();

                            ViewData["CityLabels"] = cityLabels;
                            ViewData["OrderCounts"] = orderCounts;

                            var orderStatusCounts = _context.ProductCustomers
                                .GroupBy(pc => pc.OrderStatus)
                                .Select(group => new
                                {
                                    OrderStatus = group.Key,
                                    OrderCount = group.Count()
                                })
                                .ToList();

                            var statusLabels = orderStatusCounts.Select(item => item.OrderStatus).ToArray();
                            var orderCountsByStatus = orderStatusCounts.Select(item => item.OrderCount).ToArray();

                            ViewData["StatusLabels"] = statusLabels;
                            ViewData["OrderCountsByStatus"] = orderCountsByStatus;
                        }
                    }
                }
                return View();
            }
            else
            {
                var adminName = HttpContext.Session.GetString("AdminName");
                
                if (!string.IsNullOrEmpty(adminName))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }


        }



        [HttpGet]
        public IActionResult Report()
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
						if (user.RoleId == 1)
                        {
                            var ordersInfo = _context.OrderInfos.ToList();
                            var customer = _context.Customers.ToList();
                            var product_customer = _context.ProductCustomers.ToList();
                            var addrsses = _context.Addresses.ToList();
                            var products = _context.Products.ToList();
                            var multiTable = from p in products
                                             join oi in ordersInfo on p.Id equals oi.ProductId
                                             select new
                                             {
                                                 OrderInfo = oi,
                                                 Products = p
                                             };

                            var ordersJoinList = multiTable.Select(result => new OrdersJoin(_context)
                            {
                                OrderInfo = new List<OrderInfo> { result.OrderInfo },
                                Products = new List<Product>(),
                            }).ToList();

                            var multiTable1 = from pc in product_customer
                                              join a in addrsses on pc.AddressId equals a.Id
                                              select new OrdersJoin(_context)
                                              {
                                                  Order = pc,
                                                  Address = a
                                              };



                            var ordersDetails = Tuple.Create<IEnumerable<OrdersJoin>, IEnumerable<OrdersJoin>>(ordersJoinList, multiTable1);
                            return View(ordersDetails);
                        }
                    }
                }
                return View();
            }
            else
            {
                var adminName = HttpContext.Session.GetString("AdminName");
                
                if (!string.IsNullOrEmpty(adminName))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
        }


            [HttpPost]
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate, DateTime? startDate1, DateTime? endDate1)
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
                        if (user.RoleId == 1)
                        {
                           
                            var ordersInfo = _context.OrderInfos.ToList();
                            var customer = _context.Customers.ToList();
                            var product_customer = _context.ProductCustomers.ToList();
                            var addrsses = _context.Addresses.ToList();
                            var products = _context.Products.ToList();

                            //-----------------------------------------------
                            var multiTable1 = from pc in product_customer
                                              join a in addrsses on pc.AddressId equals a.Id
                                              select new OrdersJoin(_context)
                                              {
                                                  Order = pc,
                                                  Address = a
                                              };


                            var modelContext = multiTable1.ToList();

                            if (startDate != null && endDate != null)
                            {
                                modelContext = modelContext.Where(x =>
                                    x.Order.OrderDate.Value.Date >= startDate && x.Order.OrderDate.Value.Date <= endDate)
                                    .ToList();
                            }
                            else if (startDate == null && endDate != null)
                            {
                                modelContext = modelContext.Where(x =>
                                    x.Order.OrderDate.Value.Date <= endDate)
                                    .ToList();
                            }
                            else if (startDate != null && endDate == null )
                            {
                                modelContext = modelContext.Where(x =>
                                   (x.Order.OrderDate.Value.Date >= startDate ))
                                    .ToList();
                            }

                            //--------------------------------------------------------------------

                            var multiTable = from p in products
                                             join oi in ordersInfo on p.Id equals oi.ProductId
                                             select new
                                             {
                                                 OrderInfo = oi,
                                                 Products = p
                                             };

                            var modelContext1 = multiTable.Select(result => new OrdersJoin(_context)
                            {
                                OrderInfo = new List<OrderInfo> { result.OrderInfo },
                                Products = new List<Product>(),
                            }).ToList();

                            var filteredContext1 = modelContext1.ToList(); // Create a new collection for filtering

                            if (startDate1 != null && endDate1 != null)
                            {
                                filteredContext1 = filteredContext1
                                    .Where(x =>
                                        x.OrderInfo.Any(oi =>
                                            oi.Order.OrderDate.Value.Date >= startDate1 && oi.Order.OrderDate.Value.Date <= endDate1))
                                    .ToList();
                            }
                            else if (startDate1 == null && endDate1 != null)
                            {
                                filteredContext1 = filteredContext1
                                    .Where(x =>
                                        x.OrderInfo.Any(oi => oi.Order.OrderDate.Value.Date <= endDate1))
                                    .ToList();
                            }
                            else if (startDate1 != null && endDate1 == null)
                            {
                                filteredContext1 = filteredContext1
                                    .Where(x =>
                                        x.OrderInfo.Any(oi =>
                                            oi.Order.OrderDate.Value.Date >= startDate1 ))
                                    .ToList();
                            }

                            // Now, use the filteredContext1 for further processing or displ

                            var ordersDetails = Tuple.Create<IEnumerable<OrdersJoin>, IEnumerable<OrdersJoin>>(filteredContext1, modelContext);
                            return View(ordersDetails);

                        }
                    }
                }
                return View();
            }
            else
            {
                var adminName = HttpContext.Session.GetString("AdminName");
                
                if (!string.IsNullOrEmpty(adminName))
                {
                    HttpContext.Session.Remove("AdminName");
                }

                HttpContext.SignOutAsync("MyCustomAuthScheme");
                return RedirectToAction("Login", "LoginAndRegister");
            }
        }

    }
}

