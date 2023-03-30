using System.ComponentModel.DataAnnotations.Schema;

namespace MasterDetail.Shared
{
    public class BookingEntry
    {
        public int BookingEntryId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Spot")]
        public int SpotId { get; set; }

        //Navigation
        public virtual Customer? Customer { get; set; }
        public virtual Spot? Spot { get; set; }
    }
}
