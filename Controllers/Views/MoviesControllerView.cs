using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using test_SFF.Data;
using Microsoft.AspNetCore.Authorization;

namespace test_SFF.Controllers
{
    [Authorize]
    public class MoviesControllerView : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesControllerView(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /MoviesControllerView
        public async Task<IActionResult> Index()
        {
            var movieQuery = await _context.Movies.ToListAsync();

            // Count på alla filmer:
            var movieStudioQuery =
                _context.MovieStudios
                .GroupBy(x => x.MovieId)
                .Select(y => new { MovieId = y.Key, BorrowedAmount = y.Count() })
                .ToList();

            var joinedTables = (from movie in movieQuery
                join ms in movieStudioQuery on movie.Id equals ms.MovieId into newtable
                from variablename in newtable.DefaultIfEmpty()
                select new MovieAvailableAmount {
                    Id = movie.Id,
                    Name = movie.Name,
                    PhysicalCopy = movie.PhysicalCopy,
                    TotalAmount = movie.TotalAmount,
                    BorrowedAmount = variablename?.BorrowedAmount ?? 0
                }
            ).ToList();

            return View(joinedTables);
        }

        // GET: /MoviesControllerView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: /MoviesControllerView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /MoviesControllerView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, TotalAmount, PhysicalCopy")] Movie movie)
        {
            bool isSame = false;
            isSame = _context.Movies.Any(x => x.Name == movie.Name && x.PhysicalCopy == movie.PhysicalCopy);

            if (isSame)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        // GET: MoviesControllerView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: MoviesControllerView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

// ---------------------------------------------------------------------------------- Används ej:
        // GET: MoviesControllerView/Edit/5
/*        public async Task<IActionResult> Edit(int? id)
        {
            // TODO: Måste kolla ifall namnet redan existerar och physicalcopy.
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: /MoviesControllerView/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TotalAmount,PhysicalCopy")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }*/
    }
}
