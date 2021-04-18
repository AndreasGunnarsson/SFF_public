using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using test_SFF.Data;

namespace test_SFF.Controllers
{
    [Route("api/Studios")]
    [ApiController]
    public class NewStudioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewStudioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/Studios
        [HttpPost]
        public async Task<ActionResult<StudioDTO>> PostStudio(StudioDTO studioDTO)
        {
            bool isSame = false;
            isSame = _context.Studios.Any(x => x.Name == studioDTO.Name);
            if (isSame)
                return NotFound();

            var studio = new Studio
            {
                Name = studioDTO.Name,
                Location = studioDTO.Location
            };

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();
            // TODO: id fungerar inte; returnerar alltid 0.
            return CreatedAtAction("GetStudio", new { id = studio.Id }, studioDTO);
        }

        // DELETE: /api/Studios/#
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudio(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
                return NotFound();

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: /api/Studios/#
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudio(int id, StudioDTO studioDTO)
        {
            var studioBefore = await _context.Studios.FindAsync(id);

            if (studioBefore == null)
                return NotFound();

            studioBefore.Name = studioDTO.Name;
            studioBefore.Location = studioDTO.Location;

//          try
//          {
            if (!StudioExists(id))
            {
                return NotFound();
            }
            await _context.SaveChangesAsync();
/*          }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudioExists(id))
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

        // GET: /api/Studios/#
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<MovieName>>> GetMovieStudio(int id)
        {
            // Hämtar lista med alla filmer lånade filmer som har ett specifikt StudioId.
            var movieStudioQuery = _context.MovieStudios.Where<MovieStudio>(m => m.StudioId == id).ToList();
            if (movieStudioQuery.Count() == 0)
                return NotFound();
            
            // Hämtar alla filmer.
            var moviesQuery = await _context.Movies.ToListAsync();
            if (movieStudioQuery.Count() == 0)
                return NotFound();

            // Joinar de filmer som finns i movieStudioQuery med movieQuery så att man kan få ut namnen på dem.
            var joinedTables = (from ms in movieStudioQuery
                join movie in moviesQuery on ms.MovieId equals movie.Id
                select new MovieName { Name = movie.Name, PhysicalCopy = movie.PhysicalCopy, ReturnDate = ms.ReturnDate, Returned = ms.Returned}).ToList();

            return joinedTables;
        }

        // PUT: /api/Studios/MS/#
        [HttpPut("MS/{id:int}")]
        public async Task<IActionResult> PutRating(int id, MovieStudioRating movieStudioRating)
        {
            var movieStudioQuery = await _context.MovieStudios.FindAsync(id);
            if (movieStudioQuery == null)
                return NotFound();
            
            if (movieStudioQuery.Returned == false)
                return NotFound();

            movieStudioQuery.Score = movieStudioRating.Score;
            movieStudioQuery.Review = movieStudioRating.Review;

//            try
//            {
            await _context.SaveChangesAsync();
/*            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudioExists(id))
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

        private bool StudioExists(int id)
        {
            return _context.Studios.Any(e => e.Id == id);
        }

// ---------------------------------------

        // GET: api/NewStudio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Studio>>> GetStudios()
        {
            return await _context.Studios.ToListAsync();
        }

        // GET: api/NewStudio/5
/*        [HttpGet("{id}")]
        public async Task<ActionResult<Studio>> GetStudio(int id)
        {
            var studio = await _context.Studios.FindAsync(id);

            if (studio == null)
            {
                return NotFound();
            }

            return studio;
        } */
    }
}