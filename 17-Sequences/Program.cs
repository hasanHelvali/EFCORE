using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
ApplicationDbContext context = new();

#region Sequence Nedir?
//Veritabanında benzersiz ve ardışık sayısal değerler üreten veritabanı nesnesidir.
//Sequence herhangi bir tablonun özelliği değildir. Veritabanı nesnesidir. Birden fazla tablo tarafından kullanılabilir.
#endregion
#region Sequence Tanımlama
//Tamamen fluent api ile konfigure edilir. Sequence'ler üzerinden değer oluştururken veritabanına özgü çalışma yapılması zaruridir.
//SQL Server'a özel yazılan Sequence tanımı misal olarak Oracle için hata verebilir.

#region HasSequence

#endregion
#region HasDefaultValueSql

#endregion
#endregion

//await context.Employees.AddAsync(new() { Name = "Gençay", Surname = "Yıldız", Salary = 1000 });
//await context.Employees.AddAsync(new() { Name = "Mustafa", Surname = "Yıldız", Salary = 1000 });
//await context.Employees.AddAsync(new() { Name = "Suaip", Surname = "Yıldız", Salary = 1000 });

//await context.Customers.AddAsync(new() { Name = "Muhyiddin" });
//await context.SaveChangesAsync();

//Burada db ye bakıldıgında customer a 1 id si, employee lara da sırasıyla 2,3,4 degerleri atandı. Burada 2 tablo icin 1 tane sequence 
//uzerinde calıstıgımızı gormemiz gerek.

#region Sequence Yapılandırması

#region StartsAt

#endregion
#region IncrementsBy

#endregion
#endregion
#region Sequence İle Identity Farkı
//Sequence bir veritabanı nesnesiyken, Identity ise tabloların özellikleridir.
//Yani Sequence herhangi bir tabloya bağımlı değildir. Bircok tabloda bir tane sequence i kullanabiliriz.
//Identity bir sonraki değeri diskten alırken Sequence ise RAM'den alır. Bu yüzden önemli ölçüde Identity'e nazaran daha hızlı,
//performanslı ve az maliyetlidir.
#endregion

class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
}
class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.HasSequence("EC_Sequence");//Bu sekilde sequence tanımlanır.

        modelBuilder.HasSequence("EC_Sequence")//burada ise sequence i yapılandıralım. 
            .StartsAt(100)//100 den basla
            .IncrementsBy(5);//5er 5er arttır demis olduk.


        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
        //Bu sequence ten sonra bir sonraki degeri getir demis olduk.Ne icin? ID kolonu icin
        //Boylece artıs ozelligi identity den degil sequence den gelmis olur.
        modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}