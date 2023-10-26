using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class KindOfBookController : Controller
    {
        private readonly BookstoreContext _context;

        public KindOfBookController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: KindOfBook
        [Route("admin/kindOfBook")]
        public async Task<IActionResult> Index()
        {
              return _context.KindOfBooks != null ? 
                          View(await _context.KindOfBooks.ToListAsync()) :
                          Problem("Entity set 'BookstoreContext.KindOfBooks'  is null.");
        }

        // GET: KindOfBook/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks
                .FirstOrDefaultAsync(m => m.KindOfBookId == id);
            if (kindOfBook == null)
            {
                return NotFound();
            }

            return View(kindOfBook);
        }

        // GET: KindOfBook/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KindOfBook/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("KindOfBookId,Name")] KindOfBook kindOfBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kindOfBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("create",kindOfBook);
        }

        // GET: KindOfBook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks.FindAsync(id);
            if (kindOfBook == null)
            {
                return NotFound();
            }
            return View(kindOfBook);
        }

        // POST: KindOfBook/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost([Bind("KindOfBookId,Name")] KindOfBook kindOfBook)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kindOfBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KindOfBookExists(kindOfBook.KindOfBookId))
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
            return View("edit",kindOfBook);
        }

        // GET: KindOfBook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks
                .FirstOrDefaultAsync(m => m.KindOfBookId == id);
            if (kindOfBook == null)
            {
                return NotFound();
            }

            return View(kindOfBook);
        }

        // POST: KindOfBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KindOfBooks == null)
            {
                return Problem("Entity set 'BookstoreContext.KindOfBooks'  is null.");
            }
            var kindOfBook = await _context.KindOfBooks.FindAsync(id);
            if (kindOfBook != null)
            {
                _context.KindOfBooks.Remove(kindOfBook);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KindOfBookExists(int id)
        {
          return (_context.KindOfBooks?.Any(e => e.KindOfBookId == id)).GetValueOrDefault();
        }
    }
}
