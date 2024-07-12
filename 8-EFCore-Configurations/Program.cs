using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region EF Core'da Neden Yapılandırmalara İhtiyacımız Olur?
//Default davranışları yeri geldiğinde geçersiz kılmak ve özelleştirmek isteyebiliriz. Bundan dolayı yapılandırmalara ihtiyacımız olacaktır.
#endregion

#region OnModelCreating Metodu
//EF Core'da yapılandırma deyince akla ilk gelen metot OnModelCreating metodudur.
//Bu metot, DbContext sınıfı içerisinde virtual olarak ayarlanmış bir metottur.
//Bizler bu metodu kullanarak model'larımızla ilgili temel konfigürasyonel davranışları(Fluent API) sergileyeibliriz.
//Bir model'ın oluşturulmasıyla ilgili tüm konfigürasyonları burada gerçekleştirebilmekteyiz.

#region GetEntityTypes
//EF Core'da kullanılan entity'leri elde etmek, programatik olarak öğrenmek istiyorsak eğer GetEntityTypes fonksiyonunu kullanabiliriz.
//OnModelCreating override inda ilgili kod yazılmısltır.
#endregion

#endregion

#region Configurations | Data Annotations & Fluent API

#region Table - ToTable
//Generate edilecek tablonun ismini belirlememizi sağlayan yapılandırmadır.
//Ef Core normal şartlarda generate edeceği tablonun adını DbSet property'sinden almaktadır.
//Bizler eğer ki bunu özelleştirmek istiyorsak Table attribute'unu yahut ToTable api'ını kullanabilriiz.


/*[Table("Kisiler")]
class Person
{
...
seklinde kullanılır. Artık db ye kaydedilecek tablonun ismi dbset tanımından degil Table attribute unden alır.*/

/*modelBuilder.Entity<Person>().ToTable("aksdmkasmdk");
seklinde de fluent api da table ismi belirlenebilir.

 Konfigurasyon uygularken en baskın konfigurasyon fluent api konfigurasyonlarıdır.*/
#endregion

#region Column - HasColumnName, HasColumnType, HasColumnOrder
//EF Core'da tabloların kolonları entity sınıfları içerisindeki property'lere karşılık gelmektedir. 
//Default olarak property'lerin adı kolon adıyken, türleri/tipleri kolon türleridir.
//Eğer ki generate edilecek kolon isimlerine ve türlerine müdahale etmek istiyorsak bu konfigürasyon kullanılır.

/* Name seklinde bir property nin ızerine [Column("Adi", TypeName = "metin", Order = 7)] seklinde bir
konfigurasyon soz konusu ise ilgili kolonun yeni ismi adi yeni tipi ise metin seklinde bir tip olur. Olusturulacak kolonlara bu sekilde
müdahale edebiliyoruz. Order ile de kolonun sırasına müdahale ederiz. İlgili entity de pratik mevcuttur.

 Aynı sekilde fluent api da bir konfigurasyon yapmak isityorsak 
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Name)
        //    .HasColumnName("Adi")
        //    .HasColumnType("asldalsd")
        //    .HasColumnOrder(7);
seklinde yapabiliriz. 
*/

#endregion

#region ForeignKey - HasForeignKey
//İlişkisel tablo tasarımlarında, bağımlı tabloda esas tabloya karşılık gelecek verilerin tutulduğu kolonu foreign key olarak
//temsil etmekteyiz.
//EF Core'da foreign key kolonu genellikle Entity Tnaımlama kuralları gereği default yapılanmalarla oluşturulur.
//ForeignKey Data Annotations Attribute'unu direkt kullanabilirsiniz. Lakin Fluent api ile bu konfigürasyonu yapacaksanız iki entity
//arasındaki ilişkiyide modellemeniz gerekmektedir. Aksi taktirde fluent api üzerinde HasForeignKey fonksiyonunu kullanamnazsınız!

