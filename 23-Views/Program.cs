using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();
#region View Nedir?
//Oluşturduğumuz kompleks sorguları ihtiyaç durumlarında daha rahat bir şekilde kullanabilmek için basitleştiren bir veritabanı objesidir.
#endregion
#region EF Core İle View Kullanımı

#region View Oluşturma
//1. adım : boş bir migration oluşturulmalıdır. Olusturulaqn migration a eklenen kod altta yoruma alınmıstır.
//2. adım : migration içerisindeki Up fonksiyonunda view'in create komutları, down fonksiyonunda ise drop komutları yazılmalıdır.
//3. adım : migrate ediniz.
#endregion
#region View'i DbSet Olarak Ayarlama
//View'i EF Core üzerinden sorgulayabilmek için view neticesini karşılayabilecek bir entity olşturulması ve bu entity
//türünden DbSet property'sinin eklenmesi gerekmektedir.
#endregion
#region DbSet'in Bir View Olduğunu Bildirmek
//OnModelCreating de belirtilir.
#endregion

//var personOrders = await context.PersonOrders //Bu sekilde view tetiklenmis olur. Cunku bu entity ye baglı bir view vardır.
//    .Where(po => po.Count > 10)//Ayrı view uzerine linq sorguları ekleyerek sorguya katkıda da bulunabiliyoruz.
//    .ToListAsync();

#region EF Core'da View'lerin Özellikleri
//Viewlerde primary key olmaz! Bu yüzden ilgili DbSet'in HasNoKey ile işaretlenmesi gerekemktedir.
//View neticesinde gelen veriler Change Tracker ile takip edilmezler. Haliyle üzerlerinde yapılan değişiklikleri EF Core
//veritabanına yansıtmaz

//var personOrder = await context.PersonOrders.FirstAsync();
//personOrder.Name = "Abuzer";
//await context.SaveChangesAsync();

//Bu islem neticesinde db de degisiklik olmaz.
#endregion
Console.WriteLine();
#endregion
public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }

    public Person Person { get; set; }
}
public class PersonOrder
{
    //View den gelen veriyi karsılayacak bir entity insaa edildi.
    public string Name { get; set; }
    public int Count { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PersonOrder> PersonOrders { get; set; }//View i temsil eden entity
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<PersonOrder>()
            .ToView("vm_PersonOrders")
             //Bu sekilde db ye gonderilen view e karsılık ilgili entity bildirildi.
            .HasNoKey();//View lerde primary key olmaz.

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}




/*
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Views.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                        CREATE VIEW vm_PersonOrders
                        AS
	                        SELECT TOP 100 p.Name, COUNT(*) [Count] FROM Persons p
	                        INNER JOIN Orders o
		                        ON p.PersonId = o.PersonId
	                        GROUP By p.Name
	                        ORDER By [Count] DESC
                        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP VIEW vm_PersonOrders");
        }
    }
}
*/

//Bos migration uzerine icindeki kodlar tarafımızdan yazıldı. Bu migration migrate edildiginde db de view olusturulmus olur.
