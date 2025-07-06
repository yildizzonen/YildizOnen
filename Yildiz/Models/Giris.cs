using System.ComponentModel.DataAnnotations.Schema;

namespace Yildiz.Models
{
    public class Giris
    {
        public int Id { get; set; }
        public int SanatEseriId { get; set; }
        public SanatEseri SanatEseri { get; set; }
        public DateTime GirisTarihi { get; set; }
    }
}