/*
    [ForeignKey(nameof(Department))]
    public int DId { get; set; }
    Bu sekilde bir DId property yapısı olsun. Bu property default convention ın dısındadır. Bunu yazmasak da iliskiden dolayı shadow 
    property zaten olusturulur. Ama kendi olusturdugumuz bir isimle bir foreing key property si olusturmak istiyorsak yukarıda goruldugu 
    gibi attribute kullanımı yapmamız gerekir.

    Bunun fluent konfigurasyonu ise 

    modelBuilder.Entity<Person>()
    .HasOne(p => p.Department)
    .WithMany(d => d.Persons)
    .HasForeignKey(p => p.DId);

    seklinde olur.

 */

#endregion

#region NotMapped - Ignore
//EF Core, entity sınıfları içerisindeki tüm proeprtyleri default olarak modellenen tabloya kolon şeklinde migrate eder.
//Bazn bizler entity sınıfları içerisinde tabloda bir kolona karşılık gelmeyen propertyler tanımlamak mecburiyetinde kalabiliriz.
//Bu property'lerin ef core tarafından kolon olarak map edilmesini istemediğimizi bildirebilmek için NotMapped ya da Ignore kullanabiliriz.

//Yazılımsal amaçla oluşturduğum bir property
//[NotMapped]
//public string Laylaylom { get; set; }


//modelBuilder.Entity<Person>()
//    .Ignore(p => p.Laylaylom);

//seklinde kullanılırlar. 
#endregion

#region Key - HasKey
//EF Core'da, default convention olarak bir entity'nin içerisinde Id, ID, EntityId, EntityID vs. şeklinde tanımlanan tüm proeprtylere
//varsayılan olarak primary key constraint uygulanır.
//Key ya da HasKey yapılanmalarıyla istediğinmiz her hangi bir proeprty'e default convention dışında pk uygulayabiliriz.
//EF Core'da bir entity içerisinde kesinlikle PK'i temsil edecek olan property bulunmalıdır. Aksi taktirde EF Core migration
//oluştururken hata verecektir. Eğer ki tablonun PK'i yoksa bunun bildirilmesi gerekir. 

//[Key]
//public int a { get; set; }

//veya 

//modelBuilder.Entity<Person>()
//    .HasKey(p => p.Id);

//seklinde kullanılır.
#endregion

#region Timestamp - IsRowVersion
//Bu pratikte bir satırdaki verinin bütünsel olarak değişikliğini takip etmemizi sağlayacak olan verisyon mantığını konusabiliriz.
//İşte bir verinin verisyonunu oluşturmamızı sağlayan yapılanma bu konfigürasyonlardır.
//Verinin versiyonunu tutuyoruz gibi dusunulebilir.

//[Timestamp]
//[Comment("Bu şuna yaramaktadır...")]
//public byte[] RowVersion { get; set; }
//seklinde olabilir.

Person p = new();
p.Department=new(){
    Name = "Yazılım Departmanı"
};
p.Name = "Falanca";
p.Surname= "Filanca";
context.Persons.AddAsync(p);
context.SaveChangesAsync();
//seklinde bir data olusturup kaydettik. Bu durumda timestamp kolonuna bir binary data gelir.
var person = await context.Persons.FindAsync(1);//burada persona alınır ve versiyon bilgisinde 209 gibi iki sayı bulunur.
person.Name = "Falanca2";//bir update islemi yaptık.
await context.SaveChangesAsync();//tekrardan kayıt yaptık.

person = await context.Persons.FindAsync(1);//burada persona alınır ve versiyon bilgisinde 209 gibi iki sayı bulunur.
                                            //burada ise person in versiyon bilgisi 210 oldu.
                                            //Bu sekilde verilere verisyon kazandırmak istiyorsak TimeStamp i kullanabiliriz.

//Ya da Fluent api uzeirnden 

