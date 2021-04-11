using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_SFF;
using test_SFF.Data;

/*
- Det ska gå att ändra hur många studios som kan låna filmen samtidigt
- Det ska gå att markera att en film är utlånad till en filmstudio (man får inte låna ut den mer än filmen finns tillgänglig (max-antal samtidiga utlåningar)
- Det ska gå att ändra så att filmen inte längre är utlånad till en viss filmstudio (lämna tillbaka)
*/
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

        // - Det ska gå att lägga till en ny fysisk eller digital film till databasen
        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovieDTO>> PostMovie(MovieDTO movieDTO)
        {
            var movie = new Movie
            {
                Name = movieDTO.Name,
                TotalAmount = movieDTO.TotalAmount,
                PhysicalCopy = movieDTO.PhysicalCopy
            };
            
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
            // TODO: Returnerar inte rätt Id (det nya).
        }

        // PUT: api/NewMovie/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeAmount(int id, MovieDTO movieDTO)
        {
            var movieOld = await _context.Movies.FindAsync(id);
            if (movieOld == null)
            {
                return NotFound();
            }

/*            var movie = new Movie
            {
                Id = id,
                Name = movieOld.Name,
                TotalAmount = movieDTO.TotalAmount,
                PhysicalCopy = movieOld.PhysicalCopy
            }; */
        
            movieOld.TotalAmount = movieDTO.TotalAmount;

/*          if (id != movieDTO.Id)
            {
                return BadRequest();
            } */
            // TODO: Vi måste hämta en Movie från databasen som vi kan jämföra med.
            // Det vi vill göra: köra en update på det movie-objekt som har "Id = id". Vi vill endast uppdatera "TotalAmount".
            // Problem: Den uppdaterar alla kolumner även om de inte har något värde; vill endast uppdatera TotalAmount.
            // Potentiell lösning: läs in objektet från databasen först, skriv in värdena i "movie" och spara.
//            _context.Entry(movie).State = EntityState.Modified;
//            _context.Entry(movie).State = EntityState.Unchanged;
//            _context.Update(movie);
//            DbSet.
//            _context.Update

            try
            {
                await _context.SaveChangesAsync();
            }
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
            }

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

        // TODO: Ful route:
        [Route("MS")]       // POST: /api/Movies/MS
        [HttpPost]
        public async Task<ActionResult<MovieStudio>> PostMovieStudio(MovieStudio moviestudioDTO)
        {
            var moviestudio = new MovieStudio
            {
                MovieId = moviestudioDTO.MovieId,
                StudioId = moviestudioDTO.StudioId,
                ReturnDate = moviestudioDTO.ReturnDate,
                Returned = false
            };
            //_context.Movies.
            // TODO: Måste kolla ifall det finns några filmer att låna ut; Movie.TotalAmount måste vara > 0 med alla inräknade i MovieStudios.
                // Movie med Id == MovieId måste ha TotalAmount > 0.
                // Vi måste joina MovieStudio ifall och räkna ihop alla MovieId (som är samma som MovieId).
            // TODO: Måste också kolla ifall filmen redan är utlånad till samma studio.
                // Kolla MovieStudio efter StudioId och MovieId på samma rad. Om det är sant så läggs ingen ny till.
            // TODO: Hårdkoda ett ReturnDate; ta dagens datum + 7 dagar?
            
            if (!MovieExists(moviestudio.MovieId) || !StudioExists(moviestudio.StudioId))
                return NotFound();
            else
            {
                _context.MovieStudios.Add(moviestudioDTO);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovieStudio", new { id = moviestudioDTO.Id }, moviestudioDTO);
                // TODO: Denna returnerar inte DTO! Tror att det har att göra med att "GetMovieStudio" är en actionmetod som måste existera.
            }
        }
        // TODO: Måste finnas snyggare lösning än detta (route):
        [HttpDelete("{movieid:int}/{studioid:int}")]   // DELETE: /api/Movies/2/1
        public async Task<IActionResult> ReturnMovie([FromRoute] int movieid, [FromRoute] int studioid)
        {
            // TODO: Problem med denna implementation: eftersom att man tar bort raden med lånad film
            // - Det ska gå att ändra så att filmen inte längre är utlånad till en viss filmstudio (lämna tillbaka).
            
            //_context.MovieStudios.Where<MovieStudio>() TODO: Denna borde gå att använda direkt istället för LINQ nedanför?
            IEnumerable<MovieStudio> moviestudiofromdb = await _context.MovieStudios.ToListAsync<MovieStudio>();
            if (moviestudiofromdb == null)
            {
                return NotFound();
            }

            var moviestudioquery =
                (from ms in moviestudiofromdb
                where ms.MovieId == movieid && ms.StudioId == studioid
                select ms).First();
            
            _context.MovieStudios.Remove(moviestudioquery);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // TODO: Inte snyggaste/mest consistent URL. Fixa på något sätt..
//        [HttpPut("{movieid:int}/{studioid:int}")]   // PUT: /api/Movies/2/1
//        public async Task<IActionResult> ReturnMovie([FromRoute] int movieid, [FromRoute] int studioid)
//        {
            // - Det ska gå att ändra så att filmen inte längre är utlånad till en viss filmstudio (lämna tillbaka)
            // Måste ta bort filmen/studion ur MovieStudio.
            // Måste ta bort den rad där movie och studio är det som man skickat in.
            // Hur gör man för att få med både id från movie och studio? Kan man få två "int id" som parametrar eller skapar man ett nytt objekt?
//            var movieOld = await _context.Movies.FindAsync(id);
/*            if (movieid == 1 && studioid == 2)
            {
                return NotFound();
            } */

/*            var movie = new Movie
            {
                Id = id,
                Name = movieOld.Name,
                TotalAmount = movieDTO.TotalAmount,
                PhysicalCopy = movieOld.PhysicalCopy
            }; */
        
            //movieOld.TotalAmount = movieDTO.TotalAmount;

/*          if (id != movieDTO.Id)
            {
                return BadRequest();
            } */
            // TODO: Vi måste hämta en Movie från databasen som vi kan jämföra med.
            // Det vi vill göra: köra en update på det movie-objekt som har "Id = id". Vi vill endast uppdatera "TotalAmount".
            // Problem: Den uppdaterar alla kolumner även om de inte har något värde; vill endast uppdatera TotalAmount.
            // Potentiell lösning: läs in objektet från databasen först, skriv in värdena i "movie" och spara.
//            _context.Entry(movie).State = EntityState.Modified;
//            _context.Entry(movie).State = EntityState.Unchanged;
//            _context.Update(movie);
//            DbSet.
//            _context.Update

            /*try
            {
                await _context.SaveChangesAsync();
            }
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
            }*/
//            else
//                return NoContent();
//        }

/*        [HttpGet("{id}")]
        public async Task<ActionResult<MovieStudio>> GetMovieStudio(int id)
        {
            var moviestudio = await _context.MovieStudios.FindAsync(id);

            if (moviestudio == null)
            {
                return NotFound();
            }

            return moviestudio;
        } */

/*        public async Task<ActionResult<IEnumerable<MovieStudio>>> GetMovieStudios()
        {
            return await _context.MovieStudios.ToListAsync();
        } */

        // PUT: api/NewMovie/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
/*        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
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
            }

            return NoContent();
        } */

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
