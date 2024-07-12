using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _2_OneToOneRelation : ControllerBase
    {
        #region One to One Relationship | Tüm Detaylarıyla Birebir İlişki Yapılanması
        [HttpGet]
        public async Task<IActionResult> OneToOneRelationship()
        {
            #region Default Convention
            /*
             
                 class Calisan
                {
                    public int ID { get; set; }
                    public string Adi { get; set; }
                    //Ustteki property ler iliskisel bir durum belirtmezler
                    public CalisanAdresi CalisanAdresi { get; set; }//Iste bu bir Nav Prop yapısıdır cunku bu prop turu bir entity turundendir.
                    //Bir 1-x iliski belirtir. Yani bı prop tekil bir property dir.
                }
                class CalisanAdresi
                {
                    public int ID { get; set; }
                    public string adres { get; set; }
                    //Ustteki property ler iliskisel bir durum belirtmezler
                    public Calisan Calisan { get; set; }//Iste bu bir Nav Prop yapısıdır cunku bu prop turu bir entity turundendir.
                    //Bir 1-x iliski belirtir. Yani bı prop tekil bir property dir.

                }
            Bu sekilde bir iliski yapısı 1-1 iliskiyi belirtir. Lakin entity leri boyle tasarlayıp migrate etmek istedigimiz zaman 
            bir hata alırız. Burada EFCore hangi entity nin dependent entity olacagını bilemediginden bir hata verir. Yani ney neye baglı?
            Calısan mı aderse yoksa adres mi calısana baglı? EFCore bunu bilemez ve bunu belirtmemizi ister. Bu sebeple hangi entity dependent
            entity ise o entity de principal entity ye karsılık gelen bir foreign key tanımlamamız gerekir.
            Bizim senaryomuzda CalisanAdres, Calisan tablosuna baglıdır. Bu sebeple CalisanAdres tablosunda Calisan tablosunu isaret eden bir 
            foreign key tanımlarız.
             
                class Calisan
                {
                    public int ID { get; set; }
                    public string Adi { get; set; }
                    public CalisanAdresi CalisanAdresi { get; set; }
                }
                class CalisanAdresi
                {
                    public int ID { get; set; }
                    public string adres { get; set; }
                    public int CalisanID { get; set; }
                    public Calisan Calisan { get; set; }

                }
            Bu sekilde artık hangi tablonun hangi tabloya baglı oldugunu biliyoruz. CalisanAdresi tablosu, Calisan tablosuna bagımlıdır.
            Bu sebeple kendi bunyesinde Calisan tablosuna ait bir foreign key tutar. Bu property nin Calisan tablosuna ait ID yi temsil 
            ettigini efcore yapılanması temel convention dan anlar. Ilgili isimlendirme standartında dolayı efcore bunu cozebilir. Eger 
            bir kolonda ID varsa bu bir key dir. Oncesindeki Calisan kelimesi Calisan tablosunu temsil eder. EFCore bunu anlar ve buna 
            gore bir iliskiyi db ye aktarır. Bu sekilde bir migrate yapıldıgında ilgili migration a bakıldıgında 
            ID kolonlarına primary key, CalisanID kolonuna foreign key ozellikleri verildigi gorulebilir.

            Db deki tablo yapısında 
            Calisanlar                                  CalisanAdresi
            Id      Adi                         ID      CalisanID       Adres

            1       Ahmet                       1       1               A
            2       Mehmet                      2       2               B
                                                3       2               C
                                                4       4               D

            seklinde kayıtlar eklenmek istenirse ne olur?
            Ilk 2 satırlar hatasız kaydedilir.
            CalisanAdresi tablosunda 3.satırda bir kayıt yapılmaz cunku 2 id sine sahip calisan icin zaten bir adres tanımlanmıstı.
            1-1 iliskiden dolayı buna izin verilmez.
            4.satırda ise yine hata alınır. Cunku 4 calisan id sine sahip bir calisan kaydı Calisan tablosunda yoktur.
            Yani Contraint ler devrededir. 


            Kısa ve öz olarak;
            1-1 ilsikide Default Convention kullanılabilmesi icin 
            -  her iki entity nin de navigation property ler aracılıgıyla birbirlerini tekil olarak referans ederek fiziksel bir iliskide
            olmaları gerekir.

            - Dependent entity nin hangisinin oldugunu default olarak belirleyemiyoruz. Bunun icin dependent entity de Principal entity yi 
            isaret eden bir foreign key tanımlanması gerekir. B
             */
            #endregion

            #region Data Annotations
            /*
                class Calisan
                {
                    public int ID { get; set; }
                    public string Adi { get; set; }
                    public CalisanAdresi CalisanAdresi { get; set; }
                }
                class CalisanAdresi
                {
                    public int ID { get; set; }
                    public string adres { get; set; }
                    public Calisan Calisan { get; set; }

                }

            seklinde bir tablo yapımız olsun. Yine bu yontemde de nav prop olmalıdır. Cunku nav prop olmadan herhangi bir yontemde iliski 
            belirtemiyoruz. Lakin default convention da hangi entity nin dependent entity oldugunu belirtmek icin bir foreign key kolonu daha 
            kullanıyorduk. Data QAnnotations larda ise bu kolon yine kullanılır ancak conventions lara uymaya gerek yoktur.
            
                             class Calisan
                {
                    public int ID { get; set; }
                    public string Adi { get; set; }
                    public CalisanAdresi CalisanAdresi { get; set; }
                }
                class CalisanAdresi
                {
                    public int ID { get; set; }
                    public string adres { get; set; }
                    [ForeignKey(nameof(Calisan))]        
                    public int herhanigBirIsim {get; set;}
                    public Calisan Calisan { get; set; }

                }
             
            Bu sekilde bir kullanım yapabiliyoruz. 
             [ForeignKey(nameof(Calisan))] attribute u foreign key yapmak istediimiz property nin uzerine yazılır.
            Bu attribute un altında ki foreign key olması istenen property nin adının ne oldugu farketmez. X,Y,Z herhangi bir isim olabilir.
            Dolayısıyla convention uygulamamız gerekmez. Cunku belirtilmesi gerekenler zaten attribute icinde belirtilmistir.
            Icerisine ise hangi nav property yi yani hangi principal entity yi isaret edecegi de belirtilir. Derin c# bilgisi 
            ile biz bunu nameof icerisinde belirtiriz ki eger yarın birgun Calisan ismi degisirse bize uyarı versin.
            Velhasıl buradaki yapı da birebir bir iliskiye uygun yapıda migrate edilebilir. Migration a bakıldıgında aynı sekilde 
            foreign key yapısı kurulmus olur. Ayrıca yine olusan migration da ilgili foreign key olan kolon icin bir constraint tanımlanır.
            Foreign key olan kolonun ismi X ise, X in unique:true seklinde kısıtlandıgını yani X in unique bir index e sahip oldugunu 
            goruyoruz. Bu normalde 1-n kurulacak iliskiyi 1-1 olarak garanti eden yapıdır. Bunu zaten Data Annotaions larda da gorduk. Bir calisana 
            birden fazla adresi atayamadık. Aynı yapı default convention da da kurulur. Kısacası birebir ilsikilerde foreign key unique olarak 
            kısıtlanır. Boylece birebir iliski garanti edilir.
            Bunun daha da pratik bir yolu vardır. Gorelim.

                class Calisan
            {
                public int ID { get; set; }
                public string Adi { get; set; }
                public CalisanAdresi CalisanAdresi { get; set; }
            }
            class CalisanAdresi
            {
                [Key, ForeignKey(nameof(Calisan))]
                public int ID { get; set; }
                public string adres { get; set; }
                public Calisan Calisan { get; set; }
            }

            Bun sekilde bir entity yapılanması kurulursa foreign key e gerek kalmaz. Hem de Dependent tablo belirtilmis olur.
                [Key, ForeignKey(nameof(Calisan))]
                public int ID { get; set; }
            yapısıyla ID kolonu hem bir Key yani primary key ozelligine sahip olur. Hem de Calisan principal entity sine ait bir foreing
            key oldugu belirtilir. Boylece 1-1 iliski yapısı bu sekilde de garanti altına alınmıs olur. Ek bir foreign key kolonu hem entity de 
            hem de db de tanımlanmak zorunda kalmaz. 
            Db deki tablo yapısına bakacak olursak eger

            Calisanlar                  CalisanAdres
            Id      Adi                 Id          Adres

            1       Hasan               null        A
            2       Huseyin             1           A       
            3       Ali                 2           B
                                        2           C
                                        4           D
            kayıtlarına bakalım. CalisanAdres tablosunda fazladan CalisanId kolonu yoktur. 
            CalisanAdres teki 1.kayıt yapılmaz hata verir. Burada id identity yani null bırakilsa bile otomatik olarak artan yapıda olmaz. Hata 
            fırlatır. Cunku hem id null olamaz hem de artık bilincli bir sekilde Calisanlar tablosundan bir ID beklenir.  
            Foreign Key devrededir. 3.satırdaki kayıtta yapılamaz cunku daha oncesinden 2.satırda 2 Id li calisana bir adres atanmıstır. 
            1-1 ozellik korunur. Unique Contraint devrededir.
            4.satırdaki kayıt ise yine yapılamaz. Cunku Id kolonuna atanacak degerin Calisanlar tablosunda olması gerekir. Yine 1-1 ozellik korunur.
            Burada da foreign key contraint devrededir.

            Sozun özü;
            - Nav Prop tanımlanmalıdır.
            - Foreing Key olusturulmak zorunlulugu yoktur.
            - Foreign Key kolonu olusturulacaksa eger default Convention kuralları kapsamına uymak zorunlulugu yoktur. Foreign Key attribute u ile 
            bu bildirilebilir.
            - 1-1 iliskide extradan foreign key kolonuna mantıken gerek olmadıgından dolayı dependent entity deki Id kolonunu hem Key hem de 
            foreign key olarak attribute ler aracılıgıyla kullanabilir. Hatta bu durum genellikle tercih edilir, edilmelidir. I
             */
            #endregion


            #region Fluent API
            /*
                 class Calisan
            {
                public int ID { get; set; }
                public string Adi { get; set; }
                public CalisanAdresi CalisanAdresi { get; set; }
            }
            class CalisanAdresi
            {
                public int ID { get; set; }
                public string adres { get; set; }
                public Calisan Calisan { get; set; }
            }

            seklinde entity yapılanmamız olsun.
            Fluent API yonteminde entity class ları ile oynamak yerine context class uzerinde onModleCreating fonksiyonu override 
            edilerek bir konfigurasyon yapılır. Model larin yani entity lerin olusturulması ile ilgili butun konfifurasyonlar bu metot 
            icerisinde yapılırlar. Bir baska deyisle model ların db de generate edilecek yapılarının konfigurasyonları bu fonksiyon icerisinde 
            yapılır, konfigure edilir. Burada yapılan calisamaya ise fluent api denir.

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Calisan>()
                    .HasOne(c => c.CalisanAdresi)
                    .WithOne(c => c.Calisan)
                    .HasForeignKey<CalisanAdresi>(c => c.ID);
                modelBuilder.Entity<CalisanAdresi>()
                    .HasKey(c => c.ID);
            }

            seklinde bir fluent api calısması yaptım. modelBuilder.Entity<Calisan>() yani Calisan entity si uzerindesin.
            .HasOne(c => c.CalisanAdresi) yani 1-x bir iliski baslat. CalisanAdresi propert sini Nav property olarak algıla.
            .WithOne(c => c.Calisan) yani 1-x baslattıgın iliskiyi 1-1 yap. CalisanAdresi Property sinin isaret ettigi CalisanAdresi 
            tablosunun Calisan kolonu ise diger Nav Property dir. Bunu algıla.
            .HasForeignKey<CalisanAdresi>(c => c.ID); yani az once iliskisi belirtilen CalisanAdresi tablosunun yani dependent entity nin,
            Calisanlar tablosunu yani principalEntity yi isaret eden foreign key i, CalisanAdres tablosunun id si olsun.

            Bu sekilde bir calısma yapmıs olduk. Lakin CalisanAdres tablosunun id sinin foreing key yapılması durumunda Id nin primary 
            key olma ozelligi ezilir. Bunu da ek olarak belirtmek gerekir.
            modelBuilder.Entity<CalisanAdresi>() yani CalisanAdresi tablosu uzerindesin.
            .HasKey(c => c.ID); yani Id kolonu bu tablonun key idir. 
            Bu calısmayı Calisan entity si uzerinden yaptıgımız fluent api konfigurasyonu uzerinden de yapabilirdik.

            Iste tum bu calısma sonunda Default Convention ve Data Annotations yapılanmalarında olusturulan ilgili iliskinin aynısı
            burada da kurulmus olur.

            Sozun ozu;
            - Nav Property ler kullanılması gerekir.
            - Fluent api yonteminde entity ler arası iliski context sınıfı icerisinde onModelCreating fonksiyonunu override ederek 
            metotlar aracılıgıyla yapılması gerekmektedir. Yani tum sorumluluk developer a bırakılmıstır, tum sorumluluk bu fonksiyon 
            icindeki calısmalara aittir.
             */

            #endregion

            return Ok();
        }
        #endregion
    }

    //class Calisan
    //{
    //    public int ID { get; set; }
    //    public string Adi { get; set; }
    //    public CalisanAdresi CalisanAdresi { get; set; }
    //}
    //class CalisanAdresi
    //{
    //    [Key, ForeignKey(nameof(Calisan))]
    //    public int ID { get; set; }
    //    public string adres { get; set; }
    //    public Calisan Calisan { get; set; }
    //}
}
