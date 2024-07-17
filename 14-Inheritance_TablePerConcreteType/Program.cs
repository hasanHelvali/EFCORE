using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Table Per Concrete Type (TPC) Nedir?
//TPC davranışı, kalıtımsal ilişkiye sahip olan entitylerin olduğu çalışmalarda sadece concrete/somut olan entity'lere karşılık
//bir tablo oluşturacak davranış modelidir.
//Tablolar azaldıgı dolayısıyla tablolardaki join sayıları azaldıgı icin TPC, TPT'nin daha performanslı versiyonudur.
//TPT de tablolar arasında birebir iliski varken TPC de bu sekilde bir iliski soz konusu degildir. 
#endregion
#region TPC Nasıl Uygulanır?
//Hiyerarşik düzlemde abstract olan yapılar üzerinden OnModelCreating üzerinden Entity fonskiyonuyla konfigürasyona girip ardından UseTpcMappingStrategy fonksiyonu eşliğinde davranışın TPC olacağını belirleyebiliriz.
#endregion
#region TPC'de Veri Ekleme
//await context.Technicians.AddAsync(new() { Name = "Gençay", Surname = "Yıldız", Branch = "Yazılım", Department = "Yazılım Departmanı" });
//await context.Technicians.AddAsync(new() { Name = "Mustafa", Surname = "Yıldız", Branch = "Yazılım", Department = "Yazılım Departmanı" });
//await context.Technicians.AddAsync(new() { Name = "Hilmi", Surname = "Yıldız", Branch = "Yazılım", Department = "Yazılım Departmanı" });
//await context.SaveChangesAsync();
#endregion
#region TPC'de Veri Silme
//Technician? silinecek = await context.Technicians.FindAsync(2);
//context.Technicians.Remove(silinecek);
//await context.SaveChangesAsync();
#endregion
#region TPC'de Veri Güncelleme
//Technician? guncellenecek = await context.Technicians.FindAsync(3);
//guncellenecek.Name = "Ahmet";
//await context.SaveChangesAsync();
#endregion
#region TPC'de Veri Sorgulama
//var datas = await context.Technicians.ToListAsync();
//Console.WriteLine();
#endregion
abstract class Person
{
    /*TPT yaklasımı butun entity lere karsı bir tablo olustururdu. Yani buradaki Person icinde bir tablo olusturur.
     TPC yaklasımı ise sadece somut olan yani abstract olmayan entity lere bir tablo insaa eden bir yaklasımdır.
    Yani buradaki person icin bir tablo olusturmaz. 
    */
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
}
class Employee : Person
{
    public string? Department { get; set; }
}
class Customer : Person
{
    public string? CompanyName { get; set; }
}
class Technician : Employee
{
    public string? Branch { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TPT
        //modelBuilder.Entity<Person>().ToTable("Persons");
        //modelBuilder.Entity<Employee>().ToTable("Employees");
        //modelBuilder.Entity<Customer>().ToTable("Customers");
        //modelBuilder.Entity<Technician>().ToTable("Technicians");

        //veya 
        //modelBuilder.Entity<Person>().UseTptMappingStrategy();


        //TPC
        modelBuilder.Entity<Person>().UseTpcMappingStrategy();
        //TPC de sadece abstract yapılanmanın bildirimi yapılır. Bunun sonucunda beyan edilen abstracxt yapının dısındaki butun entity ler 
        //db de kendine bir tablo olarak karsılık bulur.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}