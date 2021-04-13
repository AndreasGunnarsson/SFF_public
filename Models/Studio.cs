using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class Studio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<MovieStudio> MovieStudios { get; set; }
        public List<Rating> Ratings { get; set; }
    }
    
    public class StudioDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string Name { get; set; }
        [Required]
        [StringLength(10)]
        public string Location { get; set; }
    }
}