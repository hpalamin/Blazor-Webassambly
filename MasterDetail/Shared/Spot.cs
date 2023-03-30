using System.Text.Json.Serialization;

namespace MasterDetail.Shared
{
    public class Spot
    {
        public int SpotId { get; set; }
        public string? SpotName { get; set; } = default!;
        //Nev
        [JsonIgnore]
        public virtual ICollection<BookingEntry>? BookingEntries { get; set; } = new List<BookingEntry>();
    }
}
