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
    [Route("Admin/InvoicesIn")]
    public class InvoicesInController : Controller
    {
        private readonly BookstoreContext _context;

        public InvoicesInController(BookstoreContext context)
        {
            _context = context;
        }


        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoicesInIdSortParam = String.IsNullOrEmpty(sortOrder) ? "invoicesinid_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PublisherNameSortParam = sortOrder == "PublisherName" ? "publishername_desc" : "PublisherName";
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

            var invoicesIns = from i in _context.InvoicesIns.Include(i => i.Publisher).Include(i => i.Employee)
                               select i;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                invoicesIns = invoicesIns.Where(i => i.InvoicesInId.ToString().Contains(searchString.ToString())
                                                || i.Date.Value.ToString().Contains(searchString.ToString())
                                                || i.Employee.Name.Contains(searchString)
                                                || i.Publisher.Name.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    invoicesIns = invoicesIns.OrderByDescending(m => m.InvoicesInId);
                    break;
                case "Date":
                    invoicesIns = invoicesIns.OrderBy(m => m.Date);
                    break;
                case "date_desc":
                    invoicesIns = invoicesIns.OrderByDescending(m => m.Date);
                    break;
                case "CustomerName":
                    invoicesIns = invoicesIns.OrderBy(m => m.Publisher.Name);
                    break;
                case "customername_desc":
                    invoicesIns = invoicesIns.OrderByDescending(m => m.Publisher.Name);
                    break;
                case "EmployeeName":
                    invoicesIns = invoicesIns.OrderBy(m => m.Employee.Name);
                    break;
                case "employeename_desc":
                    invoicesIns = invoicesIns.OrderByDescending(m => m.Employee.Name);
                    break;
                default:
                    invoicesIns = invoicesIns.OrderBy(m => m.InvoicesInId);
                    break;
            }
            return View(await PaginatedList<InvoicesIn>.CreateAsync(invoicesIns.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }


        [Route("Details")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InvoicesOuts == null)
            {
                return NotFound();
            }

            var invoicesIn = await _context.InvoicesIns
                .Include(i => i.Publisher)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoicesInId == id);
            if (invoicesIn == null)
            {
                return NotFound();
            }

            return View(invoicesIn);
        }


        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
            return View();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("InvoicesInId,Date,PublisherId,EmployeeId")] InvoicesIn invoicesIn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoicesIn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Customers, "PublisherId", "Name", invoicesIn.PublisherId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesIn.EmployeeId);
            return View("create", invoicesIn);
        }


        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InvoicesIns == null)
            {
                return NotFound();
            }

            var invoicesIn = await _context.InvoicesIns.FindAsync(id);
            if (invoicesIn == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "Name", invoicesIn.PublisherId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesIn.EmployeeId);
            return View(invoicesIn);
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("InvoicesInId,Date,PublisherId,EmployeeId")] InvoicesIn invoicesIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoicesIn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoicesInExists(invoicesIn.InvoicesInId))
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "CustomerId", "Name", invoicesIn.PublisherId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesIn.EmployeeId);
            ViewData["PublisherId"] = new SelectList(_context.Customers, "PublisherId", "Name", invoicesIn.PublisherId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", invoicesIn.EmployeeId);
            return View("edit", invoicesIn);
        }


        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InvoicesIns == null)
            {
                return NotFound();
            }

            var invoicesIn = await _context.InvoicesIns
                .Include(i => i.Publisher)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoicesInId == id);
            if (invoicesIn == null)
            {
                return NotFound();
            }

            return View(invoicesIn);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("InvoicesInId")] InvoicesIn invoicesIn)
        {
            if (_context.InvoicesIns == null)
            {
                return Problem("Entity set 'BookstoreContext.InvoicesIns'  is null.");
            }
            if (invoicesIn != null)
            {
                _context.InvoicesIns.Remove(invoicesIn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoicesInExists(int id)
        {
            return (_context.InvoicesIns?.Any(e => e.InvoicesInId == id)).GetValueOrDefault();
        }
    }
}
