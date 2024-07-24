
using Microsoft.EntityFrameworkCore;
using System.Reflection;

ApplicationDbContext context = new();

#region Complext Query Operators

#region Join(Inner Join)

#region Query Syntax
//var query = from photo in context.Photos //context teki photos lar photo ile temsil edildi.
//            join person in context.Persons //context teki persons lar person ile temsil edildi ve join islemi yapıldı.
//                on photo.PersonId equals person.PersonId //join islemini photo daki personId ile person daki personId uzerindeki esitlikten yapıyoruz.
//            select new //select ile secme islemine geciyoruz. Bir anonim tur olusturuyoruz.
//            {
//                person.Name,
//                photo.Url
//            };//person ın name i ve photo nun url ini alıyoruz.
//query yapısı IQuerable olarak olusturuldu. Yani hala query yi gelistirme surecindeyim.

//var datas = await query.ToListAsync();//ilgili query yi execute ediyoruz.

#endregion

#region Method Syntax
//var query = context.Photos//photos tablosu secildi
//    .Join(context.Persons,//ilk parametrede join yapılacak tablo secildi.
//    photo => photo.PersonId,//ikinci parametrede ilk tablodaki hangi kolonla bu birlestirmeyi yapacaksak o bildirilir.
//    person => person.PersonId,//ucuncu parametrede ikinci tablodaki hangi kolonla bu birlestirmeyi yapacaksak o bildirilir.
//    (photo, person) => new //dorduncu paprametrede ise her iki tabloyu temsilen cift parametreli bir callBack function 
//    {
//        person.Name,
//        photo.Url
//    });//istenen veriler secilir.

//Bu sekilde hazır metotlar uzerinden de islem yapabiliriz.

//var datas = await query.ToListAsync();execute islemi yapılır.
#endregion

#region Multiple Columns Join
//Birden fazla kolon uzerinden yapılan join islemidir. Anonim yapılar kullanılır.

#region Query Syntax
//var query = from photo in context.Photos
//            join person in context.Persons
//                on new { photo.PersonId, photo.Url } equals new { person.PersonId, Url = person.Name }
//Burada kolonlar isimlerin aynılıgı uzerinden birlesitirilir. Burada Name ve Url in farklılıgından dolayı bu sebeple hata alınır.
//Bu sebeple Url=person.Name seklinde bir esitleme yapıldı.
//            select new
//            {
//                person.Name,
//                photo.Url
//            };
//var datas = await query.ToListAsync();
#endregion
#region Method Syntax
//var query = context.Photos
//    .Join(context.Persons,
//    photo => new //1.tablo
//    {
//        photo.PersonId,
//        photo.Url
//    },
//    person => new //2.tablo
//    {
//        person.PersonId,
//        Url = person.Name //isim aynılıgı burada da gecerlidir.
//    },
//    (photo, person) => new //2 tablo birlestiriliyor.
//    {
//        person.Name,
//        photo.Url
//    });

//var datas = await query.ToListAsync();
#endregion
#endregion

#region 2'den Fazla Tabloyla Join
//2 tablodan veri sorgulama sureci 3-4-5... tabloya genellenebilir.
#region Query Syntax
//var query = from photo in context.Photos
//            join person in context.Persons //2 tablo birlestirildi
//                on photo.PersonId equals person.PersonId
//            join order in context.Orders //3.tablo sorguya eklenerek birlestiriliyor.
//                on person.PersonId equals order.PersonId  
//            select new
//            {
//                person.Name,
//                photo.Url,
//                order.Description
//            };

//var datas = await query.ToListAsync();
#endregion
#region Method Syntax
//var query = context.Photos
//    .Join(context.Persons,
//    photo => photo.PersonId,
//    person => person.PersonId,
//    (photo, person) => new
//    {
//        person.PersonId,
//        person.Name,
//        photo.Url
//    })
//    .Join(context.Orders, //birlesitilen sorgu uzerine tekrardan join ile birlestirme yapıyoruz.
//    personPhotos => personPhotos.PersonId,
//    order => order.PersonId,
//    (personPhotos, order) => new
//    {
//        personPhotos.Name,
//        personPhotos.Url,
//        order.Description
//    });

//var datas = await query.ToListAsync();
#endregion
#endregion

#region Group Join (GroupBy Değil!)
//var query = from person in context.Persons
//            join order in context.Orders
//                on person.PersonId equals order.PersonId into personOrders 
//person lara karsılık orderları elde ettik. into ile burada order ları tekil degil grup olarak elde etmis oluyoruz. 
//            //from order in personOrders ile personOrders dan tekrar order ları alabiliriz. order i bu sekilde kullanıma alabiliriz.
//            Aklımızda olsun. Ancak bu durumda grup artık bir anlam ifade etmez. personOrders kullanılamaz. Alt satırlarda Order dan tekrardan veriler alınabilir.
//            select new
//            {
//                person.Name,
//order. ... ile order a erisemeyiz. Cunku artık order lar personOrders ile gruplandırıldılar.
//Artık elimizde bir koleksiyonel yapılanma var.
//                Count = personOrders.Count(), //bunun count unu elde edebiliriz.
//                personOrders//ya da direkt koleksiyonun kendisini elde edebiliriz.
//            };
//var datas = await query.ToListAsync();
#endregion
#endregion