//modelBuilder.Entity<Person>()
//    .Property(p => p.RowVersion)
//    .IsRowVersion();
//seklinde bir calısma da yapabiliriz. Dolayısyla RowVersion kolonunun bir versiyonlama kolonu oldugunu efcore da konfigure etmis oluruz.

#endregion

#region Required - IsRequired
//Bir kolonun nullable ya da not null olup olmamasını bu konfigürasyonla belirleyebiliriz.
//EF Core'da bir property default oalrak not null şeklinde tanımlanır. Eğer ki property'si nullable yapmak istyorsak türü üzerinde
//?(nullable) operatörü ile bbildirimde bulunmamız gerekmektedir.

//[Required()]
//public string? Surname { get; set; } 

//seklinde veya fluent api ile 

//modelBuilder.Entity<Person>()
//    .Property(p => p.Surname).IsRequired();

//seklinde kullanılabilir. Ilgili kolon null olabilecek bir kolon ise 

//public string? Surname { get; set; } 

//seklinde ? ile kullanmak yeterlidir. 
#endregion

#region MaxLenght | StringLength - HasMaxLength
//Bir kolonun max karakter sayısını belirlememizi sağlar.

//[MaxLength(13)]
//public string? Surname { get; set; }

//seklinde veya fluent api ile 

//modelBuilder.Entity<Person>()
//    .Property(p => p.Surname)
//    .HasMaxLength(13);

//seklinde kullanılabilir.

//[StringLength(14)]
//public string? Surname { get; set; }

//seklinde stringlength te kullanılabilir. Hemen hemen aynı seydir.
#endregion

#region Precision - HasPrecision
//Küsüratlı sayılarda bir kesinlik belirtmemizi ve noktanın hanesini bildirmemizi sağlayan bir yapılandırmadır.

//[Precision(5, 3)]
//public decimal Salary { get; set; }

//seklinde kulllanılabilir. Bu kusuratla beraber en fazla 5 sayı ve kusuratın da 3 sayıdan olusacagını belirtir.
//12.345678 gibi bir veri giderse 12.345 seklinde bir veri kaydedilir.

//fleunt api da ise 

//modelBuilder.Entity<Person>()
//    .Property(p => p.Salary)
//    .HasPrecision(5, 3);

//seklinde kullanılabilir.

#endregion

#region Unicode - IsUnicode
//Kolon içerisinde unicode karakterler kullanılacaksa bu yapılandırmadan istifade edilebilir.

//[Unicode]
//public string? Surname { get; set; }

//seklinde kullanılabilir. Fluent Api kullanımı ise 

//modelBuilder.Entity<Person>()
//    .Property(p => p.Surname)
//    .IsUnicode();

//seklindedir.

#endregion

#region Comment - HasComment
//EF Core üzerinden oluşturulmuş olan veritabanı nesneleri üzerinde bir açıklama/yorum yapmak istiyorsanız Comment'i kullanablirsiniz.

//[Timestamp]
//[Comment("Bu datanın versiyonlanmasına yaramaktadır.")]
//public byte[] RowVersion { get; set; }

//seklinde kullanılabilir. Bunun fluent api kullanımı ise 

//modelBuilder.Entity<Person>()
//        .HasComment("Bu tablo şuna yaramaktadır...")
//    .Property(p => p.Surname)
//        .HasComment("Bu kolon şuna yaramaktadır.");

//seklindedir.
//Neye comment eklediysek db de ilgili yapının properties kısmına gidip "extended properties" sekmesinden description kısmına bakarsak 
//ilgili comment i gorebiliriz. Veritabanıyla iliglenip kod kısmıyla alakalı olmayan bir kisi  u acıklamalardan istifade edebilir 
//veya daha sonra yaptıgımız calısmalara bakmak icin isimize yarayabilir.

#endregion

#region ConcurrencyCheck - IsConcurrencyToken
//Bir satırdaki verinin bütünsel olarak tutarlılığını sağlayacak bir concurrency token yapılanmasıyla ilgili bir konfigurasyondur.

