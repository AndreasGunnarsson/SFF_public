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

        // POST: api/Studios
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

        // DELETE: api/Studios/5
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

        // PUT: api/Studios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudio(int id, StudioDTO studioDTO)
        {
            var studioBefore = await _context.Studios.FindAsync(id);

            if (studioBefore == null)
                return NotFound();

            studioBefore.Name = studioDTO.Name;
            studioBefore.Location = studioDTO.Location;

            try
            {
                await _context.SaveChangesAsync();
            }
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
            }

            return NoContent();
        }

        private bool StudioExists(int id)
        {
            return _context.Studios.Any(e => e.Id == id);
        }

        // TODO: För att hämta vilka filmer en viss förening har lånat.
        // GET: api/Studio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Studio>> GetMovieStudio(int id)
        {
            var studio = await _context.Studios.FindAsync(id);

            // Måste joina Movie, Studio med MovieStudio för att kunna skriva ut namn på allt.
                // Skapa en ny list där allt sparas temporärt?
            if (studio == null)
            {
                return NotFound();
            }

            return studio;
        }

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