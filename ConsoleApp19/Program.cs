using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Film tablosu
public class Film
{
    [Key]
    public int FilmId { get; set; }
    public string FilmAdi { get; set; }
    public virtual ICollection<Seans> Seanslar { get; set; }
}

// Salon tablosu
public class Salon
{
    [Key]
    public int SalonId { get; set; }
    public string SalonAdi { get; set; }
    public int KoltukKapasitesi { get; set; }
    public virtual ICollection<Seans> Seanslar { get; set; }
}

// Seans tablosu
public class Seans
{
    [Key]
    public int SeansId { get; set; }
    public DateTime TarihSaat { get; set; }
    public int FilmId { get; set; }
    public int SalonId { get; set; }
    public virtual Film Film { get; set; }
    public virtual Salon Salon { get; set; }
    public virtual ICollection<Koltuk> Koltuklar { get; set; }
}

// Koltuk tablosu
public class Koltuk
{
    [Key]
    public int KoltukId { get; set; }
    public int SeansId { get; set; }
    public bool BosMu { get; set; }
    public virtual Seans Seans { get; set; }
}
using Microsoft.EntityFrameworkCore;

public class SinemaContext : DbContext
{
    public DbSet<Film> Filmler { get; set; }
    public DbSet<Salon> Salonlar { get; set; }
    public DbSet<Seans> Seanslar { get; set; }
    public DbSet<Koltuk> Koltuklar { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=Sinema.db");
    }
}
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        using (var context = new SinemaContext())
        {
            // Örnek film, salon ve seanslar oluşturalım
            var film = new Film { FilmAdi = "Örnek Film" };
            var salon = new Salon { SalonAdi = "Salon 1", KoltukKapasitesi = 50 };
            var seans = new Seans { TarihSaat = DateTime.Now, Film = film, Salon = salon };

            // Seans için koltukları oluşturalım (örneğin hepsi boş olsun)
            for (int i = 1; i <= salon.KoltukKapasitesi; i++)
            {
                seans.Koltuklar.Add(new Koltuk { BosMu = true });
            }

            context.Filmler.Add(film);
            context.Salonlar.Add(salon);
            context.Seanslar.Add(seans);
            context.SaveChanges();

            // Veritabanından bir seansı ve onun koltuk durumlarını alalım
            var secilenSeans = context.Seanslar.Include(s => s.Koltuklar).First();
            Console.WriteLine("Seans Tarihi: " + secilenSeans.TarihSaat);
            Console.WriteLine("Film: " + secilenSeans.Film.FilmAdi);
            Console.WriteLine("Salon: " + secilenSeans.Salon.SalonAdi);
            Console.WriteLine("Boş Koltuklar:");
            foreach (var koltuk in secilenSeans.Koltuklar)
            {
                if (koltuk.BosMu)
                    Console.WriteLine("Koltuk " + koltuk.KoltukId + " - Boş");
                else
                    Console.WriteLine("Koltuk " + koltuk.KoltukId + " - Dolu");
            }
        }
    }
}
