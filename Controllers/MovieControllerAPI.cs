using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using test_SFF.Data;
using Microsoft.AspNetCore.Authorization;

namespace test_SFF.Controllers
{
    [Route("api/Movies")]
    [ApiController]
    public class NewMovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewMovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<MovieDTO>> PostMovie(MovieDTO movieDTO)
        {
            bool isSame = false;
            isSame = _context.Movies.Any(x => x.Name == movieDTO.Name && x.PhysicalCopy == movieDTO.PhysicalCopy);
            if (isSame)
                return NotFound();

            var movie = new Movie
            {
                Name = movieDTO.Name,
                TotalAmount = movieDTO.TotalAmount,
                PhysicalCopy = movieDTO.PhysicalCopy
            };
            
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            // TODO: Returnerar inte rätt Id (det nya).
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
        }

        // PUT: api/Movies/#
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieAmount(int id, MovieDTO movieDTO)
        {
            var movieQuery = await _context.Movies.FindAsync(id);
            if (movieQuery == null)
                return NotFound();

            movieQuery.TotalAmount = movieDTO.TotalAmount;

//          try
//          {
            if (!MovieExists(id))
            {
                return NotFound();
            }
            await _context.SaveChangesAsync();
/*            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            } */

            return NoContent();
        }

        // TODO: Ful route:
        // POST: /api/Movies/MS
        [Route("MS")]
        [HttpPost]
        public async Task<ActionResult<MovieStudioDTO>> PostMovieBorrow(MovieStudioDTO movieStudioDTO)
        {
            // Kollar ifall det finns tillräckligt många kopior för att låna ut en film. Jämför antalet utlånade i MovieStudios med Movies.TotalAmount.
            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == movieStudioDTO.MovieId).Count();
            Movie movieQuery = await _context.Movies.FindAsync(movieStudioDTO.MovieId);
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return NotFound();

            // Kollar så att inte MovieId och StudioId finns på samma rad i tabellen MovieStudios. För att motverka att samma studio lånar samma film.
            bool isNotDuplicates = false;
            isNotDuplicates = _context.MovieStudios.Any(x => x.MovieId == movieStudioDTO.MovieId && x.StudioId == movieStudioDTO.StudioId);
            if (isNotDuplicates == true)
                return NotFound();

            // Datumet kan inte vara under eller lika med dagens datum.
            if (movieStudioDTO.ReturnDate <= DateTime.Now)
                return NotFound();

            MovieStudio movieStudio = new MovieStudio
            {
                MovieId = movieStudioDTO.MovieId,
                StudioId = movieStudioDTO.StudioId,
                ReturnDate = movieStudioDTO.ReturnDate,
                Returned = false
            };
            
            if (!MovieExists(movieStudio.MovieId) || !StudioExists(movieStudio.StudioId))
                return NotFound();
            else
            {
                _context.MovieStudios.Add(movieStudio);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovieStudio", new { id = movieStudioDTO.Id }, movieStudioDTO);
            }
        }

        // PUT: /api/Movies/MS2/#
        [HttpPut("MS2/{id:int}")]
        public async Task<IActionResult> PutReturnMovie(int id)
        {
            MovieStudio movieStudioQuery = await _context.MovieStudios.FindAsync(id);
            if (movieStudioQuery == null)
                return NotFound();

            movieStudioQuery.Returned = true;

//          try
//          {
            if (!_context.MovieStudios.Any(e => e.Id == id))
            {
                return NotFound();
            }
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
            } */

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        private bool StudioExists(int id)
        {
            return _context.Studios.Any(e => e.Id == id);
        }

// ----------------------------------------------------------- Actions som inte behövs:

        // DELETE: api/NewMovie/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/NewMovie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/NewMovie/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }
    }
}
