using System.ComponentModel.DataAnnotations.Schema;

namespace Yildiz.Models
{
    public class Islem
    {
        public int Id { get; set; }
        public int? SanatEseriId { get; set; }
        public SanatEseri? SanatEseri { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool GirisMi { get; set; }
    }
}