#region Left Join 
//left join sadece query syntax ile kullanılır.

//DefaultIfEmpty : Sorgulama sürecinde ilişkisel olarak karşılığı olmayan verilere default değerini yazdıran yani LEFT JOIN sorgusunu
//yapan bir fonksiyondur.


//var query = from person in context.Persons
//            join order in context.Orders
//                on person.PersonId equals order.PersonId into personOrders //leftjoin icin personların order larını oncelikle grupladık.
//            from order in personOrders.DefaultIfEmpty() //daha sonra orderları personOrders dan alıyoruz.
////            Bos olanlar varsa da bunların default degerlerini getirmesini istiyourz. Bunu da .DefaultIfEmpty() ile yapıyoruz.
//            select new //Person a karsılık orderlardan bos olanların default degerini getir demek efcore acısından bir leftjoin operasyonudur.
//            {
//                person.Name,
//                order.Description
//            };

//var datas = await query.ToListAsync();
#endregion

#region Right Join
//right join yapmak icin left join yaptıgımız sorguda tabloların yerlerini degistirip leftjoin ini almak yeterlidir diye dusunuyoruz.
//Yapılan calısma yine left join dir lakin mantıken right join i elde etmis oluyoruz.

//var query = from order in context.Orders
//            join person in context.Persons
//                on order.PersonId equals person.PersonId into orderPersons
//            from person in orderPersons.DefaultIfEmpty()
//            select new
//            {
//                person.Name,
//                order.Description
//            };

//var datas = await query.ToListAsync();
#endregion

#region Full Join
//Burada da direkt bir fonksiyon yoktur. Mantıken bir calısma yapıyoruz. 
//Once left sonra right join yapılır ve bu ikisi birlestirilir. Bu sekilde full join elde edilir.

//var leftQuery = from person in context.Persons
//                join order in context.Orders
//                    on person.PersonId equals order.PersonId into personOrders
//                from order in personOrders.DefaultIfEmpty()
//                select new
//                {
//                    person.Name,
//                    order.Description
//                };


//var rightQuery = from order in context.Orders
//                 join person in context.Persons
//                     on order.PersonId equals person.PersonId into orderPersons
//                 from person in orderPersons.DefaultIfEmpty()
//                 select new
//                 {
//                     person.Name,
//                     order.Description
//                 };

//var fullJoin = leftQuery.Union(rightQuery);//Union ile sorguları birlestiririz.

//var datas = await fullJoin.ToListAsync();
#endregion

#region Cross Join
//var query = from order in context.Orders
//            from person in context.Persons //cross join icin direkt olarak from ile devam ediyorum.
//            //Efcore from uzerine from sorgusunu cross join olarak algılar.
//            select new
//            {
//                order,
//                person
//            };

//var datas = await query.ToListAsync();
#endregion

#region Collection Selector'da Where Kullanma Durumu
//var query = from order in context.Orders
//            from person in context.Persons.Where(p => p.PersonId == order.PersonId)
//Burada collection selector uzerinde bir where sartı kullanmıs olduk. Bu yapıda ise efcore bu sorguyu bir inner
//join olarak yorumlar ve ona gore bir davranıs generate eder. Sonrasında ona gore execute islemi yapılır.
//            select new
//            {
//                order,
//                person
//            };

//var datas = await query.ToListAsync();

//Bunu gostermemizin amacı bu durumda where kullanıldıgında davranısın nasıl sekillendigini gostermektir.
#endregion

#region Cross Apply
//Inner Join e benzer

//var query = from person in context.Persons
//            from order in context.Orders.Select(o => person.Name)
//              //Select fonksiyonu ile bir secme bildirimi yapacak olursam bunu efcore bir cross apply olarak algılar.

//            select new
//            {
//                person,
//                order
//            };

//var datas = await query.ToListAsync();
#endregion

#region Outer Apply
//Left Join e benzer

//var query = from person in context.Persons
//            from order in context.Orders.Select(o => person.Name).DefaultIfEmpty()
//            //Outher apply davranısı icin ise Cross Aplly ın DefaultIfEmpty ile generate edilmis olması gerekir.
//            select new
//            {
//                person,
//                order
//            };

//var datas = await query.ToListAsync();

//Cross Apply ve Outher Apply ileri sebiye veritabanı terimleri olup ileri seviye sorgulardır. Bunları efcore da bu sekilde yapıyoruz. 
#endregion
#endregion
Console.WriteLine();
public class Photo
{
    public int PersonId { get; set; }
    public string Url { get; set; }

    public Person Person { get; set; }
}
public enum Gender { Man, Woman }
public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    public Gender Gender { get; set; }

    public Photo Photo { get; set; }
    public ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }

    public Person Person { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Photo>()
            .HasKey(p => p.PersonId);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Photo)
            .WithOne(p => p.Person)
            .HasForeignKey<Photo>(p => p.PersonId);

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
        //iliskiler bu sekilde modellendi.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}