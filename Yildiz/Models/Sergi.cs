using System.ComponentModel.DataAnnotations.Schema;

namespace Yildiz.Models
{
    public class Sergi
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int? KonumId { get; set; }
        public GaleriKonum? Konum { get; set; }
    }
}
