using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _4_ManyToManyRelation : ControllerBase
    {
        /*Coka cok ilisjide ara tablolara bir baska adıyla cross table lara ihtiyac duyarız. 
         Tablolar ara tablo ile cokacok bir ilsikiye sahiptir. Yani tablolar ara tablo aracılıgıyla dolaylı yoldan coka cok iliskiye 
        sahip olmus olurlar.  Ara tabloalrda ise composite primary key yapısı kullanılır.*/
        #region Many To Many Relationship | Tüm Detaylarıyla Çoka Çok İlişki Yapılanması
        [HttpGet]
        public async Task<IActionResult> ManyToManyRelationship()
        {
            #region Default Convention
            /*Default Coonvention da coka cok ozelligi belirtmek icin ikii property arasındaki bagı cokacok kurmamız gerekir.
             
                class Kitap
                {
                    public int ID { get; set; }
                    public string KitapAdi { get; set; }
                    public ICollection<Yazar> Yazarlar { get; set; }
                }

                class Yazar
                {
                    public int ID { get; set; }
                    public string YazarAdi{ get; set; }
                    public ICollection<Kitap> Kitaplar { get; set; }
                }
            seklinde bir tablo yapısında efcore coka cok yani n-n ilsiki turunu kendisi insaa eder. Biz ara tablonun entity sini 
            olusturmaksızın efcore kendisi bunu olusturur. Olusturdugu tablonun ismi birinci tablo ismi + ikinci tablo ismi yani KitapYazar 
            seklinde olur. Bu tabloya kitaplar ve yazarlar ile ilgili birer id atar. KitaplarId yi Kitaplar tablosundaki ID ile N-1 olarak 
            iliskilendirir. YazarlarID yi yazarlar tablosundaki Id ile N-1 iliskilendirir. Dolayısıyla N-N iliski saglanır. Ayrıca bir composite id den bahsettik.
            Cross table da primary key olarak hem KitaplarID yi hem YazarlarID yi tutar. Burada primary key ikisinin birlesmis halidir. 
            1-1 1-2 2-1 2-2 2-3 seklinde primary key ler tutar. 
            Lakin tekrardan 1-1 tutamaz.
             */
            #endregion

            #region Data Annotations
            /*Data Annotations larda cross table manuel oalrak bizim tarafımızdan olusturulması gerekir. EFCore buraya mudahale etmez.
            Ayrıca bu iliskiyi cross table i kendimiz olusturdugumuz icin artık ana tablolar uzerinden degil ara tablo ve ara tablo 
            uzerinden kurarız. 
            
             class Kitap
            {
                public int ID { get; set; }
                public string KitapAdi { get; set; }
                public ICollection<KitapYazar> Yazarlar { get; set; }
            }

            class Yazar
            {
                public int ID { get; set; }
                public string YazarAdi{ get; set; }
                public ICollection<KitapYazar> Kitaplar { get; set; }
            }
            class KitapYazar
            {
                public int KitapID { get; set; }
                public int YazarID { get; set; }
                public Kitap Kitap { get; set; }
                public Yazar Yazar { get; set; }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<KitapYazar>()
                    .HasKey(ky => new { ky.KitapID, ky.YazarID });//composite primary key tasarımı
                //Burada anonim bir tur olusturarak hem kitapId hem YazarId nin primary key olması saglandı.
            }

            Bu sekilde ara tablo manuel olarak olusturulur ve ilsikiler artık ana tabloların birbirleri uzerinden degil ara tablo ile 
            kurulur. Tablolar ile Ara tablo arasındaki iliski 1-n olur. Totalde iliski N-N olur.
            Kitaplar dan KitapYazar a giderim. Oradan da Yazarlar a giderim. Aynısının tersi de mumkundur.,
            Lakin composite primary key yapısının manuel olarak kurulabilmesi icin Data Annotation da kullansak Fluent api ile bu konfigurasyonu
            yapmamız gerekir. Ara tablodaki her iki id ye de [Key] attribute u verilemez. OnModelCreating fonksiyonunun override i ile yapılır.,
            Bir baska detay olarak Cross Table a karsılık olusturulan model context icerisinde dbset olarak tanımlanmak zorunda degildir.
            EFCore arkadaki yapıyı kendisi isletir. Akıs icerisinde Kitaplar dan Yazarlara veya tam tersi bir sorgulama yapılabilir.
             */
            #endregion

            #region Fluent API
            /*Aynı sekilde fluent api da da crosstable manuel olarak olusturulmalıdır.
             Crosstable ın dbset olarak olusuturulmasına gerek yoktur.
            Composite primary key yapısı hasKey ile kurulmalıdır. 
            Bunları zaten DataAnnotations kullanımından biliyoruz.
            
            class Kitap
            {
                public int ID { get; set; }
                public string KitapAdi { get; set; }
                public ICollection<Yazar> Yazarlar { get; set; }
            }

            class Yazar
            {
                public int ID { get; set; }
                public string YazarAdi{ get; set; }
                public ICollection<Kitap> Kitaplar { get; set; }//ICollection yerine List de kullanılailir.
            }

            class KitapYazar
            {
                public int KitapID { get; set; }
                public int YazarID { get; set; }
                public Kitap Kitap { get; set; }
                public Yazar Yazar { get; set; }
            }
            Bu sekilde yapılanma neredeyse aynı. Simdi konfigurasyona geliyoruz.
            
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<KitapYazar>()
                    .HasKey(ky => new { ky.KitapID, ky.YazarID });//composite primary key tasarımı
                                                                  //Burada anonim bir tur olusturarak hem kitapId hem YazarId nin primary key olması saglandı.

                modelBuilder.Entity<KitapYazar>()
                    .HasOne(ky => ky.Kitap)
                    .WithMany(k => k.Yazarlar)
                    .HasForeignKey(ky => ky.KitapID);
                
                //Kitap ile Yazarlar arasında KitapYazar aracılıgıyla 1-n iliski kuruldu. Bu iliskide foreign key KitapID dir.
                modelBuilder.Entity<KitapYazar>()
                .HasOne(ky => ky.Yazar)
                .WithMany(k => k.Kitaplar)
                .HasForeignKey(ky => ky.YazarID);
                //Yazar ile Kitaplar arasında KitapYazar aracılıgıyla 1-n iliski kuruldu. Bu iliskide foreign key YazarID dir.
            }

            Bu sekilde butun konfigurasyonları kendimiz yaparak Fluent API yapısını kullanmıs oluyoruz.
             */

            #endregion

            return Ok();
        }

        #endregion
    }
    class Kitap
    {
        public int ID { get; set; }
        public string KitapAdi { get; set; }
        public ICollection<Yazar> Yazarlar { get; set; }
    }

    class Yazar
    {
        public int ID { get; set; }
        public string YazarAdi{ get; set; }
        public ICollection<Kitap> Kitaplar { get; set; }//ICollection yerine List de kullanılailir.
    }

    class EKitapDBContext : DbContext
    {
        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Yazar> Yazarlar { get; set; }
        protected override  void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer("[connectionString]");
        }

        //Data Annotations icin icindir.
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<KitapYazar>()
        //        .HasKey(ky => new { ky.KitapID, ky.YazarID });//composite primary key tasarımı
        //    //Burada anonim bir tur olusturarak hem kitapId hem YazarId nin primary key olması saglandı.
        //}

        //FluentAPI icin icindir.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KitapYazar>()
                .HasKey(ky => new { ky.KitapID, ky.YazarID });//composite primary key tasarımı
                                                              //Burada anonim bir tur olusturarak hem kitapId hem YazarId nin primary key olması saglandı.

            modelBuilder.Entity<KitapYazar>()
                .HasOne(ky => ky.Kitap)
                .WithMany(k => k.Yazarlar)
                .HasForeignKey(ky => ky.KitapID);
                
            //Kitap ile Yazarlar arasında KitapYazar aracılıgıyla 1-n iliski kuruldu. Bu iliskide foreign key KitapID dir.
            modelBuilder.Entity<KitapYazar>()
            .HasOne(ky => ky.Yazar)
            .WithMany(k => k.Kitaplar)
            .HasForeignKey(ky => ky.YazarID);
            //Yazar ile Kitaplar arasında KitapYazar aracılıgıyla 1-n iliski kuruldu. Bu iliskide foreign key YazarID dir.
        }
    }

    //Data Annotations ve fluent api icin icindir.
    class KitapYazar
    {
        public int KitapID { get; set; }
        /*EFCore buradaki id lerin foreign key oldugunu isimlendirmeden anlar. Bu yapının ismi KID cs olsaydı EFCore kendisi KID kolonu hariicnde 
         * bir Foreign key kolonu olusturur. Bunu onlemek icin 
        [ForeignKey]
        public int KID { get; set; }

        seklinde foreign key attribute yapısını kullanırız.
         */
        public int YazarID { get; set; }
        public Kitap Kitap { get; set; }
        public Yazar Yazar { get; set; }
    }

}
