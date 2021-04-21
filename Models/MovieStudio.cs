using System;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieStudio
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int StudioId { get; set; }
        public Studio Studio { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        public bool Returned { get; set; }
        public string Review { get; set; }
        public double Score { get; set; }
    }
}