//[ConcurrencyCheck]
//public int ConcurrencyCheck { get; set; }

//seklinde kullanılabilir. Fluent api kullanımı ise 

//modelBuilder.Entity<Person>()
//    .Property(p => p.ConcurrencyCheck)
//    .IsConcurrencyToken();

//seklindedir.
#endregion

#region InverseProperty
//İki entity arasında birden fazla ilişki varsa eğer bu ilişkilerin hangi navigation property üzerinden olacağını
//ayarlamamızı sağlayan bir konfigrasyondur.

/*
 public class Flight
{
    public int FlightID { get; set; }
    public int DepartureAirportId { get; set; }
    public int ArrivalAirportId { get; set; }
    public string Name { get; set; }
    public Airport DepartureAirport { get; set; }
    public Airport ArrivalAirport { get; set; }
}

public class Airport
{
    public int AirportID { get; set; }
    public string Name { get; set; }
    [InverseProperty(nameof(Flight.DepartureAirport))]
    public virtual ICollection<Flight> DepartingFlights { get; set; }

    [InverseProperty(nameof(Flight.ArrivalAirport))]
    public virtual ICollection<Flight> ArrivingFlights { get; set; }
}
 */
//seklindeki nesne yapısındaki iliskide kullanılır. Bu sekilde efcore hata vermez.
#endregion

#endregion

#region Configurations | Fluent API (Sadece fluent api dan yapacagımız konfigurasyonlardır.)

#region Composite Key
//Tablolarda birden fazla kolonu kümülatif olarak primary key yapmak istiyorsak yani iki kolon birlesik key olarak
//bir kimlik gorevi gosterecekse buna composite key denir.

//[Key]
//public int Id { get; set; }
//public int Id2 { get; set; }

//seklinde bir yapıda Id hem default convention geregi hem key attribute u geregi bir key dir. Lakin id2 nin de fazladan primary
//key olmasını istiyorsam eger yapmamız gereken islem 

//modelBuilder.Entity<Person>().HasKey("Id", "Id2"); //ilk keyt ayarlandı
////modelBuilder.Entity<Person>().HasKey(Id,Id2); //params ile 
//modelBuilder.Entity<Person>().HasKey(p => new { p.Id, p.Id2 });//ozel tanımlı fonksiyon ile 

//seklinde bir islemdir. İkinci satırda HasKey fonksiyonu params alabildigi icin id olmasını istedigimiz butun kolonları veriyoruz.
//params virgul ile ard arda verdigimiz degerleri bir dizi olarak yakalayabilir. 
//Burada params ile iki degeri id olarak belirttigimiz icin burada efcore primary key degil composite key kullanacagını anlar.
//Aynı sekilde ozel tanımlı fonksyon kullaıp anonim bir tür olusturarak da bunu yapabiliriz.
//Bu konfigurasyon sonucunda db ye bakarsak iki kolon da primary key olmus olur. Birden fazla primary key kullanımına ise anlayacagımız 
//uzere composite key diyoruz.
#endregion

#region HasDefaultSchema
//EF Core üzerinden inşa edilen herhangi bir veritabanı nesnesi default olarak dbo şemasına sahiptir.
//Bunu özelleştirebilmek için kullanılan bir yapılandırmadır.

//modelBuilder.HasDefaultSchema("customŞema");

//seklinde ilgili default şemayı ezebiliriz. 

#endregion

#region Property

#region HasDefaultValue
//Tablodaki herhangi bir kolonun değer gönderilmediği durumlarda default olarak hangi değeri alacağını belirler.

//modelBuilder.Entity<Person>()
// .Property(p => p.Salary)
// .HasDefaultValue(100);

//seklinde kullanıyoruz.
#endregion

#region HasDefaultValueSql
//Tablodaki herhangi bir kolonun değer gönderilmediği durumlarda default olarak hangi sql cümleciğinden değeri alacağını belirler.

