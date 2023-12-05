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
    public class OrderInfoesController : Controller
    {
        private readonly ModelContext _context;

        public OrderInfoesController(ModelContext context)
        {
            _context = context;
        }

        // GET: OrderInfoes
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.OrderInfos.Include(o => o.Customer).Include(o => o.Order).Include(o => o.Product);
            return View(await modelContext.ToListAsync());
        }

        // GET: OrderInfoes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.OrderInfos == null)
            {
                return NotFound();
            }

            var orderInfo = await _context.OrderInfos
                .Include(o => o.Customer)
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderInfo == null)
            {
                return NotFound();
            }

            return View(orderInfo);
        }

        // GET: OrderInfoes/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: OrderInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,CustomerId,ProductId,TotalPrice,Quantity")] OrderInfo orderInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderInfo.CustomerId);
            ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id", orderInfo.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderInfo.ProductId);
            return View(orderInfo);
        }

        // GET: OrderInfoes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.OrderInfos == null)
            {
                return NotFound();
            }

            var orderInfo = await _context.OrderInfos.FindAsync(id);
            if (orderInfo == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderInfo.CustomerId);
            ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id", orderInfo.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderInfo.ProductId);
            return View(orderInfo);
        }

        // POST: OrderInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,OrderId,CustomerId,ProductId,TotalPrice,Quantity")] OrderInfo orderInfo)
        {
            if (id != orderInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderInfoExists(orderInfo.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderInfo.CustomerId);
            ViewData["OrderId"] = new SelectList(_context.ProductCustomers, "Id", "Id", orderInfo.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderInfo.ProductId);
            return View(orderInfo);
        }

        // GET: OrderInfoes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.OrderInfos == null)
            {
                return NotFound();
            }

            var orderInfo = await _context.OrderInfos
                .Include(o => o.Customer)
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderInfo == null)
            {
                return NotFound();
            }

            return View(orderInfo);
        }

        // POST: OrderInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.OrderInfos == null)
            {
                return Problem("Entity set 'ModelContext.OrderInfos'  is null.");
            }
            var orderInfo = await _context.OrderInfos.FindAsync(id);
            if (orderInfo != null)
            {
                _context.OrderInfos.Remove(orderInfo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderInfoExists(decimal id)
        {
          return (_context.OrderInfos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
