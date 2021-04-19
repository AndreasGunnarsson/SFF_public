using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieDTO
    {
//      public int Id { get; set; }         // TODO: Fyller ingen funktion?
        [StringLength(10)]
        public string Name { get; set; }
//      public DateTime Year { get; set; }
        [Range(0, 999)]
        public int TotalAmount { get; set; }
        public bool PhysicalCopy { get; set; }
    }
}