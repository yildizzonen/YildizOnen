using System.ComponentModel.DataAnnotations;

namespace Yildiz.ViewModels
{
    public class KayitOlViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Eposta { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string SifreTekrar { get; set; }
    }
} 