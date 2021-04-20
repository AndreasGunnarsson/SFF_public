using System;
using System.Collections.Generic;
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
        public DateTime ReturnDate { get; set; }
        public bool Returned { get; set; }
        public string Review { get; set; }
        public double Score { get; set; }
    }

    public class MovieName
    {
        public int MovieStudioId { get; set; }
        public string Name { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool PhysicalCopy { get; set; }
        public bool Returned { get; set; }
    }

    public class MovieStudioDetails
    {
        public Studio Studio { get; set; }
        public Movie Movie { get; set; }
        public List<MovieName> JoinedList { get; set; }
    }
}