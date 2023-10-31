using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.Data;
using BookStore.Util;
using BookStore.CustomAtrribute;
using Humanizer.Localisation.TimeToClockNotation;

namespace BookStore.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly BookstoreContext _context;

        public CustomerController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Customer
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AddressSortParam = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.GenderSortParam = sortOrder == "Gender" ? "gender_desc" : "Gender";
            ViewBag.DateOfBirthSortParam = sortOrder == "DateOfBirth" ? "dateofbirth_desc" : "DateOfBirth";
            ViewBag.UserIdSortParam = sortOrder == "UserId" ? "userid_desc" : "UserId";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var customers = from c in _context.Customers.Include(c => c.User)
                            select c;
            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.Name.Contains(searchString)
                                       || c.Address.Contains(searchString)
                                       || c.Gender.Contains(searchString)
                                       || c.DateOfBirth.ToString().Contains(searchString.ToString())
                                       || c.UserId.ToString().Contains(searchString.ToString())
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(m => m.Name);
                    break;
                case "Address":
                    customers = customers.OrderBy(m => m.Address);
                    break;
                case "address_desc":
                    customers = customers.OrderByDescending(m => m.Address);
                    break;
                case "Gender":
                    customers = customers.OrderBy(m => m.Gender);
                    break;
                case "gender_desc":
                    customers = customers.OrderByDescending(m => m.Gender);
                    break;
                case "DateOfBirth":
                    customers = customers.OrderBy(m => m.DateOfBirth);
                    break;
                case "dateofbirth_desc":
                    customers = customers.OrderByDescending(m => m.DateOfBirth);
                    break;
                case "UserId":
                    customers = customers.OrderBy(m => m.UserId);
                    break;
                case "userid_desc":
                    customers = customers.OrderByDescending(m => m.UserId);
                    break;
                default:
                    customers = customers.OrderBy(m => m.Name);
                    break;
            }
            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        // GET: Customer/Details/5
        [Route("Details/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("CustomerId,Address,Gender,DateOfBirth,Name,UserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = new SelectList(_context.Users, "UserId", "UserId", customer.UserId);
            return View("create", customer);
        }

        // GET: Customer/Edit/5
        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.UserId = new SelectList(_context.Users, "UserId", "UserId", customer.UserId);
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("CustomerId,Address,Gender,DateOfBirth,Name,UserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                ViewBag.UserId = new SelectList(_context.Users, "UserId", "UserId", customer.UserId);
            }
            return View("edit", customer);
        }

        // GET: Customer/Delete/5
        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        [Route("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed([Bind("CustomerId")] Customer customer)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'BookstoreContext.Customers'  is null.");
            }
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
