using System;

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
    }

    public class MovieStudioDTO
    {
        public int Id { get; set; }             // TODO: Remove Id from DTO?
        public int MovieId { get; set; }
        public int StudioId { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool Returned { get; set; }
    }
}