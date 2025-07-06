using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yildiz.Models
{
    public class Sanatci
    {
        public int Id { get; set; }
        public string Isim { get; set; }
        public DateTime DogumTarihi { get; set; }
        public int? UyrukId { get; set; }
        public Ulke? Uyruk { get; set; }
    }
}