//modelBuilder.Entity<Person>()
//    .Property(p => p.CreatedDate)
//    .HasDefaultValueSql("GETDATE()");

//seklinde kullanılır.
#endregion

#endregion

#region HasComputedColumnSql
//Tablolarda birden fazla kolondaki veirleri işleyerek değerini oluşturan kolonlara Computed Column denmektedir.
//EF Core üzerinden bu tarz computed column oluşturabilmek için kullanıolan bir yapılandırmadır.

//class Example
//{

//    public int X { get; set; }
//    public int Y { get; set; }
//    public int Computed { get; set; }
//}

//seklinde bir entity yapımız olsun.

//modelBuilder.Entity<Example>()
//    .Property(p => p.Computed)
//    .HasComputedColumnSql("[X] + [Y]");

//seklinde bir kullanım yapılabilir. Burada Computed kolonunun degeri x ve y kolonlarında yapılan islemin toplamı olmus olur.
//x ve y ye girilen degerler dogrultusunda Computed kolonunun degeri sql tarafından hesaplanır.
#endregion

#region HasConstraintName
//EF Core üzerinden oluşturulkan constraint'lere default isim yerine özelleştirilmiş bir isim verebilmek için kullanılan yapılandırmadır.

//modelBuilder.Entity<Person>()
//    .HasOne(p => p.Department)//1'e
//    .WithMany(d => d.Persons)//n ilişki modellendi
//    .HasForeignKey(p => p.DepartmentId)//foreign key belirlendi. Buradaki foreign key bir constraint tir ve bu constraint e default bir constraint name atanır.
//    .HasConstraintName("DepartmanForeignKey");//burada da kendi constraint name imizi verebiliriz.

//seklinde kullanım yapılabilir.
#endregion

#region HasData
//Seed Data isimli bir konu sonrasında olabilir. Bu konuda migrate sürecinde veritabanını inşa ederken bir
//yandan da yazılım üzerinden hazır veriler oluşturmak istiyorsak eğer bunun yöntemini öğremke gerekebilir.
//İşte HasData konfigürasyonu bu operasyonun yapılandırma ayağıdır.
//HasData ile migrate sürecinde oluşturulacak olan verilerin pk olan id kolonlarına iradeli bir şekilde değerlerin girilmesi zorunludur!

//modelBuilder.Entity<Department>().HasData(
//    new Department()
//    {
//        Name = "asd",
//        Id = 1
//    });
//modelBuilder.Entity<Person>().HasData(
//    new Person
//    {
//        Id = 1,//seed data olustururken id degerleri manuel girilmelidir. Yoksa hata verir.
//        DepartmentId = 1,
//        Name = "ahmet",
//        Surname = "filanca",
//        Salary = 100,
//        CreatedDate = DateTime.Now
//    },
//    new Person
//    {
//        Id = 2,
//        DepartmentId = 1,
//        Name = "mehmet",
//        Surname = "filanca",
//        Salary = 200,
//        CreatedDate = DateTime.Now
//    }
//    );

//seklinde bir kullanım yapılabilir. HasData kullanılırken migration da insert data ile veri girildiğini gorebiliriz.

#endregion

#region HasDiscriminator ve HasValue
//Entityler arasında kalıtımsal ilişkilerin olduğu TPT ve TPH isminde durumlar vardır.
//İşte bu durumlarda ilgili yapılandırmalarımız HasDiscriminator ve HasValue fonksiyonlarıdır.

//class Entity //base class
//{
//    public int Id { get; set; }
//    public string X { get; set; }
//}
//class A : Entity //drived class
//{
//    public int Y { get; set; }
//}
//class B : Entity //drived class
//{
//    public int Z { get; set; }
//}

//seklinde bir entity yapımız var. 

//public DbSet<Entity> Entities { get; set; }
//public DbSet<A> As { get; set; }
//public DbSet<B> Bs { get; set; }

