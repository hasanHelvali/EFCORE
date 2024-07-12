using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationalStructuresController : ControllerBase
    {
        #region Temel Terminoloji
        [HttpGet]
        public async Task<IActionResult> Relationships_IliskiselTerimler()
        {
            #region Principal Entity (Asıl Varlık)
            /*Bagımsız olan, kendi basına calısabilen tabloyu modelleyen entitiy dir. Alttaki Calisanlar ve Departmanlar 
              sınıflarnın birer entity oldugunu dusunelim. Bu durumda Calisanlar tablosu Departman olmadan olmuyor yani 
              departmana bagımlıdır. Lakin departman tablosu ise baska bir tabloya bagımlı degildir. Tek basına olusturulabilir.
             Buradaki Principal Entity Departmanları temsil eden Departmanlar Entity sidir. */
            #endregion

            #region Dependent Entity (Bagımlı Varlık)
            /*Uatteki tanımdan yola cıkarak anşayabiliriz ki Calisanlar entity si ise Bagımlı entity dir.
             Bir tablonun bir baska tabloya iliskisel olarak baglılıgı varsa bu tablo dependent Entity dir.*/
            #endregion


            #region Foreign Key
            /*Principal Entity ile Dependent Entity arasındaki iliskiyi saglayan key dir. Dependent Entity de tanımlanır.
             Principal Entity deki ID yi yani Principal Key i tutar.
             Ilgili misale bakılırsa buradaki Foreign key yapısı DepartmanID dir.*/
            #endregion

            #region Principal Key
            /*Principal Key ise Principal Entity deki ID nin ta kendisidir.Bir baska deyisle Principal Entity nin kimligi 
             olan kolondur. Modellememize gore Departman sınıfının ID si bizim principal key imiz olur.*/
            #endregion



            #region Navigation Property Nedir
            /*Iliskili tablolar arasındaki fiziksel erisimi saglayuan property lerdir. Calisanlar2 ve Departmanlar2 sınıflarına bakalım.
             Bir iliski kurulmus. Her bir Calisan bir Departmana sahiptir. Lakin Her bir departman bunyesinde birden cok calisan tutabilir.
            Iste bu iliskiyi saglayan 

            public Departmanlar Departmanlar { get; set; }
            public ICollection<Calisanlar2> Calisanlar { get; set; }

            property ler birer Navigation Property dir.
            
            Bir property nin Navigation Property olabilmesi icin o property nin turu kesinlikle bir Entity turunden olmalıdır.
            Lakin iliskisel olmayan, entity bile olmayan bir sınıfta bir entity turunden property varsa bu tabii ki Nav prop olmaz.

            Nav Prop entity ler arasındaki iliskiye dair mesajı veren yapılardır.
             */
            #endregion

            #region One To One (1-1)
            /*Herkesin bir parmak izi vardır. Her parmak izi bir kisiye aittir. ParmakIzi ve Person arasındaki iliski birebir bir 
             iliskidir.*/
            #endregion

            #region One To Many (1-n || n-1)
            /*Modelledigimiz ornegin ta kendisidir. Bir calisan bir departmanda bulunabilir lakin bir departmanda birden cok 
             calısan bulunur.*/
            #endregion

            #region Many To Many (n-n)
            /*Nav prop Koleksiyonel veriler halinde tanımlandıgı yapılardır. Bir calisan birden fazla projede calısabilir.
             bir projenin birden fazla calısanı olabilir. Bu tur bir iliski yapısıdır.*/
            #endregion
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EFCore_IliskiYapilandirmaYontemleri()
        {
            #region Default Convention
            /*EFcore da iliski yapilandirma yontemlerinden biri de Convention yontemidir ki default olarak zaten bu kullanılır.
             Varsayılan entity kurallarını kullanarak yani Nav Prop kullanılarak yapılan iliski cıkarma yontemleridir. Geleneksel
            yontemdir.
            EFCore, ID nin primary key oldugunu isminden tanıması, Nav Prop lerin isminden tanınması vs temel convention dan 
            dolayıdır.*/
            #endregion

            #region Data Annotations Attributes 
            /*Bir diger iliski yapılandırma aracları Data Annotation yapılarıdır. Bildigimiz attribute lerdir. Attribute ler ile 
             hangi property nin primary key oldugunu, hangi tablo ile iliski kurulacagını vs belirleyebiliyoruz.
            Bir property nin yanına ID geliyorsa bu foreign key dir. Lakin ben ID icermeyen herhangi bir icerigi Foreign key yapmak 
            istiyorsam yani convention in dısına cıkıyorsam burada data annotation ları kullanabilirim. 
            Entity nin niteliklerine gore ince ayarlar yapmak istiyosak da kullanabiliriz. */
            #endregion

            #region Fluent API
            /*Entity modellerindeki iliskileri yapılandırırken daha detaylı calısmamızı saglayan ozelliktir, yontemdir.
             Eger fluent api ilem iliski yapılandırması yapıyorsak 4 adet  fonksiyonun bilinmesi gerekir.*/

            #region HasOne 
            /*Bir entity nin baska bir iliskisel entity ile 1-1 veya 1-n iliskisini baslatırken, yapılandırırken bu fonksiyonu kullanırız.
             İliski 1 ile baslıyorsa HasOne kullanırız.*/
            #endregion

            #region HasMany
            /*HasOne tanımından hareketle ilsikinin cok ile yani n ile baslayan halidir. Yani 
             ilgili entity nin iliskisel olan entity ye n-n veya n-1 sekilde iliskisini baslatan fonksiyondur. */
            #endregion

            #region WithOne
            /*HasOne veya HasMany den sonra 1-1 veya n-1 yani birebir veya coka bir sekilde iliskinin yapılandırılmasını 
             tamamlayan metottur. */
            #endregion

            #region WithMany
            /*HasOne veya HasMany den sonra 1-n veya n-n yani bire cok veya coka cok sekilde iliskinin yapılandırılmasını 
            tamamlayan metottur.*/
            #endregion

            #endregion
            return Ok();
        }
        #endregion


    }

    public class Calisanlar()
    {
        public int ID { get; set; }
        public string CalisanAdi { get; set; }
        public int DepartmanID { get; set; }
    }

    public class Departmanlar()
    {
        public int ID{ get; set;}
        public string DepartmanAdi { get; set;}
    }

    public class Calisanlar2()
    {
        public int ID { get; set; }
        public string CalisanAdi { get; set; }
        public int DepartmanID { get; set; }
        public Departmanlar Departmanlar { get; set; }
    }

    public class Departmanlar2()
    {
        public int ID { get; set; }
        public string DepartmanAdi { get; set; }
        public ICollection<Calisanlar2> Calisanlar { get; set; }
    }
}
