using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using System.Web;
using test_SFF.Data;
using Microsoft.AspNetCore.Authorization;

namespace test_SFF.Controllers
{
    [Authorize]
    public class StudiosControllerView : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudiosControllerView(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudiosControllerView
        public async Task<IActionResult> Index()
        {
            return View(await _context.Studios.ToListAsync());
        }

        // GET: StudiosControllerView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Hämtar lista med alla filmer lånade filmer som har ett specifikt StudioId.
            var movieStudioQuery = _context.MovieStudios.Where<MovieStudio>(m => m.StudioId == id).ToList();

            // Hämtar den studio som efterfrågas i URL:en.
            Studio studio = await _context.Studios.FirstOrDefaultAsync(m => m.Id == id);
            if (studio == null)
                return NotFound();

            // Hämtar alla filmer.
            var moviesQuery = await _context.Movies.ToListAsync();

            // Joinar de filmer som finns i moviestudio med movieQuery så att man kan få ut namnen på dem.
            var joinedTables = (from ms in movieStudioQuery
                join movie in moviesQuery on ms.MovieId equals movie.Id
                select new MovieName {
                    MovieStudioId = ms.Id,
                    Name = movie.Name,
                    PhysicalCopy = movie.PhysicalCopy,
                    ReturnDate = ms.ReturnDate,
                    Returned = ms.Returned
                }
            ).ToList();

            MovieStudioDetails collectionObject = new MovieStudioDetails { Studio = studio, JoinedList = joinedTables };

            return View(collectionObject);
        }

        // GET: StudiosControllerView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudiosControllerView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Location")] Studio studio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studio);
        }

        // GET: StudiosControllerView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _context.Studios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: StudiosControllerView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

// --------------------------------------------------------------- Används ej:
        // GET: StudiosControllerView/Edit/5
/*        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }
            return View(studio);
        }

        // POST: StudiosControllerView/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Studio studio)
        {
            if (id != studio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudioExists(studio.Id))
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
            return View(studio);
        } */
    }
}
