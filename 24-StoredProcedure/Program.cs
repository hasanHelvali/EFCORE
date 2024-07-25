using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();
#region Stored Procedure Nedir?
//SP, view'ler gibi kompleks sorgularımızı daha basit bir şekilde tekrar kullanılabilir  bir hale getirmemiz isağlayan veritabanı nesnesidir. 
//View'ler tablo misali bir davranış sergilerken, SP'lar ise fonksiyonel bir davranış sergilerler.
//Ve türlü türlü artılarıda vardır.
#endregion

#region EF Core İle Stored Procedure Kullanımı
#region Stored Procedure Oluşturma
//1. adım : boş bir migration oluşturunuz.
//2. adım : migration'ın içerisindeki Up fonksiyonuna SP'ın Create komutlarını yazınız, Down fonk. ise Drop komutlarını yazınız.
//3. adım : migrate ediniz.

//En altta migration a yazılan kodlar yorum olarak verilmistir.
#endregion
#region Stored Procedure'ü Kullanma
//SP'ı kullanabilmek için bir entity'e ihtiyacımız vardır. Bu entity'nin DbSet propertysi ollarak context nesnesine de eklenmesi gerekmektedir.
//Bu DbSet properytysi üzerinden FromSql fonksiyonunu kullanarak 'Exec ....' komutu eşliğinde SP yapılanmasını çalıştırıp sorgu neticesini
//elde edebilirsiniz.
#region FromSql
//var datas = await context.PersonOrders.FromSql($"EXEC sp_PersonOrders") 
//    .ToListAsync();

//stored proc yapısı bir fonksiyonalite davranısı yaptıgından execute edilir.

#endregion

#endregion

#region Geriye Değer Döndüren Stored Procedure'ü Kullanma

//Mig 3 te ilgili kod yazılmıstır.  Asagıda yoruma alınmıstır.

//SqlParameter countParameter = new()
//{
//    ParameterName = "count",
//    SqlDbType = System.Data.SqlDbType.Int,
//    Direction = System.Data.ParameterDirection.Output
//sp yapılanması geriye bir deger dondursede veya islemi output olarak degerlendirsede biz bu iki islem icin de output tanımlaması 
//yapmamız gerekir.

//};
//await context.Database.ExecuteSqlRawAsync($"EXEC @count = sp_bestSellingStaff", countParameter);

//Burada herhangi bir entity ye bagımlı sorgu atmadıgımız icin, sp un calısması sonucunda sadece bir deger elde etmek istedigim icin burada
//.Database ile bir islem yapıyorum. Ayruca @count isimli degiskene donen degeri assign ettigim icin bu degiskeni daha oncesinden
//SqlParameters ile olusturmam gerekiyor.

//Console.WriteLine(countParameter.Value);
#endregion
#region Parametreli Stored Procedure Kullanımı

#region Input Parametreli Stored Procedure'ü Kullanma
//3.migration da ilgili procedure insaa edilmistir.
#endregion

#region Output Parametreli Stored Procedure'ü Kullanma
//3.migration da ilgili procedure insaa edilmistir.

#endregion

//SqlParameter nameParameter = new()
//{
//    ParameterName = "name",
//    SqlDbType = System.Data.SqlDbType.NVarChar,
//    Direction = System.Data.ParameterDirection.Output,
//    Size = 1000
//};
//await context.Database.ExecuteSqlRawAsync($"EXECUTE sp_PersonOrders2 7, @name OUTPUT", nameParameter);
//Console.WriteLine(nameParameter.Value);

//Seklinde parametreli bir sekilde stored procedure kullanılmıstır. Burada input parametresi 7 dir. Output parametresi ise name seklinde
//ayarlanmıstır. Verilen input a karsılık db de gelen kaydı output olarak name seklinde elde ettik ve console ekranında yazdırmıs olduk.

#endregion
#endregion
Console.WriteLine();
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

[NotMapped]//Bu entity yi sp u kullanabilmek icin ekledik. Ancak view de oldugu gibi bir tablo olarak algılanmaması gerekiyor. 
//Bunun icin bu attribute u ekledik.
public class PersonOrder
{
    public string Name { get; set; }
    public int Count { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PersonOrder> PersonOrders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<PersonOrder>()
            .HasNoKey();//herhangi bir pk yok.

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

/* Mig2
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoredProcedures.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                        CREATE PROCEDURE sp_PersonOrders
                        AS
	                        SELECT p.Name, COUNT(*) [Count] FROM Persons p
	                        JOIN Orders o
		                        ON p.PersonId = o.PersonId
	                        GROUP By p.Name
	                        ORDER By COUNT(*) DESC
                        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP PROC sp_PersonOrders");
        }
    }
}

 */



/*Mig3
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoredProcedures.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                    CREATE PROCEDURE sp_bestSellingStaff
                    AS
	                    DECLARE @name NVARCHAR(MAX), @count INT
	                    SELECT TOP 1 @name = p.Name, @count = COUNT(*) FROM Persons p
	                    JOIN Orders o
		                    ON p.PersonId = o.PersonId
	                    GROUP By p.Name
	                    ORDER By COUNT(*) DESC
	                    RETURN @count
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP PROCEDURE sp_bestSellingStaff");
        }
    }
}

 */

/*Mig 4 
 
 ﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoredProcedures.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                    CREATE PROCEDURE sp_PersonOrders2
                    (
	                    @PersonId INT,
	                    @Name NVARCHAR(MAX) OUTPUT
                    )
                    AS
                    SELECT @Name = p.Name FROM Persons p
                    JOIN Orders o
                        ON p.PersonId = o.PersonId
                    WHERE p.PersonId = @PersonId
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP PROC sp_PersonOrders2");
        }
    }
}

 */