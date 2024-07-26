using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

//DB seviyesinde iki tane kullanıcı tanımlı fonksiyon vardır. Bunlar scalar ve inline fonksiyonlardır.

ApplicationDbContext context = new();
#region Scalar Functions Nedir?
//Geriye herhangi bir türde değer döndüren fonksiyonlardır.
#endregion
#region Scalar Function Oluşturma
//1. adım : boş bir migration oluşturulmalı.
//2. adım : bu migration içerisinde Up metodunda Sql metodu eşliğinde fonksiyonun create kodları yazılacak, Down metodu içerisinde de Drop kodları yazılacaktır.
//3. adım : migrate edilmeli. 

//Mig2 de yazılmıstır. Mig2 icerigi en altta yorum satırı olarak verilmistir.
#endregion
#region Scalar Function'ı EF Core'a Entegre Etme

#region HasDbFunction
//Veritabanı seviyesindeki herhangi bir fonksiyonu EF Core/yazılım kısmında bir metoda bind etmemizi sağlayan fonksiyondur.
#endregion

//var persons = await (from person in context.Persons
//                     where context.GetPersonTotalOrderPrice(person.PersonId) > 500
//                     select person).ToListAsync();

//Bu sekilde bir sorgu yazdık ve ilgili verileri scalar bir fonksyonu efcore da entegre ederek getirdik.

//Console.WriteLine();

#endregion

#region Inline Functions Nedir?
//Geriye bir değer değil, tablo döndüren fonksiyonlardır.
#endregion
#region Inline Function Oluşturma
//1. adım : boş bir migration oluşturunuz.
//2. adım : bu migration içerisindeki Up fonksiyonunda Create işlemini,  down fonksiyonunda ise drop işlemlerini gerçekleştiriniz.
//3. adım : migrate ediniz.

//Ilgili migration icerigi asagıda yorum satırı olarak mig 3 te verilmistir.
#endregion
#region Inline Function'ı EF Core'a Entegre Etme
//Inline funciton lar geriye bir tablo dondugu icin bu tabloyu kasılayacak olan bir modele ihtiyacımız vardır.
//Entegrasyon onModelCreating de yapılmıstır ve BestSellingStaff modeli ilgili entegrasyonda bildirilmistir.
var persons = await context.BestSellingStaff(3000).ToListAsync();
//Bu sekilde ilgili fonksiyonu kullanıyoruz.
foreach (var person in persons)
{
    Console.WriteLine($"Name : {person.Name} | Order Count : {person.OrderCount} | Total Order Price : {person.TotalOrderPrice}");
}
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
    public int Price { get; set; }

    public Person Person { get; set; }
}
public class BestSellingStaff
{
    //İnline function dan donen veriye gore bir model insaa edildi.
    public string Name { get; set; }
    public int OrderCount { get; set; }
    public int TotalOrderPrice { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Scalar
        modelBuilder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(ApplicationDbContext.GetPersonTotalOrderPrice), 
            new[] { typeof(int) }))
            .HasName("getPersonTotalOrderPrice");
        //Db deki function ile buradaki function arasında bir binding islemi yapıyoruz. 2.parametrede ise scalar fonksiyonun parametresini verdik. 
        //Artık biz db deki bir function i context teki GetPersonTotalOrderPrice fonksiyon ismiyle temsil edebilmekteyiz.
        #endregion
        #region Inline
        modelBuilder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(ApplicationDbContext.BestSellingStaff), new[] { typeof(int) }))
            .HasName("bestSellingStaff");

        modelBuilder.Entity<BestSellingStaff>()
            .HasNoKey();//Inline func sonucunda geriye donecek olan verileri tutan tablonun bir pk inin olmadıgını burada bildiriyorum.
        #endregion

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
    }

    /*Hala context icindeyiz. Db de kullandıgımız fonksiyonu efcore da bir fonksiyon olarak kullanmak istiyoruz. Bunun icin bazı 
     konfigurasyonlara ihtiyacımız var. */
    #region Scalar Functions
    public int GetPersonTotalOrderPrice(int personId)
        => throw new Exception();//burada yapılan islem onemli degildir. Biz rastgele bir hata fırlatalım. 
    //db de yazılan fonksiyon geriye int deger donuyor. Yine scalar func ın adına gore fonksiyona bir name verildi.
    //Kısacası burada db deki fonksiyonu temsil edeccek bir imza ve bir yapı olusturuldu. Bundan sonra onModelCreating e gidiyoruz.
    #endregion

    #region Inline Functions
    public IQueryable<BestSellingStaff> BestSellingStaff(int totalOrderPrice = 10000)
         => FromExpression(() => BestSellingStaff(totalOrderPrice)); 
    //Db deki inline func bu sekilde temsil edildi.
    //Inline function ın db deki inline function ile bind i buradaki imza ve yapıyla saglandı. Inline fuction dan bir table dondugu 
    //icin gelen veriyi IQuerable ile karsılamak daha dogrudur. 
    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}



/* Mig 2
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Functions.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                    CREATE FUNCTION getPersonTotalOrderPrice(@personId INT)
	                    RETURNS INT
                    AS
                    BEGIN
	                    DECLARE @totalPrice INT
	                    SELECT @totalPrice = SUM(o.Price) FROM Persons p
	                    JOIN Orders o
		                    ON p.PersonId = o.PersonId
	                    WHERE p.PersonId = @personId
	                    RETURN @totalPrice
                    END
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP FUNCTION getPersonTotalOrderPrice");
        }
    }
}

 */


/*Mig 3 
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Functions.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE FUNCTION bestSellingStaff(@totalOrderPrice INT = 10000)
	                RETURNS TABLE //Burada table dedigimizden dolayı bu bir inline fonksiyon olur. int vs denseydi scalar fonksiyon olurdu.
                AS
                RETURN 
                SELECT TOP 1 p.Name, COUNT(*) OrderCount, SUM(o.Price) TotalOrderPrice FROM Persons p
                JOIN Orders o
	                ON p.PersonId = o.PersonId
                GROUP By p.Name
                HAVING SUM(o.Price) < @totalOrderPrice
                ORDER By OrderCount DESC
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP FUNCTION bestSellingStaff");
        }
    }
}

 */