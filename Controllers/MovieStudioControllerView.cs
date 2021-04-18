using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using test_SFF.Data;

namespace test_SFF.Controllers
{
    public class MovieStudioControllerView : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieStudioControllerView(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MovieStudioControllerView
        public async Task<IActionResult> Index()
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

        // GET: MovieStudioControllerView/Create
/*        public IActionResult Create()           // När man klickar på "Create New" i index kommer man hit.
        {
            // TODO: Returned ska inte gå att kryssa i här; görs i bakgrunden.
            // TODO: Hade varit snyggt om man kunde skriva in datum på ett snyggare sätt.
                // Man ska kunna skriva in ett datum som filmen måste vara inlämnad på. Måste vara efter dagens datum.
            //ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Name");
            // TODO: Om man vill ha en snyggare lista (t.ex. för att visa physical och inte samt filmer som inte går att låna) måste man nog göra en egen list med "SelectListItem".
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");      // Name i sista istället för Id.
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Id");
            return View();
        }*/

        public async Task<IActionResult> Create(int id)           // När man klickar på "Create New" i index kommer man hit.
        {
            // TODO: Returned ska inte gå att kryssa i här; görs i bakgrunden.
            // TODO: Hade varit snyggt om man kunde skriva in datum på ett snyggare sätt.
                // Man ska kunna skriva in ett datum som filmen måste vara inlämnad på. Måste vara efter dagens datum.
            //ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Name");
            // TODO: Om man vill ha en snyggare lista (t.ex. för att visa physical och inte samt filmer som inte går att låna) måste man nog göra en egen list med "SelectListItem".
                // Tror också att man hade kunnat fixa till det snyggare med (int? id)
//            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", new {MovieId = id});      // Name i sista istället för Id.

            Movie movieQuery = await _context.Movies.FindAsync(id);

            if(movieQuery == null)
                return NotFound();          // TODO: Snyggare felmeddelande

            ViewData["Movie"] = movieQuery;
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Id");        // TODO: Name i sista istället för Id.
            return View();
        }

        /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id)
        {
            // Skapa eget objekt
            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == movieStudio.MovieId).Count();
            Movie movieQuery = await _context.Movies.FindAsync(movieStudio.MovieId);
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return NotFound();

            bool isNotDuplicates = false;
            isNotDuplicates = _context.MovieStudios.Any(x => x.MovieId == movieStudio.MovieId && x.StudioId == movieStudio.StudioId);
            if (isNotDuplicates == true)
                return NotFound();
                
            if (ModelState.IsValid)
            {
                _context.Add(movieStudio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Name", movieStudio.MovieId);       // TODO: Vet inte ifall dessa fungerar alls.
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", movieStudio.StudioId);
            return View(movieStudio);
        }*/

        // POST: MovieStudioControllerView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudioId, ReturnDate")] MovieStudio movieStudio, int id)
        {
            // TODO: Kolla så att all logik här fungerar!
            // Kollar ifall det finns tillräckligt många kopior för att låna ut en film. Jämför antalet utlånade i MoviStudios med Movies.TotalAmount.
/*            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == id).Count();
            Movie movieQuery = await _context.Movies.FindAsync(id);
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return NotFound();*/
                // TODO: Snyggare felmeddelande behövs här! Kanske skriver ut direkt på create-sidan?
//              return Redirect("../Views/Shared/Error.cshtml");

            // Kollar så att inte MovieId och StudioId finns på samma rad i tabellen MovieStudios. För att motverka att samma studio lånar samma film.
/*            bool isNotDuplicates = false;
            isNotDuplicates = _context.MovieStudios.Any(x => x.MovieId == id && x.StudioId == movieStudio.StudioId);
            if (isNotDuplicates == true)
                return NotFound();*/

            // TODO: Kolla så att ReturnDate är efter dagens datum.
            if (movieStudio.ReturnDate <= DateTime.Now)
                return NotFound();

            /* MovieStudio moviestudio = new MovieStudio
            {
                MovieId = moviestudioDTO.MovieId,
                StudioId = moviestudioDTO.StudioId,
                ReturnDate = moviestudioDTO.ReturnDate,
                Returned = false
            }; */

            MovieStudio fisen = new MovieStudio
            {
                MovieId = id,
                StudioId = movieStudio.StudioId,
                ReturnDate = movieStudio.ReturnDate
            };

            if (ModelState.IsValid)
            {
//              _context.Add(movieStudio);
                _context.Add(fisen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Name", movieStudio.MovieId);       // TODO: Vet inte ifall dessa fungerar alls.
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", movieStudio.StudioId);
            return View(movieStudio);
        }

        // GET: MovieStudioControllerView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
/*            if (id == null && StudioId == null)
            {
                return NotFound();
            }*/

            var movieStudio = await _context.MovieStudios.FindAsync(id);
            if (movieStudio == null)
            {
                return NotFound();
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieStudio.MovieId);
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Id", movieStudio.StudioId);
            return View(movieStudio);
        }

        // GET: MovieStudioControllerView/Edit/5
/*        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieStudio = await _context.MovieStudios.FindAsync(id);
            if (movieStudio == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieStudio.MovieId);
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Id", movieStudio.StudioId);
            return View(movieStudio);
        } */

        // POST: MovieStudioControllerView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id, Returned")] MovieStudio movieStudio)
        {
            MovieStudio movieStudioQuery = await _context.MovieStudios.FindAsync(movieStudio.Id);
            if (movieStudioQuery == null)
                return NotFound();
            //if (movieBefore == null)
            //    return NotFound();

            movieStudioQuery.Returned = movieStudio.Returned;
            //movieBefore.TotalAmount = movieDTO.TotalAmount;
            // TOD: Måste man hantera vad som händer med filmer som redan är utlånade?
                // T.ex. ifall man lånat ut totalt 10 och man sedan sätter TotalAmount till 6?
                    // När filmerna sedan är returnerade kommer ..
                        // Tror itne detta är ett problem.
//            try
//            {
                await _context.SaveChangesAsync();
/*            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(1))      // TODO: Denna gör inget vettigs; vi borde kolla MovieStudio.
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

//            return NoContent();
// -------------------------------------------
/*            if (id != movieStudio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieStudio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieStudioExists(movieStudio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            } */
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieStudio.MovieId);
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Id", movieStudio.StudioId);
            return View(movieStudio);
        }





// ------------------------------------------------------ Not needed:

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
        }
    }
}
