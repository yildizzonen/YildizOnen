using System.ComponentModel.DataAnnotations.Schema;

namespace Yildiz.Models
{
    public class SanatEseri
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public int? SanatciId { get; set; }
        public Sanatci? Sanatci { get; set; }
        public int Yil { get; set; }
        public string Aciklama { get; set; }
        public string Resim { get; set; }  

        [NotMapped]
        public IFormFile ResimDosyasi { get; set; } 
    }
}
