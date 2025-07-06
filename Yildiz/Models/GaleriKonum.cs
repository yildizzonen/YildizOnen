namespace Yildiz.Models
{
    public class GaleriKonum
    {
        public int Id { get; set; }
        public string Isim { get; set; }
        public string Adres { get; set; }
        public ICollection<Sergi> Sergiler { get; set; }
    }
}
