using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Index Nedir?
//Index, bir sütuna dayalı sorgulamaları daha verimli ve performanslı hale getirmek için kullanılan yapıdır.
#endregion
#region Index'leme Nasıl Yapılır?
//PK, FK ve Alternate Key yani AK olan kolonlar otomatik olarak indexlenir. 
//context.Employees.Where(e => e.Name == "") seklindeki sorgular index sebebiyle artık daha az maliyetli olur.

#region Index Attribute'u

#endregion
#region HasIndex Metodu

#endregion
#endregion
#region Composite Index
//context.Employees.Where(e => e.Name == "" || e.Surname == "")
//Name ve surname in beraber sorgulanması gereksin. Bu sekilde sorgularda name indexlendi ve surname indexlenmediyse sorgunun
//maliyeti artar. Bu sebeple ilgili yapıların composite olarak indexlenmesi gerekir.
//Bu yapıda name ve surname ayrı ayrı sorgulanırsa composite yapıdan istifade edilemez. Cunku bu iki prop composite yani birlesik bir 
//yapıda indexlendi. Buradaki indexlemenin dusuk maliyetinden faydalanmak icin name ve surname in beraber sorgulanması gerekir.
//Tum durumlarda indexlemenin dusuk maliyetini kullanmak istiyorsak hem name, hem surname, hem name ve surname composite indexlemesi
//yapılabilir. Lakin tabii ki indexlemenin de bir maliyeti vardır. Bu sebeple cok yogun sorgulama yapılan kolonlarda indexleme yapılması
//gerekir. Yani indexlemeyi bol keseden sacmıyoruz. Tum kolonları indexlemek en dogru yaklasım olsaydı indexleme default bir davranıs olurdu.
#endregion
#region Birden Fazla Index Tanımlama

#endregion
#region Index Uniqueness
//indexler unique degildir. Default halinde mukerrer olabilir. Unique olabilmesi icin 
//[Index(nameof(Name), IsUnique = true)]
//seklinde bir yapılandırma gerekir. FluentAPI karsılıgında ise HasIndex ten sonra IsUnique seklinde bir fonksiyon cagrılarak aynı
//konfigurasyon yapılabilir.

#endregion
#region Index Sort Order - Sıralama Düzeni (EF Core 7.0)
//Indexlerde sıralamalar yapılabilir. Default sıralama asc dir. Ama biz bunu desc hale getirebiliriz.
#region AllDescending - Attribute
//Tüm indexlemelerde descending davranışının bütünsel olarak konfigürasyonunu sağlar.
#endregion
#region IsDescending - Attribute
//Indexleme sürecindeki her bir kolona göre sıralama davranışını hususi ayarlamak istiyorsak kullanılır.
#endregion
#region IsDescending Metodu
//Fluent api fonksiyonudur.
#endregion
#endregion
#region Index Name

#endregion
#region Index Filter

#region HasFilter Metodu
//fleunt api de kullanılır. Attribute de filter kullanamıyoruz.
#endregion
#endregion
#region Included Columns

#region IncludeProperties Metodu
//Attribute kullanımı yoktur. Fluent api ile konfigure edilir.

#endregion
#endregion



//[Index(nameof(Name))] //Name kolonuna burada bir index yapısı atanmıs oldu.
//[Index(nameof(Surname))]
//[Index(nameof(Name), nameof(Surname))]//composite index bildirimi
//[Index(nameof(Name), IsUnique=true)]
//[Index(nameof(Name), AllDescending = true)]
//[Index(nameof(Name), nameof(Surname), IsDescending = new[] { true, false })] //name desc, surname asc olmus oldu.
//[Index(nameof(Name), Name = "name_index")]//index e isim verdik.
class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Employee>()
        //.HasIndex(x => x.Name);
        //.HasIndex(x => new { x.Name, x.Surname });
        //.HasIndex(nameof(Employee.Name), nameof(Employee.Surname));
        //.HasIndex(x => x.Name)
        //.IsUnique();

        //modelBuilder.Entity<Employee>()
        //    .HasIndex(x => x.Name)
        //    .IsDescending();

        //modelBuilder.Entity<Employee>()
        //    .HasIndex(x => new { x.Name, x.Surname })
        //    .IsDescending(true, false);

        //modelBuilder.Entity<Employee>()
        //    .HasIndex(x => x.Name)
        //    .HasDatabaseName("name_index");//index e isim verdigimiz fluent api fonksiyonudur.

        //modelBuilder.Entity<Employee>()
        //    .HasIndex(x => x.Name)
        //    .HasFilter("[NAME] IS NOT NULL");
        //Burada bu filter a uymayan degerler index tablosuna alınmazlar.

        modelBuilder.Entity<Employee>()
            //.HasIndex(x => x.Name)//Attributre dısında bu sekilde de bildirme yapılabilir. Name property sine bir index atandı.
            //.HasIndex(x => new { x.Name, x.Surname })//composite index bildirimi
            .HasIndex(nameof(Employee.Name),nameof(Employee.Surname))//diger bir cesit composite index bildirimi
            .IncludeProperties(x => x.Salary);
        //Ilgili composite index bildirimi kullanılırken salary de index tablosuna bu konfigurasyon ile eklenir ve sorgunun maliyeti
        //artmamıs olur.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}