//seklinde bir yapılandırmamız var.

//seklinde bir kullanım yapılabilir. Burada bir migration atarsak migration daki tablo yapılandırmasında ne A class ı ne B class ı vardır.
//Entities tablosu vardır ve bu tabloda hem Entity hem A hem de B deki kolonlar tek tabloda modellenir. Iste burada A ya B ye vs
//deger girildiginde hangi tabloya deger vereceginin ayrımını efcore discriminator ile yapar.
//Olusturulan Entities tablosu db ye gonderilirse kolonları
//Id,X,Discriminator,Y ,Z kolonlarıdır. Y ve Z kolonları A ve B entity sine aittir. 
//Burada A ya veri eklenirse bu veri X e eklenir. Verinin Y e eklendigine yani aslında A ya eklendigine dair discriminator
//kolonuna bir işaret konur. 

//A a = new A
//{
//    X = "A'dan",
//    Y = 1
//};
//B b = new B
//{
//    X = "B'den",
//    Z = 2
//};
//Entity entity = new Entity
//{
//    X = "Entity'den"
//};

//await context.As.AddAsync(a);
//await context.Bs.AddAsync(b);
//await context.Entities.AddAsync(entity);
//await context.SaveChangesAsync();

//seklinde bir veri ekleme islemi yapılmıs olsun. Db ye giden veri

// Id=1,X=A'dan,Discriminator=A,Y=1,Z=NULL
// Id=2,X=B'den,Discriminator=B,Y=NULL,Z=2
// Id=3,X=Entity'den,Discriminator=Entity,Y=NULL,Z=NULL

//seklinde olur. Goruldugu üzere Discriminator bir ayrac gorevi gorur.

//Iste tam buradaki Discriminator kolonuna müdahale etmek istiyorsak 

//modelBuilder.Entity<Entity>()//once base entity ye gidiyoruz cunku tablo base entity için oluşturulur.
//    .HasDiscriminator<string>("Ayirici")//Discriminator kolonuna mudahale edildi. Tip string verildigi icin sub tabloların ismi tutulur.

//modelBuilder.Entity<Entity>()
//    .HasDiscriminator<int>("Ayirici")
//    .HasValue<A>(1)
//    .HasValue<B>(2)
//    .HasValue<Entity>(3);

//seklinde discriminator tipi int verilirse bu sefer hasvalue ile A dan veri gelirse 1 B den veri gelirse 2 Entity den veri gelirse 3
//degerini ver seklinde bir yapı da kurabiliriz.

//seklinde bir konfigurasyon yapabiliriz. Oncesinde string veri eklenmisse bu konfigurasyon tabii ki hata verir. Tablo bastan
//olusturuluyorsa hata durumu oluşmaz.

#endregion

#region HasField
//Backing Field özelliğini kullanmamızı sağlayan bir yapılandırmadır.

//modelBuilder.Entity<Person>()
//    .Property(p => p.Name)
//    .HasField(nameof(Person._name));

//seklinde kullanılabilir. _name backing field ozelligine sahip olur.
#endregion

#region HasNoKey
//Normal şartlarda EF Core'da tüm entitylerin bir PK kolonu olmak zorundadır. i
//Eğer ki entity'de pk kolonu olmayacaksa bunun bildirilmesi gerekmektedir! İşte bunun için kullanuılan fonksiyondur.

//modelBuilder.Entity<Example>()
//    .HasNoKey();//bu entity nin bir primary key i yok demis olduk.

//seklinde kullanılabilir.
#endregion

#region HasIndex
//Bu yapılanmaya dair konfigürasyonlarımız fluent api de HasIndex ve atrribute olarak da Index attribute'dur.

//modelBuilder.Entity<Person>()
//    .HasIndex(p => new { p.Name, p.Surname });

//seklinde kullanılabilir.Bunun uzerinde de sonradan durabiliriz.
#endregion

