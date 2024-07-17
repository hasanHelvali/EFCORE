using Loading_Related_Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();
#region Loading Related Data

#region Eager(Istekli) Loading
//Eager loading, generate edilen bir sorguya ilişkisel verilerin parça parça eklenmesini sağlayan ve bunu yaparken
//iradeli/istekli bir şekilde yapmamızı sağlayan bir yöntemdir.

#region Include
//Eager loading operasyonunu yapmamızı sağlayan bir fonksiyondur. Eager Loading uretilen sorguya join islemi uygular. 
//Yani üretilen bir sorguya diğer ilişkisel tabloların dahil edilmesini sağlayan bir işleve sahiptir..

//var employees = await context.Employees.ToListAsync();
//employee lar bize getirildi.

//var employees = await context.Employees.Include("Orders").ToListAsync();//text ile
//var employees = await context.Employees.Include(e=>e.Orders).ToListAsync();//tip guvenligi ile
//Bu iki sekilde orders sorguya dahil edilebilir. İkiside arka planda bir join islemi yapar.
/*Burada bir dipnot gecelim. ToList i Include dan once calıstırmamın sebebi ToList sorguyu yapar. Sorgu yapıldıktan sonra include 
 * dememin bir anlamı yoktur. Sorguyu nihai haline getirdikten sonra son sorguyu calıstırmak icin toList i kullanırız. Yani IEnumerable dan
 Kısacası include u kullanmak istiyorsam hala sorgu bitmemistir. Bu bitmeyen sorgu yuzunden toList ten sonra include u 
kullanmıyorumç ToList e geceriz. 
Bunu su sekilde de ozetleyebiliriz. DBSet ve IQuerable uzerinde calısmamızı yaparız. IQuerable neticede bir sorgudur ve 
calısma bitince bu sorgu calıstırılır. Bu da ek bir bir bilgi olsun. */

//var employees = await context.Employees
//    .Include(e => e.Region)//region eager loading ile dahil edildi.
//    .Where(e => e.Orders.Count > 2)
//    .Include(e => e.Orders)//orders eager loading ile dahil edildi.
//    .ToListAsync();
//Dikkat edilirse order dahil edilmeden once order a sart konuldu. EFCore bunu anlar ve sorguyu ona gore duzenler. Bir sorun yok.

//SQL Profiler dan yapılan sorgulara bakılıp join yapıları gorulebilir.

#endregion
#region ThenInclude
//ThenInclude, üretilen sorguda Include edilen tabloların ilişkili olduğu diğer tablolarıda sorguya ekleyebilmek için kullanılan bir
//fonksiyondur. 
//Eğer ki, üretilen sorguya include edilen navigation property koleksionel bir propertyse işte o zaman bu property üzerinden
//diğer ilişkisel tabloya erişim gösterilememektedir. Böyle bir durumda koleksiyonel propertylerin türlerine erişip,
//o tür ile ilişkili diğer tablolarıda sorguya eklememizi sağlayan fonksiyondur.

//var orders = await context.Orders
//    .Include(o => o.Employee)
//    .Include(o => o.Employee.Region)
//    .ToListAsync();

//var orders = await context.Orders
//    .Include(o => o.Employee.Region)
//    .ToListAsync();

//Bu ustteki iki kodda aynı isi yapar. Ikinci kod icin hem employee hem de region getirilmis olur. 

//order employee ile employee ise region ile iliskili oldugunda orderdan region a ulasmak mumkundur. Bu operasyonda bir sorun da yoktur.
//Lakin buradaki mevzu employee region ile region da order ile tekil nav prop ile baglıdır. Bu sebeple buradaki sorguyu yapabiliyoruz. Peki
//bunu tersine cevirseydik ne olurdu?

//var regions = await context.Regions.Include(r => r.Employees...)...
//seklinde bir sorgu yapsaydık r.Employee. dedigimizde bize koleksiyonel bir yapı gelir ve Add() ToList gibi fonksiyonlar aktif olur.
//Yani region ile employee birbirine tekil bir nav prop ile baglanmadıgı icin burada bu islemi yapamıyoruz. Istye burada ThenInclude ise 
//yarıyor. 

//var regions = await context.Regions
//    .Include(r => r.Employees) //Burada donen yapı koleksiyonel. Bu koleksiyonel employee dan orders a ulasmak icin ThenInclude kullanılır.
//        .ThenInclude(e => e.Orders)
//    .ToListAsync();

#endregion
#region Filtered Include
//Sorgulama süreçlerinde Include yaparken sonuçlar üzerinde filtreleme ve sıralama gerçekleştirebilmemiz isağlayan bir özleliktir.

