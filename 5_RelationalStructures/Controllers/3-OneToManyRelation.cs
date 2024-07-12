using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _3_OneToManyRelation : ControllerBase
    {
        #region One to Many Relationship | Tüm Detaylarıyla Bire Çok İlişki Yapılanması
        [HttpGet]
        public async Task<IActionResult> OneToManyRelationship()
        {
            #region Default Convention
            /*1-n iliskide 1-1 iliskideki gibi dependent entity de foreign key olustuma zorunlulugu yoktur.
             EFCore bu kolonu kendisi olusturur. Istersek opsiyonel olarak bu kolonu kendimizde olusturabiliriz.
            
            class Calisan //dependent entity
            {
                public int ID { get; set; }
                public string Adi { get; set; }
                //public int DepartmanID { get; set; } opsiyonel
                public Departman Departman { get; set; }
            }
            class Departman
            {
                public int ID { get; set; }
                public string DepartmanAdi{ get; set; }
                public ICollection<Calisan> Calisanlar { get; set; }
            }
            
            seklinde bir entity yapısı migrate edilebilir. Tablo yapısına gelecek olursak 
            
            Departmanlar                    Calisanlar                     
            ID      DepartmanAdi        ID      Adi         DepartmanID

            1       A                   1       Hasan       1
            2       B                   2       Mehmet      2
            3       C                   3       Şuayip      4

            seklinde bir tablo yapısı var ve kayıtlar eklemeye calıstık. Biz Calisanlar tablosunda DepartmanId seklinde bir 
            kolon yani aslında bir foreign key tanımlamamamıza ragmen efcore bunu kendisi tanımladı. Biz tanımlasaydık kendisi bu 
            sorumlulugu ustlenmezdi.
            Kayıtlara gelecek olursak 3 numaralı calisan kaydı yapılmaz, hata verir. Cunku her calisanin bir departmanı olması gerekir.
            Lakin 4 numaralı bir departman Departmanlar tablosunda bulunmamaktadır.
            
             */
            #endregion

            #region Data Annotation
            /*Default Convention da foreign key olusturma durumu opsiyoneldi. Biz olusturabiliyorduk, olusturmazsak efcore bunu 
             kendisi yapıyordu. Bu durum burada da gecerlidir. Lakin bir foreign key olusturmak istiyorsak ve bunu default convention ın 
            dısında yapmak istiyorsak yani DepartmanID degil de DId seklinde bir kolonu foreign key yapmak istiyorsak artık default 
            convention ile bir calısma yapamayız. Burada da Data Annotations lar devreye girer.
            
            class Calisan //dependent entity
            {
                public int ID { get; set; }
                public string Adi { get; set; }
                [ForeignKey(nameof(Departman))]
                public int DID { get; set; }
                public Departman Departman { get; set; }
            }
            class Departman
            {
                public int ID { get; set; }
                public string DepartmanAdi{ get; set; }
                public ICollection<Calisan> Calisanlar { get; set; }
            }

            seklinde bir yapı ile default convention ın dısına cıkarak, custom foreign key olusturabiliriz. Bunun yolu da data annotations 
            kullanmaktan gecer.
            */
            #endregion

            #region Fluent API
            /*Fluent api kullanımında hatırlarsak entity sınıfları uzerinde bir oynama yapmaksızın context class ı uzerinde konfigurasyonlar 
             yaparak tablolar arası iliskiyi ve bu iliskilerin detaylarını betimliyorduk.
            
            class Calisan //dependent entity
            {
                public int ID { get; set; }
                public Departman Departman { get; set; }
            }
            class Departman
            {
                public int ID { get; set; }
                public string DepartmanAdi{ get; set; }
                public ICollection<Calisan> Calisanlar { get; set; }
            }
            
            seklinde bir tablo yapımız olsun.
            
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Calisan>()
                    .HasOne(c => c.Departman)
                    .WithMany(d => d.Calisanlar);

            }
            seklinde ilgili context i konfigure ediyoruz. 
            modelBuilder.Entity<Calisan>() Calisanlar tablosunu ele al.
            .HasOne(c => c.Departman) //1-x bir iliski baslat. Burada ki referans alacagın Nav property ise Departman dir. 
            Dolayısıyla iliskide bulunacagın sınıf Departman ismindeki property nin tipi olan Departman entity sidir.
            .WithMany(d => d.Calisanlar); peki iliskiyi 1' e ne? olarak tanımlayalım. 1-n olarak tanımla demis olduk.
            
            Peki foreign key nerede? Foreign key tanımlamamıza burada da gerek yok. EFCore bunu kendisi halleder. DepartmanID adında bir 
            foreign key i, 1-n iliskisindeki 1'i  baslatan tabloya verir.
            Ama ben DeaprtmanID degil de x adında bir foreign key tanımlamak istersem ne yaparım?
            Bunun icin ise istenilen isimdeki kolon olusturulur.
            class Calisan 
            {
                public int ID { get; set; }
                public string xID { get; set; }
                public Departman Departman { get; set; }
            }
            xID kolonu burada foreign key yapılmak istenen kolon olsun.
            
            modelBuilder.Entity<Calisan>()
           .HasOne(c => c.Departman)
           .WithMany(d => d.Calisanlar)
           .HasForeignKey(c=>c.xID);

            seklinde son satırda bir konfigurasyon daha ekleyerek
            .HasForeignKey(c=>c.xID); yani senin foreign key ini ben soyluyorum, xID senin foreign key in olsun seklinde bir 
            bildirimde bulunuyorum. Bunun sonucunda xID kolonu dependent id nin foreign key i olmus olur.

            */

            #endregion

            return Ok();
        }
        #endregion
    }
    //class Calisan //dependent entity
    //{
    //    public int ID { get; set; }
    //    public int DID { get; set; }
    //    public Departman Departman { get; set; }
    //}
    //class Departman
    //{
    //    public int ID { get; set; }
    //    public string DepartmanAdi{ get; set; }
    //    public ICollection<Calisan> Calisanlar { get; set; }
    //}
}
