
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

ApplicationDbContext context = new();

#region Lazy Loading Nedir?
//Navigation property'ler üzerinde bir işlem yapılmaya çalışıldığı taktirde ilgili propertynin/ye temsil ettiği/karşılık gelen
//tabloya özel bir sorgu oluşturulup execute edilmesini ve verilerin yüklenmesini sağlayan bir yaklaşımdır.
//Hasılı tum veriler bellege yuklenmez. Tetiklendikce yuklenirler.
#endregion

//var employee = await context.Employees.FindAsync(2);//employee lar sorgulandı. Bu employee a baglı orders ya da region lar getirilmedi.

//Console.WriteLine(employee.Region.Name);//Ancak burada region in bazı verilerini almak istedik. Lakin burada patlama olur.

//Hata alınmasının sebebi employee daki region null dır ve name ine bu nedenle ulasılamaz.
//Iste byu yaklasım durumunda lazy loading yaklasımını kullanmamız gerekir.
//Lazy loading kullanıldıgı zaman employee.Region denildiginde yani ilgili navigation property kullanıldıgı anda ilgili sorguyu olusturup
//db ye gider. Veriyi alır gelir. Bellege yukler. Daha sonra .Name ile name ini bize getirebilir. 

#region Prox'lerle Lazy Loading
//Lazy laodign in kullanılabilmesi icin genellikle proxy yapılanması kullanılır.
//Microsoft.EntityFrameworkCore.Proxies seklinde bir kutuphane mevcuttur. Bu kutuphanede proxy yapılanması mevcuttur.
//Bu kutuphane uygulamaya dahil edildikten sonra OnConfiguring metodunda konfigurasyon yapmamız gerekir.

//optionsBuilder
//    .UseLazyLoadingProxies()
//    .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");

//veya connection string vs verildikten sonra 

//optionsBuilder.UseLazyLoadingProxies(); 

//seklinde de tekil cagrılabilir.

//Bu sekilde configuration ile uygulamamız lazy loading operasyonlarını destekler nitelikte olur.
//Artık lazy loading yaklasımı enable yani aktif hale gelmis olur.

//Bundan sonra 

//var employee = await context.Employees.FindAsync(2);
//Console.WriteLine(employee.Region.Name);

//seklinde name i elde edebiliriz sanabiliriz. Ancak bu da yeterli degildir. Proxy uzerinden lazy loading yapacaksak eger Nav Prop virtual 
//olarak isaretlenmesi gerekir.

#region Property'lerin virtual Olması   
//Eğer ki proxler üzerinden lazy loading operasyonu gerçekleştiriyorsanız Navigtation Propertylerin virtual ile
//işaretlenmiş olması gerekmektedir. Aksi taktirde patlama meydana gelecektir.

//public virtual List<Order> Orders { get; set; }
//public virtual Region Region { get; set; }

//bu sekilde butun nav prop virtul ile isaretlenir.

//var employee = await context.Employees.FindAsync(2);
//Console.WriteLine(employee.Region.Name);

//sorgusu yapıldıgında ise Region lara dolayısıyla name e ulasilabilir.

/*!!!!!!!!!!!!!!!!!!    Not                    !!!!!!!!!!!!!!!!!!!!!!!!!!!
 Biz
//var employee = await context.Employees.FindAsync(2);
seklinde bir sorgu yapmıs olalım. Burada halihazırda employee sorgusu atıldı ve employee lar geldi. Lakin orders ve region lar su anda 
yoklar. Ancak biz debug da employee un uzerine geldigimizde orders i ve region ları goruruz. Bu bizi yanıltabilir.
Bunun sebei sudur:
Biz daha employee nesnesinin uzeirne gelip property lerini actıgımızda dahi arkada region ve order sorguları atılır. O an atılır ve bize 
ilgili veriler getirilir. Bu veriler nesnenin uzerine geldigimizde getirildiler. 
Bunun sebebi nav property lerin virtual oldugundan dolayı yuklenme kararlarının runtime da alınmasıdır. Run time da o verilerin 
goruntulenmesinin istenmesi dahi sorguların atılmasına yeterlidir.
 */
#endregion
#endregion

