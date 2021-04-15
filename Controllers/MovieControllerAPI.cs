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

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeAmount(int id, MovieDTO movieDTO)
        {
            var movieQuery = await _context.Movies.FindAsync(id);
            if (movieQuery == null)
                return NotFound();

            movieQuery.TotalAmount = movieDTO.TotalAmount;
            // TOD: Måste man hantera vad som händer med filmer som redan är utlånade?
                // T.ex. ifall man lånat ut totalt 10 och man sedan sätter TotalAmount till 6?
                    // När filmerna sedan är returnerade kommer ..
                        // Tror itne detta är ett problem.
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
        // - Det ska gå att markera att en film är utlånad till en filmstudio (man får inte låna ut den mer än filmen finns tillgänglig (max-antal samtidiga utlåningar)
        [Route("MS")]       // POST: /api/Movies/MS
        [HttpPost]
        public async Task<ActionResult<MovieStudio>> PostMovieStudio(MovieStudio moviestudioDTO)
        {
            // Kollar ifall det finns tillräckligt många kopior för att låna ut en film. Jämför antalet utlånade i MoviStudios med Movies.TotalAmount.
            int movieStudioQuery = _context.MovieStudios.Where(x => x.MovieId == moviestudioDTO.MovieId).Count();
            Movie movieQuery = await _context.Movies.FindAsync(moviestudioDTO.MovieId);
            if (movieStudioQuery >= movieQuery.TotalAmount)
                return NotFound();

            // Kollar så att inte MovieId och StudioId finns på samma rad i tabellen MovieStudios. För att motverka att samma studio lånar samma film.
            bool isNotDuplicates = false;
            isNotDuplicates = _context.MovieStudios.Any(x => x.MovieId == moviestudioDTO.MovieId && x.StudioId == moviestudioDTO.StudioId);
            if (isNotDuplicates == true)
                return NotFound();

            MovieStudio moviestudio = new MovieStudio
            {
                MovieId = moviestudioDTO.MovieId,
                StudioId = moviestudioDTO.StudioId,
                ReturnDate = moviestudioDTO.ReturnDate,
                Returned = false
            };
            // TODO: Väldigt viktigt att kolla att Id på Movie och Studio existerar annars kommer databasen klaga på fel FK.
            // TODO: Man ska kunna låna samma film en gång till ifall den är Returned = True; Sätter bara Returned till false igen.
            // TODO: Hårdkoda ett ReturnDate; ta dagens datum + 7 dagar?
            
            if (!MovieExists(moviestudio.MovieId) || !StudioExists(moviestudio.StudioId))
                return NotFound();
            else
            {
                _context.MovieStudios.Add(moviestudio);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovieStudio", new { id = moviestudioDTO.Id }, moviestudioDTO);
                // TODO: Denna returnerar inte DTO! Tror att det har att göra med att "GetMovieStudio" är en actionmetod som måste existera.
                    // Spännande notis är att här fungerar iaf "id" (returnerar rätt id).
                    // Lösning: Har något att göra med Add()?
            }
        }

        // PUT: api/Movies/5/5
//      [HttpPut("{movieid:int}/{studioid:int}")]
        //[Route("MS")]       // POST: /api/Movies/MS/5
        
//      public async Task<IActionResult> ReturnMovie(int movieid, int studioid)
//        [Route("MS3")]
        [HttpPut("MS3/{id:int}")]
        public async Task<IActionResult> ReturnMovie(int id)
        {
            //var movieBefore = await _context.Movies.FindAsync(id);
            //var query = _context.MovieStudios.Where<MovieStudio>(x => x.MovieId == movieid && x.StudioId == studioid).First();
            var query = await _context.MovieStudios.FindAsync(id);
            if (query == null)
                return NotFound();
            //if (movieBefore == null)
            //    return NotFound();

            query.Returned = true;
            //movieBefore.TotalAmount = movieDTO.TotalAmount;
            // TOD: Måste man hantera vad som händer med filmer som redan är utlånade?
                // T.ex. ifall man lånat ut totalt 10 och man sedan sätter TotalAmount till 6?
                    // När filmerna sedan är returnerade kommer ..
                        // Tror itne detta är ett problem.
            try
            {
                await _context.SaveChangesAsync();
            }
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
            }

            return NoContent();
        }

        // TODO: Måste finnas snyggare lösning än detta (route):
        // TODO: Alternativt så ta bara det id som båda (movieid och studioid) använder?
/*        [HttpDelete("{movieid:int}/{studioid:int}")]   // DELETE: /api/Movies/2/1
        public async Task<IActionResult> ReturnMovie([FromRoute] int movieid, [FromRoute] int studioid)
        {
            // TODO: Problem med denna implementation: eftersom att man tar bort raden med lånad film
            // - Det ska gå att ändra så att filmen inte längre är utlånad till en viss filmstudio (lämna tillbaka).
            
            //IEnumerable<MovieStudio> moviestudiofromdb = await _context.MovieStudios.ToListAsync<MovieStudio>();
            var query = _context.MovieStudios.Where<MovieStudio>(x => x.MovieId == movieid && x.StudioId == studioid).First();
            // TODO: .Where ovanför gör det den ska men if-satsen under fungerar ej?
            if (query == null)
            {
                return NotFound();
            } */
            // TODO: Byt ut denna mot en where direkt..
/*            var moviestudioquery =
                (from ms in moviestudiofromdb
                where ms.MovieId == movieid && ms.StudioId == studioid
                select ms).First(); */
            
/*            _context.MovieStudios.Remove(query);
            await _context.SaveChangesAsync();

            return NoContent();
        } */

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
