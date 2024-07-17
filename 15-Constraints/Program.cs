using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Primary Key Constraint

//Bir kolonu PK constraint ile birincil anahtar yapmak istiyorsak eğer bunun için name convention'dan istifade edebiliriz.
//Id, ID, EntityNameId, EntityNameID şeklinde tanımlanan tüm propertyler default olarak EF Core tarafından pk constraint
//olacak şekilde generate edilirler.
//Eğer ki, farklı bir property'e PK özelliğini atamak istiyorsan burada HasKey Fluent API'ı yahut Key attribute'u ile bu bildirimi
//iradeli bir şekilde yapmak zorundasın.

#region HasKey Fonksiyonu
//Fluent api fonksiyonudur.
#endregion
#region Key Attribute'u
//Data Annotations yapısıdır.
#endregion
#region Alternate Keys - HasAlternateKey
//Bir entity içerisinde PK'e ek olarak her entity instance'ı için alternatif bir benzersiz tanımlayıcı işlevine sahip olan bir key'dir.
//Db de ilgili kolonu unique hale getirir ve kolon bir nevi kimlik deger kazanır.
#endregion
#region Composite Alternate Key
//Birden fazla kolonu alternnate key yapmakta kullanılır.
#endregion

#region HasName Fonksiyonu İle Primary Key Constraint'e İsim Verme
//PK olarak kısıtlanmıs yapıya farklı bir isim vermek icin kullanılır.
#endregion
#endregion

#region Foreign Key Constraint

#region HasForeignKey Fonksiyonu

#endregion
#region ForeignKey Attribute'u

#endregion
#region Composite Foreign Key

#endregion

#region Shadow Property Üzerinden Foreign Key

#endregion

#region HasConstraintName Fonksiyonu İle Primary Key Constraint'e İsim Verme

#endregion
#endregion

#region Unique Constraint 
//Bir kolonu unique yaparak mükerrer kaydın girilmesini engellemektir.

#region HasIndex - IsUnique Fonksiyonları

#endregion

#region Index, IsUnique Attribute'ları

#endregion

#region Alternate Key

#endregion
#endregion

#region Check Constratint

#region HasCheckConstraint

#endregion
#endregion

//[Index(nameof(Blog.Url), IsUnique = true)]//Url attribute unu unique yapar. Class seviyesinde kullanılan bir attribute tur.
class Blog
{
    public int Id { get; set; }
    //[Key] seklinde yaparsak bu entity de kilige karsılık gelecek kolon BlogName olur. Bu sekilde bir override islemi yapmıs oluruz.
    //Bu islemi fluent api ile de yapabiliriz.
    public string BlogName { get; set; }
    public string Url { get; set; }

    public ICollection<Post> Posts { get; set; }
}
class Post
{
    public int Id { get; set; }

    //[ForeignKey(nameof(Blog))]
    //public int BlogId { get; set; }
    //seklinde de Foreign Key i tanımlayabiliriz.
    public string Title { get; set; }
    public string BlogUrl { get; set; }
    public int A { get; set; }
    public int B { get; set; }

    public Blog Blog { get; set; }
}


class ApplicationDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Blog>()
        //    .HasKey(b => b.BlogName) 
        //or
        //    .HasKey(b => b.Id)
        //    .HasName("ornek");

        //modelBuilder.Entity<Blog>()
        //    .HasAlternateKey(b => b.Url); //alternate key 
        //or
        //    .HasAlternateKey(b => new { b.Url, b.BlogName });//composite alternate key


        //modelBuilder.Entity<Blog>()
        //    .Property<int>("BlogForeignKeyId");//shadow property tanımı

        //modelBuilder.Entity<Blog>()
        //    .HasMany(b => b.Posts)
        //    .WithOne(b => b.Blog)
        //iliski tanımlandı
        //    .HasForeignKey(p=>p.BlogId)//dependent table dan yani dependent entity uzerinden foreign key tanımlandı.
        //or
        //    .HasForeignKey(p=>new {p.BlogId,p.BlogUrl})//Seklinde composite Foreign key yapısı da olusturabiliriz.

        //or
        //    .HasForeignKey("BlogForeignKeyId")//shadow property uzerinden Foreign key tanımı yapıldı.
        //Bundan sonra efcore fiziksel bir property uzerinden degilde bizim olusturdugumuz sanal shadow property uzerinden bir foreign 
        //key tanımı yapar.

        //    .HasConstraintName("ornekforeignkey");//Bu sekilde foreign key in custom isimlendirmesi yapılabilir.

        //modelBuilder.Entity<Blog>()
        //    .HasIndex(b => b.Url)
        //    .IsUnique();
        //Unique constraint tanımlamasının fluent api versiyonudur. Ilgili kolonu unique yapar. 

        //modelBuilder.Entity<Blog>()
        //    .HasAlternateKey(b => b.Url);
        //Bu sekilde de unique constraint uygulamasını yapabiliriz. 

        modelBuilder.Entity<Post>()
            .HasCheckConstraint("a_b_check_const", "[A] > [B]");
        //A kolonundaki deger B kolonundaki degerden buyuk ise Post a deger girilebilsin seklinde bir konfigurasyon yapmıs olduk.
        //Bu konfigurasyona sekilde ki gibi isimde verebiliyoruz.
        //Burada A>B seklinde de yazabilirdik. Lakin C# icin uygun olan bir text Db de bir anahtar kelimeye vs karsılık gelebilir.
        //Buradaki guvenligi saglamak acısından [] kullanıyoruz.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}