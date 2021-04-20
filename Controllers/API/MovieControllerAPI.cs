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

        // POST: /api/Movies
        [HttpPost]
        public async Task<ActionResult<MovieDTO>> PostMovie(MovieDTO movieDTO)
        {
            bool isSame = false;
            isSame = _context.Movies.Any(x => x.Name == movieDTO.Name && x.PhysicalCopy == movieDTO.PhysicalCopy);
            if (isSame)
                return BadRequest();

            var movie = new Movie
            {
                Name = movieDTO.Name,
                TotalAmount = movieDTO.TotalAmount,
                PhysicalCopy = movieDTO.PhysicalCopy
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
        }

        // PUT: /api/Movies/#
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieAmount(int id, MovieDTO movieDTO)
        {
            var movieQuery = await _context.Movies.FindAsync(id);
            if (movieQuery == null)
                return NotFound();

            movieQuery.TotalAmount = movieDTO.TotalAmount;

            if (!MovieExists(id))
                return NotFound();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Movies/#
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
/*        [HttpDelete("{id}")]
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
        }*/
    }
}
