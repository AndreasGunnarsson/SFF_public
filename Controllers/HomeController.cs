using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using test_SFF.Models;
using test_SFF.Data;
using test_SFF;
using Microsoft.EntityFrameworkCore;

namespace test_SFF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

// ----------------------------------------------------------- API:
/*    [ApiController]
    [Route("api")]                  // Kommer att ha addressen /api
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // TODO: Vet inte ifall detta är rätt sätt att få in DbContext? */
/*        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<MovieController> _logger;

        public WeatherForecastController(ILogger<MovieController> logger)
        {                                                                                                                                                                                                                                                                 
            _logger = logger;                                                                                                                                                                                                                                             
        } */
/*        public MovieController(ApplicationDbContext context)    // TODO: Bör man använda "using" istället?
        {
            _context = context;
        }

        [HttpGet]
        [Route("films")]         // /api/films
        public IEnumerable<Movie> Get()   
        {                                                                                                                                                                                                                                                                 
            var rng = new Random();                                                                                                                                                                                                                                       
            return Enumerable.Range(1, 5).Select(index => new Movie 
            {
                Name = "Bob",
                Year = DateTime.Now.AddDays(index),
                TotalAmount = 2,
                DigitalPhysical = true
                //TemperatureC = rng.Next(-20, 55),
                //Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("fis")]
        public void fis()
        {
            _context.Add(new Movie {Name = "boob", Year = DateTime.Now, TotalAmount = 2, DigitalPhysical = false});
            _context.SaveChanges();
        }
// ---------------------------------------- Movie:
        public void AddMovie(){}        // Lägger till en ny film i listan och sparar till databasen.
        public void MovieRentLimit(){}  // Ändrar "limit" för hur många filmer det finns att låna ut i Movie-objektet.
// ---------------------------------------- Mellantabell (Movie/Filmförening):
        public void MarkRented(){}      // För att markera att en film som utlånad. Får inte gå över "limiten" och en studio får endast låna en film. Måste skriva in både studio och film-id.
        public void MarkReturned(){}    // För att lämna tillbaka en film. Tar bort film från mellantabellen. Man måste skriva in både studio och film-id.
        public void ListRented(){}      // Tar ett id för en förening och visar alla lånade filmer.
// ---------------------------------------- Filmförening:
        public void AddFörening(){}     // Lägger till ny förening.
        public void RemoveFörening(){}
        public void ChangeInfo(){}      // Byta namn och ort.
// ---------------------------------------- Trivia:
        public void AddTrivia(int movieId, int föreningsId, string trivia)       // Lägger till trivia till en film efter den lämnats tillbaka av en filmförening.
        {}
        public void EditTrivia(int movieId, int föreningsId, string updatedTrivia){}      // Kräver att man anger vilken film och vilken filmstudio.
        public void AddScore(int movieId, int föreningsId, int score){}        // Efter att en film lämnats tillbaka kan en filmstudio sätta ett poäng på lånad film.
    }
} */

/*
 TODO:
 - Ska endast vara routing-logik här; business-logik sköts i model.
 - Kolla vart gränsen för "business logic" går mellan controller och "model".
*/

/*  [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {                                                                                                                                                                                                                                                                 
            _logger = logger;                                                                                                                                                                                                                                             
        }                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                          
        [HttpGet]                                                                                                                                                                                                                                                         
        public IEnumerable<WeatherForecast> Get()                                                                                                                                                                                                                         
        {                                                                                                                                                                                                                                                                 
            var rng = new Random();                                                                                                                                                                                                                                       
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast                                                                                                                                                                                             
            {                                                                                                                                                                                                                                                             
                Date = DateTime.Now.AddDays(index),                                                                                                                                                                                                                       
                TemperatureC = rng.Next(-20, 55),                                                                                                                                                                                                                         
                Summary = Summaries[rng.Next(Summaries.Length)]                                                                                                                                                                                                           
            })                                                                                                                                                                                                                                                            
            .ToArray();                                                                                                                                                                                                                                                   
        }                                                                                                                                                                                                                                                                 
    } */