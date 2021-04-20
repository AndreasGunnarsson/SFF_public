using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieStudioRating
    {
        [Required]
        [StringLength(40)]
        public string Review { get; set; }
        [Required]
        [Range(1, 5)]
        public int Score { get; set; }
    }
}