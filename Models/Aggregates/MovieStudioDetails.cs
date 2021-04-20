using System;
using System.Collections.Generic;

namespace test_SFF
{
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