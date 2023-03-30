namespace MasterDetail.Shared
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public int Phone { get; set; }
        public string? Picture { get; set; } = null!;
        public bool MaritialStatus { get; set; }
        //Nev
        public virtual ICollection<BookingEntry>? BookingEntries { get; set; } = new List<BookingEntry>();
    }
}
