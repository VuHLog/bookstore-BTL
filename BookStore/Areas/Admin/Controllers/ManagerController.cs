using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.CustomAtrribute;
using BookStore.Data;
using BookStore.Util;
using System.Drawing.Printing;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/manager")]
    public class ManagerController : Controller
    {
        private readonly BookstoreContext _context;

        public ManagerController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Manager
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder,string searchString, string currentFilter, int? pageNumber,int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AddressSortParam = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.SalarySortParam = sortOrder == "Salary" ? "salary_desc" : "Salary";
            ViewBag.BookshelfNameSortParam = sortOrder == "BookshelfName" ? "bookshelfname_desc" : "BookshelfName";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var managers =from m in _context.Managers.Include(m => m.Bookshelf)
                                  select m;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize==null?5:pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                managers = managers.Where(m => m.Name.Contains(searchString)
                                       || m.Address.Contains(searchString)
                                       || m.Salary.ToString().Contains(searchString.ToString())
                                       || m.Bookshelf.Name.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    managers = managers.OrderByDescending(m => m.Name);
                    break;
                case "Address":
                    managers = managers.OrderBy(m => m.Address);
                    break;
                case "address_desc":
                    managers = managers.OrderByDescending(m => m.Address);
                    break;
                case "Salary":
                    managers = managers.OrderBy(m => m.Salary);
                    break;
                case "salary_desc":
                    managers = managers.OrderByDescending(m => m.Salary);
                    break;
                case "BookshelfName":
                    managers = managers.OrderBy(m => m.Bookshelf.Name);
                    break;
                case "bookshelfname_desc":
                    managers = managers.OrderByDescending(m => m.Bookshelf.Name);
                    break;
                default:
                    managers = managers.OrderBy(m => m.Name);
                    break;
            }
            return View(await PaginatedList<Manager>.CreateAsync(managers.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        // GET: Manager/Details/5
        [Route("Details")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.Bookshelf)
                .FirstOrDefaultAsync(m => m.ManagerId == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // GET: Manager/Create
        [Route("create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["BookshelfId"] = new SelectList(_context.Bookshelves, "BookshelfId", "BookshelfId");
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("Address,Name,Salary,BookshelfId")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookshelfId"] = new SelectList(_context.Bookshelves, "BookshelfId", "BookshelfId", manager.BookshelfId);
            return View("create", manager);
        }

        // GET: Manager/Edit/5
        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }
            ViewData["BookshelfId"] = new SelectList(_context.Bookshelves, "BookshelfId", "BookshelfId", manager.BookshelfId);
            return View(manager);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("ManagerId,Address,Name,Salary,BookshelfId")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagerExists(manager.ManagerId))
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
            ViewData["BookshelfId"] = new SelectList(_context.Bookshelves, "BookshelfId", "BookshelfId", manager.BookshelfId);
            return View("edit", manager);
        }

        // GET: Manager/Delete/5
        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.Bookshelf)
                .FirstOrDefaultAsync(m => m.ManagerId == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // POST: Manager/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("ManagerId")] Manager manager)
        {
            if (_context.Managers == null)
            {
                return Problem("Entity set 'BookstoreContext.Managers'  is null.");
            }
            if (manager != null)
            {
                var sql = "";
                sql = "update employee set manager_id=NULL where manager_id = {0}";
                await _context.Database.ExecuteSqlRawAsync(sql, manager.ManagerId);
                _context.Managers.Remove(manager);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
            return (_context.Managers?.Any(e => e.ManagerId == id)).GetValueOrDefault();
        }
    }
}