//var regions = await context.Regions
//    .Include(r => r.Employees.Where(e => e.Name.Contains("a")).OrderByDescending(e => e.Surname))
//    .ToListAsync(); 
//Iste bu calısma filteredInclude olarak degerlendirilir.

//Include Icerisinde Desktelenen Fonksiyon : Where, OrderBy, OrderByDescending, ThenBy, ThenByDescending, Skip, Take

//Change Tracker'ın aktif olduğu durumlarda Include ewdilmiş sorgular üzerindeki filtreleme sonuçları beklenmeyen olabilir.
//Bu durum, daha önce sorgulanmş ve Change Tracker tarafından takip edilmiş veriler arasında filtrenin gereksinimi dışında
//kalan veriler için söz konusu olacaktır. Bundan dolayı sağlıklı bir filtred include operasyonu için change tracker'ın kullanılmadığı
//sorguları tercih etmeyi düşünebilirsiniz.

#endregion
#region Eager Loading İçin Kritik Bir Bilgi
//EF Core, önceden üretilmiş ve execute edilerek verileri belleğe alınmış olan sorguların verileri, sonraki sorgularda KULLANIR!

//var orders = await context.Orders.ToListAsync();//order tablosu icin bir sorgu calıstırıldı ve bellege alındı.

//var employees = await context.Employees.Include(e=>e.Orders)ToListAsync();//Buna gerek kalmadı.

//var employees = await context.Employees.ToListAsync();//aynı islem burada Employee icin yapıldı. Bu sorguda orderlarda elde edilir.
/*Burada gelen employee lara baktıgımız zaman employee ların orderlarının da getirildigini de goruruz. Ancak biz Orderları bu sorguda 
 getirmek icin bir sorgu yazmadık. EFCore onceden order ları bellege aldıgı icin burada orderları kullandı. Bu mikro duzeyde 
muthis bir optimizasyondur.
Onceden bir veriyi sorgulayıp bellege aldıgımızdan eminsek eger daha sonradan o tabloyu include etmemize gerek kalmadıgını bilmemiz gerekir.
Include edebiliriz sorun yok ama etmezsek de sorun yok cunku o veri bellekte sorguya bunu dahil etmedigimiz icin 
performans artısı elde etmis oluruz. */

#endregion
#region AutoInclude - EF Core 6
//Uygulama seviyesinde bir entitye karşılık yapılan tüm sorgulamalarda "kesinlikle" bir tabloya Include işlemi gerçekleştirlecekse
//eğer bunu her bir sorgu için tek tek yapmaktansa merkezi bir hale getirmemizi sağlayan özelliktir.
//Bu islem context nesnesinde onModelCreating de yapılır.

//var employees = await context.Employees.ToListAsync();
//Yapılan konfigurasyon sonrasında bu sorgu sonucunda employees verisine karsılık region larda bize getirilmis olur.
#endregion
#region IgnoreAutoIncludes
//AutoInclude konfigürasyonunu sorgu seviyesinde pasifize edebilmek için kullandığımız fonksiyondur.

//var employees = await context.Employees.IgnoreAutoIncludes().ToListAsync();
#endregion

#region Birbirlerinden Türetilmiş Entity'ler Arasında Include
//Onceden gordugumuz Entity icin Inheritance konusunun TPH TPT VE TPCT turleri icin Include kullanımından soz ediyoruz.
//Burada default davranıs olan TPH icin konuyu işleyelim.
#region Cast Operatörü İle Include
var persons1 = await context.Persons.Include(p => ((Employee)p).Orders).ToListAsync();
//her employee aynı zamanda bir person dır cunku buna gore bir kalıtım insaa edildi. Bu sebeple burada cast operatorunu kullanıp Employee a
//donusturdugumuz yapı uzerinden orders lara ulasabiliriz.
#endregion
#region as Operatörü İle Include
var persons2 = await context.Persons.Include(p => (p as Employee).Orders).ToListAsync();
//aynı islemi as ile yapabiliriz. 
#endregion
#region 2. Overload İle Include
var persons3 = await context.Persons.Include("Orders").ToListAsync();
//Gelen veri person dır ancak her employee da persondır. Burada gelen veri employee diye dusunursek buradan Orders lara ulasabiliriz.
//Yani gelen employee verisine orders ı bu sekilde text vererek include edebiliriz.
#endregion
#endregion


Console.WriteLine();
#endregion
#endregion



public class Person
{
    public int Id { get; set; }

}
public class Employee : Person
{
    //public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }

    public List<Order> Orders { get; set; }
    public Region Region { get; set; }
}
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }

    public Employee Employee { get; set; }
}


class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Employee>()
            .Navigation(e => e.Region)
            .AutoInclude();
        //Auto include burada bu sekilde yapılır. Ben her employee sorguladıgımda kesinlikle region bekliyorsam burada bunu yapabilirim. 
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}