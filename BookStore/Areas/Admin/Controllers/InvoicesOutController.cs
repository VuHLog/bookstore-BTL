using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.CustomAtrribute;
using BookStore.Util;

namespace BookStore.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/InvoicesOut")]
    public class InvoicesOutController : Controller
    {
        private readonly BookstoreContext _context;

        public InvoicesOutController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admin/InvoicesOut
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoicesOutIdSortParam = String.IsNullOrEmpty(sortOrder) ? "invoicesoutid_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CustomerNameSortParam = sortOrder == "CustomerName" ? "customername_desc" : "CustomerName";
            ViewBag.EmployeeNameSortParam = sortOrder == "EmployeeName" ? "employeename_desc" : "EmployeeName";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var invoicesOuts = from i in _context.InvoicesOuts.Include(i => i.Customer).Include(i=> i.Employee)
                           select i;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                invoicesOuts = invoicesOuts.Where(i => i.InvoicesOutId.ToString().Contains(searchString)
                                                || i.Date.Value.ToString().Contains(searchString)
                                                || i.Employee.Name.Contains(searchString)
                                                || i.Customer.Name.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    invoicesOuts = invoicesOuts.OrderByDescending(m => m.InvoicesOutId);
                    break;
                case "Date":
                    invoicesOuts = invoicesOuts.OrderBy(m => m.Date);
                    break;
                case "date_desc":
                    invoicesOuts = invoicesOuts.OrderByDescending(m => m.Date);
                    break;
                case "CustomerName":
                    invoicesOuts = invoicesOuts.OrderBy(m => m.Customer.Name);
                    break;
                case "customername_desc":
                    invoicesOuts = invoicesOuts.OrderByDescending(m => m.Customer.Name);
                    break;
                case "EmployeeName":
                    invoicesOuts = invoicesOuts.OrderBy(m => m.Employee.Name);
                    break;
                case "employeename_desc":
                    invoicesOuts = invoicesOuts.OrderByDescending(m => m.Employee.Name);
                    break;
                default:
                    invoicesOuts = invoicesOuts.OrderBy(m => m.InvoicesOutId);
                    break;
            }
            return View(await PaginatedList<InvoicesOut>.CreateAsync(invoicesOuts.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        // GET: Admin/InvoicesOut/Details/5
        [Route("Details")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InvoicesOuts == null)
            {
                return NotFound();
            }

            var invoicesOut = await _context.InvoicesOuts
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoicesOutId == id);
            if (invoicesOut == null)
            {
                return NotFound();
            }

            return View(invoicesOut);
        }

        // GET: Admin/InvoicesOut/Create
        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
            return View();
        }

        // POST: Admin/InvoicesOut/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("InvoicesOutId,Date,CustomerId,EmployeeId")] InvoicesOut invoicesOut)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoicesOut);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", invoicesOut.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesOut.EmployeeId);
            return View("create",invoicesOut);
        }

        // GET: Admin/InvoicesOut/Edit/5
        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InvoicesOuts == null)
            {
                return NotFound();
            }

            var invoicesOut = await _context.InvoicesOuts.FindAsync(id);
            if (invoicesOut == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", invoicesOut.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesOut.EmployeeId);
            return View(invoicesOut);
        }

        // POST: Admin/InvoicesOut/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("InvoicesOutId,Date,CustomerId,EmployeeId")] InvoicesOut invoicesOut)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoicesOut);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoicesOutExists(invoicesOut.InvoicesOutId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", invoicesOut.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesOut.EmployeeId);
            return View("edit",invoicesOut);
        }

        // GET: Admin/InvoicesOut/Delete/5
        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InvoicesOuts == null)
            {
                return NotFound();
            }

            var invoicesOut = await _context.InvoicesOuts
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoicesOutId == id);
            if (invoicesOut == null)
            {
                return NotFound();
            }

            return View(invoicesOut);
        }

        // POST: Admin/InvoicesOut/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("InvoicesOutId")] InvoicesOut invoicesOut)
        {
            if (_context.InvoicesOuts == null)
            {
                return Problem("Entity set 'BookstoreContext.InvoicesOuts'  is null.");
            }
            if (invoicesOut != null)
            {
                _context.InvoicesOuts.Remove(invoicesOut);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoicesOutExists(int id)
        {
          return (_context.InvoicesOuts?.Any(e => e.InvoicesOutId == id)).GetValueOrDefault();
        }
    }
}
