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
                return BadRequest();

            var studio = new Studio
            {
                Name = studioDTO.Name,
                Location = studioDTO.Location
            };

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();

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

            if (!StudioExists(id))
                return NotFound();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: /api/Studios/Borrowed/#
        [HttpGet("Borrowed/{id:int}")]
        public async Task<ActionResult<IEnumerable<MovieName>>> GetMovieStudio(int id)
        {
            // H??mtar lista med alla filmer l??nade filmer som har ett specifikt StudioId.
            var movieStudioQuery = await _context.MovieStudios.Where<MovieStudio>(m => m.StudioId == id).ToListAsync();
            if (movieStudioQuery.Count() == 0)
                return BadRequest();
            
            // H??mtar alla filmer.
            var moviesQuery = await _context.Movies.ToListAsync();
            if (movieStudioQuery.Count() == 0)
                return NotFound();

            // Joinar de filmer som finns i movieStudioQuery med movieQuery s?? att man kan f?? ut namn p?? filmerna.
            var joinedTables = (from ms in movieStudioQuery
                join movie in moviesQuery on ms.MovieId equals movie.Id
                select new MovieName {
                    Name = movie.Name,
                    PhysicalCopy = movie.PhysicalCopy,
                    ReturnDate = ms.ReturnDate,
                    Returned = ms.Returned
                }
            ).ToList();

            return joinedTables;
        }

        // GET: /api/Studios/#
        [HttpGet("{id}")]
        public async Task<ActionResult<Studio>> GetStudio(int id)
        {
            var studio = await _context.Studios.FindAsync(id);

            if (studio == null)
            {
                return NotFound();
            }

            return studio;
        }

        private bool StudioExists(int id)
        {
            return _context.Studios.Any(e => e.Id == id);
        }

// ---------------------------------------

        // GET: api/NewStudio
/*        [HttpGet]
        public async Task<ActionResult<IEnumerable<Studio>>> GetStudios()
        {
            return await _context.Studios.ToListAsync();
        } */
    }
}