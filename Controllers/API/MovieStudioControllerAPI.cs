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
    [Route("api/MovieStudio")]
    [ApiController]
    public class MovieStudioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieStudioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/MovieStudio
        [HttpPost]
        public async Task<ActionResult<MovieStudioDTO>> PostMovieBorrow(MovieStudioDTO movieStudioDTO)
        {
            // Finns tillräckligt många kopior för att låna ut en film?
            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == movieStudioDTO.MovieId && x.Returned == false).Count();
            Movie movieQuery = await _context.Movies.FindAsync(movieStudioDTO.MovieId);
            if (movieQuery == null)
                return NotFound();
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return BadRequest();

            // Har samma studio lånat samma film en gång tidigare utan att returnera?
            bool isDuplicates = false;
            isDuplicates = _context.MovieStudios.Any(x => x.MovieId == movieStudioDTO.MovieId && x.StudioId == movieStudioDTO.StudioId && x.Returned == false);
            if (isDuplicates == true)
                return BadRequest();

            // Är datumet efter dagens datum?
            if (movieStudioDTO.ReturnDate <= DateTime.Now.Date)
                return BadRequest();

            MovieStudio movieStudio = new MovieStudio
            {
                MovieId = movieStudioDTO.MovieId,
                StudioId = movieStudioDTO.StudioId,
                ReturnDate = movieStudioDTO.ReturnDate,
                Returned = false,
                Score = 0
            };
            
            if (!MovieExists(movieStudio.MovieId) || !StudioExists(movieStudio.StudioId))
                return NotFound();
            else
            {
                _context.MovieStudios.Add(movieStudio);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovieStudio", new { id = movieStudio.Id }, movieStudioDTO);
            }
        }

        // PUT: /api/MovieStudio/#
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutReturnMovie(int id)
        {
            MovieStudio movieStudioQuery = await _context.MovieStudios.FindAsync(id);
            if (movieStudioQuery == null)
                return NotFound();

            movieStudioQuery.Returned = true;

            if (!_context.MovieStudios.Any(e => e.Id == id))
                return NotFound();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: /api/MovieStudio/Rating/#
        [HttpPut("Rating/{id:int}")]
        public async Task<IActionResult> PutRating(int id, MovieStudioRating movieStudioRating)
        {
            var movieStudioQuery = await _context.MovieStudios.FindAsync(id);
            if (movieStudioQuery == null)
                return NotFound();

            if (movieStudioQuery.Returned == false)
                return NotFound();

            movieStudioQuery.Score = movieStudioRating.Score;
            movieStudioQuery.Review = movieStudioRating.Review;

            await _context.SaveChangesAsync();

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

        // GET: api/MovieStudio/#
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieStudio>> GetMovieStudio(int id)
        {
            var moviestudio = await _context.MovieStudios.FindAsync(id);

            if (moviestudio == null)
            {
                return NotFound();
            }

            return moviestudio;
        }
    }
}
