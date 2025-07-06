using Microsoft.AspNetCore.Identity;

namespace Yildiz.Models
{
    public class Kullanici : IdentityUser
    {
        // Ekstra Türkçe alanlar eklenebilir
        public string Ad { get; set; }
        public string Soyad { get; set; }
    }
} 
 