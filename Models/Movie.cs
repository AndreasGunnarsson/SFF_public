using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Range(1, 999)]
        [Display(Name = "Total Amount")]
        public int TotalAmount { get; set; }
        [Display(Name = "Physical Copy")]
        public bool PhysicalCopy { get; set; }
        public List<MovieStudio> MovieStudios { get; set; }
    }
}