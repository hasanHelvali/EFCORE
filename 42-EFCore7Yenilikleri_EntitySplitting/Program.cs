

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Entity Splitting
//Birden fazla fiziksel tabloyu Entity Framework Core kısmında tek bir entity ile temsil etmemizi sağlayan bir özelliktir.
#endregion
#region Örnek
#region Veri Eklerken
Person person = new()
{
    Name = "Nevin",
    Surname = "Yıldız",
    City = "Ankara",
    Country = "Türkiye",
    PhoneNumber = "1234567890",
    PostCode = "1234567890",
    Street = "..."
    //Tablo ayırma sonucunda bu sekilde veri ekledigimiz zaman ilgili kolonlar efcore tarafından konfigurasyondaki gibi ayrıstırılır.
    //Biz tek yerden veri ekleriz lakin bu db de 3 tane iliskili tabloda tutulur.
};

//await context.Persons.AddAsync(person);
//await context.SaveChangesAsync();
#endregion
#region Veri Okurken
person = await context.Persons.FindAsync(2);
//Farklı tablolardaki veriler tek instance uzerinden bize getirilir.
Console.WriteLine();
#endregion
#endregion
public class Person
{
    #region Persons Tablosu
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    #endregion
    #region PhoneNumbers Tablosu
    public string? PhoneNumber { get; set; }
    #endregion
    #region Addresses Tablosu
    public string Street { get; set; }
    public string City { get; set; }
    public string? PostCode { get; set; }
    public string Country { get; set; }
    #endregion

    //Buradaki property lerin ayrımını konfigurasyon ile yapıyoruz. 
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entityBuilder =>
        {
            entityBuilder.ToTable("Persons")//Ana tablo belirtilir.
                .SplitToTable("PhoneNumbers", tableBuilder =>//Bu tabloyu ayırıyoruz.
                {
                    tableBuilder.Property(person => person.Id).HasColumnName("PersonId");//İliskiyi belirtiyoruz.
                    tableBuilder.Property(person => person.PhoneNumber);//Kolonu belirliyoruz.
                })
                .SplitToTable("Addresses", tableBuilder =>
                {
                    tableBuilder.Property(person => person.Id).HasColumnName("PersonId");//İliskiyi belirtiyoruz.
                    tableBuilder.Property(person => person.Street);//Kolonu belirliyoruz.
                    tableBuilder.Property(person => person.City);//Kolonu belirliyoruz.
                    tableBuilder.Property(person => person.PostCode);//Kolonu belirliyoruz.
                    tableBuilder.Property(person => person.Country);//Kolonu belirliyoruz.
                });
        });
        /*Bu konfigurasyon ile butun entity ler  person entity si uzerinden konfigure edilmis oldu.
         Migrate islemi sonucunda db de 3 farklı iliskisel tablo olusturulmus olur. Guzelligi ise biz buradak sadece person uzerinden 
        yapacagımız operasyonlar, db de 3 tablo icin uygun hale getirilir.
         */
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}