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
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace test_SFF.Controllers
{
    public class HomeController : Controller
    {
/*      private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        } */

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

//        [AllowAnonymous]                // Gör så att man kan komma åt sidan utan inlogg.
        public IActionResult Index()
        {
            // TODO: Lägg till funktionalitet för att returnera även en lista med objekt över vilka filmer som är tillgängliga (för bilder).
                // Måste kolla Movies-listan
            List<string> movieCoverImagesStatus = new List<string>();
            string filepath;
            var moviesQuery = _context.Movies.ToList();

            foreach(var i in moviesQuery)
            {
                filepath = @"wwwroot\images\" + i.Name + ".png";
                if (System.IO.File.Exists(filepath))
                    movieCoverImagesStatus.Add(i.Name);
                else
                    movieCoverImagesStatus.Add("NonExistent");
            }
            ViewData["availableImages"] = movieCoverImagesStatus;
            // Kolla File.IO
            // Kolla ifall filen existerar
            // Spara i ny List

              return View(moviesQuery);  
        //    return View(_context.Movies.ToList());
        }

/*      [AllowAnonymous]                // Gör så att man kan komma åt sidan utan inlogg.
        public IActionResult Index()
        {
            return View();
        } */

/*        [AllowAnonymous]                // Gör så att man kan komma åt sidan utan inlogg.
        public IActionResult Privacy()
        {
            return View();
        } */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}