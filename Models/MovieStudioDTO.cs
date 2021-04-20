using System;
using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieStudioDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int StudioId { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool Returned { get; set; }
    }
}