#region HasQueryFilter
//Global Query Filter başlıklı calısmanın yapılandırmasıdır.
//Temeldeki görevi bir entitye karşılık uygulama bazında global bir filtre koymaktır.

//modelBuilder.Entity<Person>()
//    .HasQueryFilter(p => p.CreatedDate.Year == DateTime.Now.Year);

//seklinde kullanılabilir. Bir filtre her sorguda verilmek istenebilir ancak filtreyi
//uygulama bazında bu sekilde tanımlarsak her sorguda yazmamıza gerek kalmaz. Yapılan her sorguya efcore bir where sartı ekler. 
//Gelen verileri where in iceriginine gore getirmis olur.
#endregion

#region DatabaseGenerated - ValueGeneratedOnAddOrUpdate, ValueGeneratedOnAdd, ValueGeneratedNever
//Bunlar icin sonradan calısma planlandı.
#endregion
#endregion



//[Table("Kisiler")]
class Person
{
    //[Key]
    public int Id { get; set; }
    //public int Id2 { get; set; }
    //[ForeignKey(nameof(Department))]
    //public int DId { get; set; }
    //[Column("Adi", TypeName = "metin", Order = 7)]
    public int DepartmentId { get; set; }
    public string _name;
    public string Name { get => _name; set => _name = value; }
    //[Required()]
    //[MaxLength(13)]
    //[StringLength(14)]
    [Unicode]
    public string? Surname { get; set; }
    //[Precision(5, 3)]
    public decimal Salary { get; set; }
    //Yazılımsal amaçla oluşturduğum bir property
    //[NotMapped]
    //public string Laylaylom { get; set; }

    [Timestamp]
    //[Comment("Bu şuna yaramaktadır...")]
    public byte[] RowVersion { get; set; }

    //[ConcurrencyCheck]
    //public int ConcurrencyCheck { get; set; }

    public DateTime CreatedDate { get; set; }
    public Department Department { get; set; }
}
class Department
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Person> Persons { get; set; }
}
class Example
{