#region Proxy Olmaksızın Lazy Loading
//Prox'ler tüm platformlarda veya operasyonlarda desteklenmeyebilir.
//Böyle bir durumda manuel bir şekilde lazy loading'i uygulamak mecburiyetinde kalabiliriz. Bunu da 2 farklı yontemle yapabiliriz.

//Manuel yapılan Lazy Loading operasyonlarında Navigation Proeprtylerin virtual ile işaretlenmesine gerek yoktur!
//Virtual kaldırıldıgı icin 

//.UseLazyLoadingProxies(false)

//seklinde ilgili konfigurasyon false a cekilmelidir. Proxy tabanlı lazy loading disabled hale getirilmis oldu.

#region ILazyLoader Interface'i İle Lazy Loading
//Microsoft.EntityFrameworkCore.Abstractions

//kutuphanesiyle ilgili interface e ulasılabilir.

/*Daha sonra manuel bir sekilde lazy loading i kullanacagımız entity lere parametresiz olan ctor larının dısında ILazyLoader tipinden 
 parametre alan bir ctor ını insaa etmemiz gerekir.*/

//var employee = await context.Employees.FindAsync(2);
#endregion
#region Delegate İle Lazy Loading
//Herhangi bir kutuphaneye vs ihtiyac duymadan amele usulu lazy loading yapmak istiyorsak programlama dilinde bulunan delegate 
//ozelliklerini kullanabiliriz. Operasyonlar entity lerde yapılmıstır.

//var employee = await context.Employees.FindAsync(2);
//Bu sekilde employee lar uzerinden ilgili ihtiyac duyulan nav member lar lazy olarak yuklenirler.
#endregion
#endregion

#region N+1 Problemi
//var region = await context.Regions.FindAsync(1);//region lar elde edildi
//foreach (var employee in region.Employees)//region lar icindeki employee larda donuyoruz.
//{
//    var orders = employee.Orders;//employee lardaki order lar elde edildiler.
//    foreach (var order in orders)//order larda donuyoruz.
//    {
//        Console.WriteLine(order.OrderDate);
//    }
//}

/*SQL Profiler dan bakarsak eger once region lar sorgulanır,sonra employee sorgulanır, sonra order lar sorgulanır.
 Ancak her dongude employee lara bir sorgu atılır. Aynısı order icinde gecerli olur. 
Bu lazyLoading in maliyetli oldugu anlamına gelir.
Bu tarz calısmalarda lazy loading kullanılırsa her dongude bir sorgu atılır. Burada olusan maliyet ise N+1 problemi olarak 
degerlendirilmiştir. Bu sebeple cogu zaman lazy loading operasyonunun kullanılmaması onerilir. 
Lazy Loading mumkun mertebe kacınılması gereken bir veri yukleme operasyonudur. Cunku dedigimiz gibi her bir nav prop tetiklenmesine 
karsılık arka planda bir sorgu yapılır.
Bu probleme cozum yoktur. Ya lazy loading mumkun mertebe kullanılmamalıdır ya da veri sorgulama sureclerinde bu tip sureklı 
lazy sekilde sorgu yapacak olan dongusel yapılanmalardan kacınmak gerekir.

Hasılı

Lazy Loading, kullanım açısından oldukça maliyetli ve performans düşürücü bir etkiye sahip yöntemdir. O yüzden kullanırken mümkün mertebe 
dikkatli olmalı ve özellikle navigation propertylerin döngüsel tetiklenme durumlarında lazy loading'i tercih etmemeye odaklanmalıyız. 
Aksi taktirde her bir tetiklemeye karşılık aynı sorguları üretip execute edecektir. 
Bu durumu N+1 Problemi olarak nitelendirmekteyiz.
Mümkün mertebe, ilişkisel verileri eklerken Lazy Loading kullanmamaya özen göstermeliyiz.
 */

#endregion


Console.WriteLine();

#region Proxy İle Lazy Loading
public class Employee
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
    public virtual List<Order> Orders { get; set; }
    public virtual Region Region { get; set; }
}
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public virtual Employee Employee { get; set; }
}
#endregion

#region ILazyLoader Interface'i İle Lazy Loading

