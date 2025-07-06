using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Models
{
    public class UygulamaDbContext : IdentityDbContext<Kullanici, Rol, string>
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {
        }

        public DbSet<SanatEseri> SanatEserleri { get; set; }
        public DbSet<Sanatci> Sanatcilar { get; set; }
        public DbSet<Ulke> Ulkeler { get; set; }
        public DbSet<Sergi> Sergiler { get; set; }
        public DbSet<GaleriKonum> GaleriKonumlari { get; set; }
        public DbSet<Giris> Girisler { get; set; }
        public DbSet<Cikis> Cikislar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
    }
} 