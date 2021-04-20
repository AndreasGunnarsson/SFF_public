using System.ComponentModel.DataAnnotations;

namespace test_SFF
{
    public class MovieAvailableAmount
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [Range(0, 999)]
        [Display(Name = "Total Amount")]
        public int TotalAmount { get; set; }
        [Display(Name = "Physical Copy")]
        public bool PhysicalCopy { get; set; }
        [Display(Name = "Borrowed Amount")]
        public int BorrowedAmount { get; set; }
    }
}