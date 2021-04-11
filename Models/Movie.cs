using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class Movie
    {
        public int Id { get; set; }
        [StringLength(10)]
        public string Name { get; set; }
//      public DateTime Year { get; set; }
        [Range(0, 999)]
        public int TotalAmount { get; set; }
        public bool PhysicalCopy { get; set; }
        public List<MovieStudio> MovieStudios { get; set; }
//        public bool DigitalPhysical { get; set; }
        public List<Rating> Ratings { get; set; }
    }

        public class MovieDTO
    {
        public int Id { get; set; }
        [StringLength(10)]
        public string Name { get; set; }
//      public DateTime Year { get; set; }
        [Range(0, 999)]
        public int TotalAmount { get; set; }
        public bool PhysicalCopy { get; set; }
    }
}
// TODO: Går det få bara year i DateTime?
// TODO: Hur fixar man med en mellantabell?