    public int X { get; set; }
    public int Y { get; set; }
    public int Computed { get; set; }
}
class Entity
{
    public int Id { get; set; }
    public string X { get; set; }
}
class A : Entity
{
    public int Y { get; set; }
}
class B : Entity
{
    public int Z { get; set; }
}
class ApplicationDbContext : DbContext
{
    //public DbSet<Entity> Entities { get; set; }
    //public DbSet<A> As { get; set; }
    //public DbSet<B> Bs { get; set; }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<Flight> Flights { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Example> Examples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region GetEntityTypes
        //var entities = modelBuilder.Model.GetEntityTypes();
        //foreach (var entity in entities)
        //{
        //    Console.WriteLine(entity.Name);
        //}
        #endregion
        #region ToTable
        //modelBuilder.Entity<Person>().ToTable("aksdmkasmdk");
        #endregion
        #region Column
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Name)
        //    .HasColumnName("Adi")
        //    .HasColumnType("asldalsd")
        //    .HasColumnOrder(7);
        #endregion
        #region ForeignKey
        //modelBuilder.Entity<Person>()
        //    .HasOne(p => p.Department)
        //    .WithMany(d => d.Persons)
        //    .HasForeignKey(p => p.DId);
        #endregion
        #region Ignore
        //modelBuilder.Entity<Person>()
        //    .Ignore(p => p.Laylaylom);
        #endregion
        #region Primary Key -> Has Key
        //modelBuilder.Entity<Person>()
        //    .HasKey(p => p.Id);
        #endregion
        #region IsRowVersion
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.RowVersion)
        //    .IsRowVersion();
        #endregion
        #region Required
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Surname).IsRequired();
        #endregion
        #region MaxLength
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Surname)
        //    .HasMaxLength(13);
        #endregion
        #region Precision
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Salary)
        //    .HasPrecision(5, 3);
        #endregion
        #region Unicode
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Surname)
        //    .IsUnicode();
        #endregion
        #region Comment
        //modelBuilder.Entity<Person>()
        //        .HasComment("Bu tablo şuna yaramaktadır...")
        //    .Property(p => p.Surname)
        //        .HasComment("Bu kolon şuna yaramaktadır.");
        #endregion
        #region ConcurrencyCheck
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.ConcurrencyCheck)
        //    .IsConcurrencyToken();
        #endregion
        #region CompositeKey
        //modelBuilder.Entity<Person>().HasKey("Id", "Id2");
        //modelBuilder.Entity<Person>().HasKey(p => new { p.Id, p.Id2 });
        #endregion
        #region HasDefaultSchema
        //modelBuilder.HasDefaultSchema("ahmet");
        #endregion
        #region Property
        #region HasDefaultValue
        //modelBuilder.Entity<Person>()
        // .Property(p => p.Salary)
        // .HasDefaultValue(100);
        #endregion
        #region HasDefaultValueSql
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.CreatedDate)
        //    .HasDefaultValueSql("GETDATE()");
        #endregion
        #endregion
        #region HasComputedColumnSql
        //modelBuilder.Entity<Example>()
        //    .Property(p => p.Computed)
        //    .HasComputedColumnSql("[X] + [Y]");
        #endregion
        #region HasConstraintName
        //modelBuilder.Entity<Person>()
        //    .HasOne(p => p.Department)
        //    .WithMany(d => d.Persons)
        //    .HasForeignKey(p => p.DepartmentId)
        //    .HasConstraintName("ahmet");
        #endregion
        #region HasData
        //modelBuilder.Entity<Department>().HasData(
        //    new Department()
        //    {
        //        Name = "asd",
        //        Id = 1
        //    });
        //modelBuilder.Entity<Person>().HasData(
        //    new Person
        //    {
        //        Id = 1,
        //        DepartmentId = 1,
        //        Name = "ahmet",
        //        Surname = "filanca",
        //        Salary = 100,
        //        CreatedDate = DateTime.Now
        //    },
        //    new Person
        //    {
        //        Id = 2,
        //        DepartmentId = 1,
        //        Name = "mehmet",
        //        Surname = "filanca",
        //        Salary = 200,
        //        CreatedDate = DateTime.Now
        //    }
        //    );
        #endregion
        #region HasDiscriminator
        //modelBuilder.Entity<Entity>()
        //    .HasDiscriminator<int>("Ayirici")
        //    .HasValue<A>(1)
        //    .HasValue<B>(2)
        //    .HasValue<Entity>(3);

        #endregion
        #region HasField
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Name)
        //    .HasField(nameof(Person._name));
        #endregion
        #region HasNoKey
        //modelBuilder.Entity<Example>()
        //    .HasNoKey();
        #endregion
        #region HasIndex
        //modelBuilder.Entity<Person>()
        //    .HasIndex(p => new { p.Name, p.Surname });
        #endregion
        #region HasQueryFilter
        //modelBuilder.Entity<Person>()
        //    .HasQueryFilter(p => p.CreatedDate.Year == DateTime.Now.Year);
        #endregion
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDb;User ID=SA;Password=1q2w3e4r+!");
    }
}

public class Flight
{
    public int FlightID { get; set; }
    public int DepartureAirportId { get; set; }
    public int ArrivalAirportId { get; set; }
    public string Name { get; set; }
    public Airport DepartureAirport { get; set; }
    public Airport ArrivalAirport { get; set; }
}

public class Airport
{
    public int AirportID { get; set; }
    public string Name { get; set; }
    [InverseProperty(nameof(Flight.DepartureAirport))]
    public virtual ICollection<Flight> DepartingFlights { get; set; }

    [InverseProperty(nameof(Flight.ArrivalAirport))]
    public virtual ICollection<Flight> ArrivingFlights { get; set; }
}