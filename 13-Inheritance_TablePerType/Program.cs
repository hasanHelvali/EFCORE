using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Table Per Type (TPT) Nedir?
//Entitylerin aralarında kalıtımsal ilişkiye sahip olduğu durumlarda her bir türe/entitye/tip/referans karşılık bir tablo
//generate eden davranıştır.
//Burada TPT nin farkı Her generate edilen bu tablolar hiyerarşik düzlemde kendi aralarında birebir ilişkiye sahiptir.

//Performans acısından bir kaygı varsa TPH daha avantajlıdır. Ancak normalizasyon onemliyse ve maliyet katlanılabilirse TPT kullanılabilir. 
#endregion
#region TPT Nasıl Uygulanır?
//TPT'yi uygulayabilmek için öncelikle entitylerin kendi aralarında olması gereken mantıkta inşa edilmesi gerekmektedir.
//Entityler DbSet olarak bildirilmelidir.
//Hiyerarşik olarak aralarında kalıtımsal ilişki olan tüm entityler OnModelCreating fonksiyonunda ToTable metodu ile konfigüre edilmelidir.
//Böylece EF Core kalıtımsal ilişki olan bu tablolar arasında TPT davranışının olduğunu anlayacaktır.
//Bu konfigurasyondan sonra migration basılabilir.
#endregion
#region TPT'de Veri Ekleme
//Technician technician = new() { Name = "Şuayip", Surname = "Yıldız", Department = "Yazılım", Branch = "Kodlama" };
//await context.Technicians.AddAsync(technician);

//Customer customer = new() { Name = "Hilmi", Surname = "Celayir", CompanyName = "Çaykur" };
//await context.Customers.AddAsync(customer);
//await context.SaveChangesAsync();
#endregion
#region TPT'de Veri Silme
//Employee? silinecek = await context.Employees.FindAsync(3);
//context.Employees.Remove(silinecek);
//await context.SaveChangesAsync();

//Burada employee eklemedik lakin 3 id sine sahip bir veri var. Bu sebeple veri getirilir. Tpt de bir veri ne employee dur ne customer dir 
//ne de baska bir seydir. Verilerin tamamı bir butun gibi davranır. 3 id si ortak id oldugu icin 3 id sine sahip butunsel veriyi
//alır getirir. Veya Veriyi siler. Persondan elde edilip silinme yapılabilir. Anlamamız gereken veriler butunseldir ve oyle davranır.  

//Person? silinecekPerson = await context.Persons.FindAsync(1);
//context.Persons.Remove(silinecekPerson);
//await context.SaveChangesAsync();
#endregion
#region TPT'de Veri Güncelleme
//Technician technician = await con text.Technicians.FindAsync(2);
//technician.Name = "Mehmet";
//await context.SaveChangesAsync();

//Teknician dan veri yakalandı ve degistirildi. Employee dan devgistirilebilirdi. Veya Persondan degistirilebilirdi.
//Hepsine aynı isi gorurdu cunku veriler butunsel olarak davranır.

#endregion
#region TPT'de Veri Sorgulama
//Employee employee = new() { Name = "Fatih", Surname = "Yavuz", Department = "ABC" };
//await context.Employees.AddAsync(employee);
//await context.SaveChangesAsync();

//var technicians = await context.Technicians.ToListAsync();//2 technitians varsa 2 tane veri gelir.
//var employees = await context.Employees.ToListAsync();//1 employee varsa 3 veri gelir cunku technitians lar aynı zamanda employee dur.

//Yani polimorphizm kuralları burada gecerlidir. Bu sekilde dagıtılmıs verileri kumulatif yani birikimli olarak yine elde edebiliriz.
//Console.WriteLine();
#endregion



abstract class Person//Normalde abstract class uzerinden bir table olusturamıyoruz lakin Tpt abstract class uzerinden table olusturabilir.
{
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
        modelBuilder.Entity<Person>().ToTable("Persons");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Technician>().ToTable("Technicians");
        //Default kalıtımsal davranıs TPH dir. Burada TPH yi override edip TPT uygulamam icin bu sekilde bir konfigurasyon yapmam gerekir.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;" +
            "TrustServerCertificate=True");
    }
}