//public class Employee
//{
//    ILazyLoader _lazyLoader; //bir ILazyLoader referansı olusturuldu.
//    Region _region;//lazy loading i employee nesnesi uzerinden region da kullanacaz. Bir referans olusturuldu. Nav prop de ilgili operasyon yapılır.
//    public Employee() { }
//    public Employee(ILazyLoader lazyLoader) //ctor a gelen nesne 
//        => _lazyLoader = lazyLoader; //ustteki nesneye verildi.
//    public int Id { get; set; }
//    public int RegionId { get; set; }
//    public string? Name { get; set; }
//    public string? Surname { get; set; }
//    public int Salary { get; set; }
//    public List<Order> Orders { get; set; }
//    public Region Region
//    {
//        get => _lazyLoader.Load(this, ref _region);//_lazyLoader uzerinden load operasyonu this ile bu entity uzerinde _region referansına yapıldı.
//        set => _region = value; //gelen degeri referansa esitliyoruz.
//    }

//}
//public class Region
//{
//    ILazyLoader _lazyLoader;
//    ICollection<Employee> _employees;//Nav Prop turu ne ise buradaki referansın turu de aynı turden olmalıdır.
//    public Region() { }
//    public Region(ILazyLoader lazyLoader)
//        => _lazyLoader = lazyLoader;
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public ICollection<Employee> Employees
//    {
//        get => _lazyLoader.Load(this, ref _employees);
//        set => _employees = value;
//    }
//}
//public class Order
//{
//    public int Id { get; set; }
//    public int EmployeeId { get; set; }
//    public DateTime OrderDate { get; set; }
//    public Employee Employee { get; set; }
//}

#endregion
#region Delegate İle Lazy Loading
//public class Employee
//{
//    Action<object, string> _lazyLoader; //ref tanımı
      
      /*Action turu alacaz. Bu tur bir obje ve bir string beklesin. Buradaki obje lazyloading e taabi tutacagımız entity nin turunu 
       bildirirken string ise lazy loading e taabi tutulacak nav prop nin ismini bildirir.*/

//    Region _region;
//    public Employee() { }
//    public Employee(Action<object, string> lazyLoader)//ref bekleniyor
//        => _lazyLoader = lazyLoader;//ref gelen delegate ile dolduruluyor.
//    public int Id { get; set; }
//    public int RegionId { get; set; }
//    public string? Name { get; set; }
//    public string? Surname { get; set; }
//    public int Salary { get; set; }
//    public List<Order> Orders { get; set; }
//    public Region Region
//    {
//        get => _lazyLoader.Load(this, ref _region);
//        set => _region = value;
//    }
//}
//public class Region
//{
//    Action<object, string> _lazyLoader;
//    ICollection<Employee> _employees;
//    public Region() { }
//    public Region(Action<object, string> lazyLoader)
//        => _lazyLoader = lazyLoader;
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public ICollection<Employee> Employees
//    {
//        get => _lazyLoader.Load(this, ref _employees);
//        set => _employees = value;
//    }
//}
//public class Order
//{
//    public int Id { get; set; }
//    public int EmployeeId { get; set; }
//    public DateTime OrderDate { get; set; }
//    public Employee Employee { get; set; }
//}

//static class LazyLoadingExtension
//{
    
    /*load fonksiyonu delegate icin gecerli degildir. Bu sebeple delegate e bir extension fonksiyon yazdım.
    Extension fonk yapıları herhangi bir ture ek olarak fonk veya operasyonel bir yapı yazmak istedigimiz zaman yazılır. 
    Bu ileri duzey programlamanın bir konusudur.*/

//    public static TRelated Load<TRelated>(this Action<object, string> loader, object entity, ref TRelated navigation, [CallerMemberName] string navigationName = null)
//    {
            //callerMemberName ilgili yapının hangi member dan cagrıldıgını tutar. Bu attribute ile ilgili parametreye ilgili member ismi 
            //otomatik olarak verilir. CallerMemberName in bir default degeri olmalıdır. Bu default degere ben null degerini vermis oldum.

//        loader.Invoke(entity, navigationName);//load fonk uzerinden action burada tetiklenmis oluyor.
//        return navigation; //lazy loading e taabi tutulan navigation hangisi ise bunu da geri donmemiz gerekiyor. 

//    }
//}
#endregion

class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");

        //optionsBuilder.UseLazyLoadingProxies();
    }
}