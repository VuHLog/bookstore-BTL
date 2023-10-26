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

namespace BookStore.Admin.Controllers
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
        public async Task<IActionResult> Index()
        {
            var bookstoreContext = _context.Managers.Include(m => m.Bookshelf);
            return View(await bookstoreContext.ToListAsync());
        }

        // GET: Manager/Details/5
        [Route("Details")]
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
        public async Task<IActionResult> DeleteConfirmed([Bind("ManagerId")] Manager manager)
        {
            if (_context.Managers == null)
            {
                return Problem("Entity set 'BookstoreContext.Managers'  is null.");
            }
            if (manager != null)
            {
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
