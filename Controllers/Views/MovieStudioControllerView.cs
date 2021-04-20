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
    public class MovieStudioControllerView : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieStudioControllerView(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int id)
        {
            Movie movieQuery = await _context.Movies.FindAsync(id);

            if (movieQuery == null)
                return NotFound();

            ViewData["Movie"] = movieQuery;
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name");
            return View();
        }

        // POST: MovieStudioControllerView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudioId, ReturnDate")] MovieStudio movieStudio, int id)
        {
            // Finns tillräckligt många kopior för att låna ut en film?
            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == movieStudio.MovieId).Count();
            Movie movieQuery = await _context.Movies.FindAsync(id);
            if (movieQuery == null)
                return NotFound();
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return BadRequest();

            // Har samma studio lånat samma film en gång tidigare?
            bool isNotDuplicates = false;
            isNotDuplicates = _context.MovieStudios.Any(x => x.MovieId == id && x.StudioId == movieStudio.StudioId);
            if (isNotDuplicates == true)
                return BadRequest();

            // Är datumet efter dagens datum?
            if (movieStudio.ReturnDate <= DateTime.Now)
                return BadRequest();

            MovieStudio moviestudioobject = new MovieStudio
            {
                MovieId = id,
                StudioId = movieStudio.StudioId,
                ReturnDate = movieStudio.ReturnDate,
                Returned = false,
                Score = 0
            };

            if (ModelState.IsValid)
            {
                _context.Add(moviestudioobject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "MoviesControllerView");
            }

            ViewData["Movie"] = movieQuery;
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name");
            return View();
        }

        // GET: MovieStudioControllerView/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            MovieStudio movieStudioQuery = await _context.MovieStudios.FindAsync(id);
            if (movieStudioQuery == null)
                return NotFound();

            ViewData["Movie"] = await _context.Movies.FindAsync(movieStudioQuery.MovieId);
            ViewData["Studio"] = await _context.Studios.FindAsync(movieStudioQuery.StudioId);

            return View();
        }

        // POST: MovieStudioControllerView/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id")] MovieStudio movieStudio)
        {
            MovieStudio movieStudioQuery = await _context.MovieStudios.FindAsync(movieStudio.Id);
            if (movieStudioQuery == null)
                return NotFound();

            movieStudioQuery.Returned = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "StudiosControllerView", new { id = movieStudioQuery.StudioId });
        }


// ------------------------------------------------------ Not needed:

        // GET: MovieStudioControllerView
/*        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MovieStudios.Include(m => m.Movie).Include(m => m.Studio);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MovieStudioControllerView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieStudio = await _context.MovieStudios
                .Include(m => m.Movie)
                .Include(m => m.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieStudio == null)
            {
                return NotFound();
            }

            return View(movieStudio);
        }

        // GET: MovieStudioControllerView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieStudio = await _context.MovieStudios
                .Include(m => m.Movie)
                .Include(m => m.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieStudio == null)
            {
                return NotFound();
            }

            return View(movieStudio);
        }

        // POST: MovieStudioControllerView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieStudio = await _context.MovieStudios.FindAsync(id);
            _context.MovieStudios.Remove(movieStudio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieStudioExists(int id)
        {
            return _context.MovieStudios.Any(e => e.Id == id);
        }*/
    }
}
