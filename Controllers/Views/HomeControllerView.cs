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

//      public IActionResult Index()
        public async Task<IActionResult> Index()
        {
            List<string> movieCoverImagesStatus = new List<string>();
            string filepath;
            var moviesQuery = await _context.Movies.ToListAsync();

            foreach(var i in moviesQuery)
            {
                filepath = @"wwwroot\images\" + i.Name + ".png";
                if (System.IO.File.Exists(filepath))
                    movieCoverImagesStatus.Add(i.Name);
                else
                    movieCoverImagesStatus.Add("NonExistent");
            }
            ViewData["availableImages"] = movieCoverImagesStatus;

            List<MovieStudio> movieStudioQuery = await _context.MovieStudios.Where(x => x.Score > 0).ToListAsync();

            var movieAverageScore = from ms in movieStudioQuery
                group ms by ms.MovieId into newtable
                select new
                {
                    MovieId = newtable.Key,
                    AverageScore = newtable.Average(x => x.Score)
                };

            var joinedTables = (from movie in moviesQuery
                join ma in movieAverageScore on movie.Id equals ma.MovieId into newtable
                from variablename in newtable.DefaultIfEmpty()
                select new MovieWithRating {
                    Id = movie.Id,
                    Name = movie.Name,
                    TotalAmount = movie.TotalAmount,
                    PhysicalCopy = movie.PhysicalCopy,
                    AverageScore = variablename?.AverageScore ?? 0
                }
            ).ToList();

            return View(joinedTables);
        }

        public IActionResult Review(int id)
        {
            return View(_context.MovieStudios.Where(x => x.MovieId == id));
        }

/*        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
    }
}