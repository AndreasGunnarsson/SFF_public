using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieDTO
    {
//      public int Id { get; set; }         // TODO: Fyller ingen funktion?
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
//      public DateTime Year { get; set; }
        [Range(1, 999)]
        public int TotalAmount { get; set; }
        public bool PhysicalCopy { get; set; }
    }
}