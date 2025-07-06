namespace Yildiz.Models
{
    public class HataGorunumModeli
    {
        public string? IstekId { get; set; }

        public bool IstekIdGoster => !string.IsNullOrEmpty(IstekId);
    }
}
