namespace test_SFF
{
    public class Rating
    {
        public int Id { get; set; }
        public int StudioId { get; set; }
        public Studio Studio { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string Review { get; set; }
        public int Score { get; set; }
    }
}