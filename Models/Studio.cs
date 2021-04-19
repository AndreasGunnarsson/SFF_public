using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class Studio
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Location { get; set; }
        public List<MovieStudio> MovieStudios { get; set; }
//      public List<Rating> Ratings { get; set; }
    }
}