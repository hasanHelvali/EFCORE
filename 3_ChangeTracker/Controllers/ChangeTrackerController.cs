using _3_ChangeTracker.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _3_ChangeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeTrackerController : ControllerBase
    {
        #region ChangeTracker Nedir?
        /*Context nesnesi uzerinden gelen butun nesneler veya veriler otomatik ve anlık olarak (snapshot) bir   takip mekanizması ile izlenir . Bu takip 
        mekanizmasına ChangeTracker denir. Bu yapılanma nesneler uzeirndeki degisiklikleri ve islemleri takip ederek bu islemler neticesinde 
        islemlerin yapısına uygun sql sorgularını generate ederler. Bu isleme ise ChangeTracking denir. 
        
         Bu mekanizma bir property ye sahiptir. Context class ının base class ı olan DbContext class ına gidilirse eger buradaki ChangeTracker 
        property gorulebilir. Bu property nin kullandıgı referans private, ancak property nin kendisi public tir. Yani bu property ye hem dbContext ten 
        hem DbContext in child class ı olan, database i programatik olarak modelledigimiz NorthwindDbContext class ından erisebiliyorum,
        hem de NorthwindDbContext class ının nesnesinden erisebiliyorum. Dolayısıyla bu yapıya müdahale edebiliyorum.

        Sozun özü; 
        Bu mekanizma takip edilen nesnelere erisebilmemizi saglayan ve gerektiginde bu mekanizmaya müdahale edebilmemizi saglayan 
        bir property ye sahiptir demis olduk. Nesnelerin durumlarını gorebiliyor ve bu durumlara uygun aksiyonlar alabiliyoruz. Araya bir 
        interceptor mekanizması kurarak girebiliyoruz.
        */
        #endregion

        private readonly NorthwindDbContext _context;
        public ChangeTrackerController(NorthwindDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ChangeTracker()
        {
            #region ChangeTracker Nedir
            var urunler = await _context.Urunlers.ToListAsync();
            var datas = _context.ChangeTracker.Entries();
            /*Bu sekilde context nesnem uzerinden bu nesnemin bir member i olan ChangeTracker a erisebiliyorum. 
             Bu erisim sonucunda Entries fonksiyonunu kullandııgmızda takip edilen butun nesneler Entries() --daha once deginmistik.--
            fonksiyonu ile IEnumerable olarak EntityEntry turunden bize getirilir.
            Db den kac adet nesne alıyorsak o kadar EntityEntry nesnesi bize getirilir cunku default olarak hepsi bir takip mekanizmasına 
            sahiptir. Bunlar datas icine alınır. Bu nesnelerin durumlarına datas dan bakılabilir. İlk kez gelen nesnelerin unchanged olarak 
            tutuldugunu gorebiliriz. */
            urunler[6].Fiyat = "1000"; //Bu sekilde bir islem yaptım. Yani aslında bir update islemi yaptım.
            _context.Remove(urunler[7]);//Burada da bir delete islemi yapmıs oldum.
            /*Iste bu asamada tekrardan datas a bakılırsa degisiklik yapılan nesneler en bastaki indexlere alınmıs ve state lerinin 
             degismis olduklarını gorebiliriz. Bu degisikliklerin bir degisiklik kazanması icin yani db ye aktarılması icin 
            SavrChanges fonksiyonunu kullanmalıyız.*/
            await _context.SaveChangesAsync();
            //Bu sekilde de ilgii state ler sonucunda CT mekanizmasının olusturdugu sorguları db ye generate edebiliriz.


            #endregion

            #region ChangeTracker Members
            //ChangeCtracker mekanizmasının icinde belirli memberlar vardır. Bu member lara deginelim.


            #region DetectChanges Func
            /*Bazı asenkron sureclerde changeTracker mekanizmasının tam anlamıyla dogru calıstıgından emin olamayabiliriz. Bu sebeple
             ChangeTracker mekanizmasını manuel olarak tetikleme ihtiyacı ortaya cıkabiir. Iste bu ihtiyaca binaen DetectChanges fonksiyonu
            gelistirilmistir. Bu fonksiyon acıklamadan anlasılacagı uzere ChangeTracker mekanizmasını manuel olarak tetiklememizi dolayısıyla
            yapılan degisikliklerin kayda gectiginden emin olmamızı saglar.
            SaveChanges ve efcore mimarisi bu degisiklikleri zaten detect eder yani saptar. Ancak biz işi onlara bırakmak istemezsek,
            kontrolu ele almak istersek iste bu durumda detectChanges kullanırız.*/
            var urun = await _context.Urunlers.FirstOrDefaultAsync(x => x.Id== 3);//3 id li urun cekildi.
            urun.Fiyat = "123";//Guncelleme islemi yapıldı.
            _context.ChangeTracker.DetectChanges();//Degisiklikleri DetectChanges aracılıgıyla manuel olarak tetikleyip kontrol ettirdik.
            await _context.SaveChangesAsync();
            //SaveChanges normal sartlarda DetectChanges i otomatik tetikler. Lakin biz işi saglama alarak kendimiz daha oncesinden tetikledik.
            #endregion

            #region AutoDetectChangesEnabled Prop
            /*DetechChanges metodunun oromatik olarak tetiklenmesinin konfigurasyonunu  saglayan property dir. 
             Farkettigimiz üzere changetracker mekanizmasının hem manuel hem de otomatik olarak saveChanges ile 
            tetiklenmesi gereksiz bir maliyet olusturabilir. Biz bu property ye false degerini verdiigmizde artık 
            sveChanges yaparken detectChanges otomatik olarak tetiklenmez. Bu ise efcore da asırı derecede bir maliyet 
            optimizasyonu yapmamızı saglar. ChangeTracker mekanizmasını tamamen kendi irademle yoneteyim, detectChagnes i 
            ihtiyacım varsa cagırayım yoksa cagırmayayım haliyle de buradaki maliyeti kırmıs olayım, maliyetten tasarruf 
            edeyim diyorsak bu yapıyı false a cekebiliriz.
            Kısacası;
            ilgili metotlar(SveChhanges, Entries) tarafından detectChanges metodunun otomatik olarak tetiklenmesinin konfigurasyonunu 
            yapmamızı saglayan property dir.*/
            #endregion


            #region Entries Func
            /*Context teki Entry metodunun koleksiyonel versiyonudur. ChangeTracker mekanizması tarafından izlenen her entity nesnesinin 
             bilgisini entityEntry turunden elde etmemizi saglar ve belirli yapabilmemize yarar.
            Bu metot kendi icerigini calıstırmadan once DetectChanges metodunu tetikler. Bunu entity lerin en guncel hallerine ulasmamız 
            icin yapar. Bu durumda tıpkı saceChanges da oldugu gibi bir maliyettir. 
            Buradaki maliyetten kacınmak icin AutoDetectChangesEnabled Property sine false degeri verilir.*/
            
            var urunler2 = await _context.Urunlers.ToListAsync();
            //urunler2[6].UrunAdi = "araba";
            urunler2.FirstOrDefault(x => x.UrunAdi == "araba").Fiyat = "123";
            _context.Urunlers.Remove(urunler2[7]);
            //seklinde guncellestirmeler yaptım.
            _context.ChangeTracker.Entries().ToList().ForEach(e =>
            {
                if (e.State==EntityState.Unchanged)
                {
                    //bir degisiklik yoksa buradaki islemleri uygula
                }
                else if(e.State==EntityState.Deleted)
                {
                    //ilgili entry nin staty i deleted ise buradaki islemleri uygula vs vs 
                }
            });
            //Ilgili calısmaları saveChanges edip veri tabanına gondermeden once bu sekilde operasyonel davranıslar sergileyebilirim.
            #endregion

            #region AcceptAllChanges Func
            /*SaveChanges() ve SaveChanges(true) fonksiyonları aynı yapılardır. SaveChanges ilk overload unda default olarak zaten 
             true bri parametre alır. 
            Bu metotlardan biri tetiklendiginde efcore her seyin yolunda oldugunu varsayarak track edilen nesnelerin takibini keser ve 
            yeni yapılacak olan degisikliklerin takibini bekler. Boyle bir durumda bir hata meydana gelirse takipm edilen nesnelerin takibi
            bırakılacagından dolayı bir duzeltme yapılamıyor. Yani saveChanges bu sekilde  tetiklendiginde surec basarılı veya basarısız 
            farketmeksizin efcore surecın basarılı oldugunu varsayar ve track edilen nesnelerden takibi keser. Biz ise hangi nesnede ne gibi 
            duzenlemeler yapıldı buna vakıf degilsek veri kaybı yasanır. Iste tum bu nedenler yuzunden SaveChanges onemli yerlerde false ile 
            kullanılır. Ayrıca burada devreye AcceptAllChanges fonksiyonu da girer.
            SaveChanges(false) kullanımında ise track edilen nesnelerin execute islemi basarılı veya basarısız farketmeksizin efcore bu nesnelerden 
            track mekanizmasını koparmıyor. Bu sebeple basarısızlık durumunda gidip veriler uzerindeki onarım islemini yapabiliyoruz.
            Basarı durumunda ise artık track mekanizmasının maliyetinden kurtulmak icin AcceptAllChanges fonksiyonunu kullanıyoruz. Takip islemini 
            irademizle sonlandırıyoruz.
            
             Kısacası AcceptAllChanges fonksiyonu, ChangeTracker ile nesnelerinin takibinin iradeli bir bicimde koparılmasını saglar.
            SaveChanges(false) efcore a gerekli db komutlarını yurutmesini soyler fakat gerektiginde yeniden oynatılabilmesi icin degisiklikleri
            beklemeye/nesneleri takip etmeye devam eder. Taa ki AcceptAllChanges fonksiyonunu irademizle cagırana kadar...*/

            var urunler3 = await _context.Urunlers.ToListAsync();
            urunler2[6].UrunAdi = "araba";
            urunler2.FirstOrDefault(x => x.UrunAdi == "araba").Fiyat = "123";
            _context.Urunlers.Remove(urunler2[7]);

            await _context.SaveChangesAsync(false);
            _context.ChangeTracker.AcceptAllChanges();
            #endregion

            #region HasChanges Func
            /*Takip edilen nesneler arasında degisiklik yapılanların olup olmadıgının checkin ini yani kontrolunu yapar, 
             bunun bilgisini verir. Lakin biraz maliyete sahiptir. Cunku bu kontrolu yapabilmesi icin detectChanges fonksiyonunu 
            arka planda bir kereye mahsus tetikler. Aksi halde nesnelerin son durumlarıyla ilgili guncel bilgileri edinemez. */
            var result =_context.ChangeTracker.HasChanges();//bool turunde bir result doner.
            #endregion





            #endregion
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> EntityStates()
        {
            //EntityState ozelligi entity nesnelerinin durumlarını ifade eder.
            
            #region Detached
            //Nesnenin changeTracker mekanizması tarafından takip edilmedigini ifade eder.
            Urunler urun=new Urunler();
            //Bu nesne db den efcore aracılıgıyla gelmedigi icin CT tarafından takip edilemez.
            var _state = _context.Entry(urun).State; //_context nesnesinin entry fonksiyonu ile ilgili nesnelerin state ine erisebiliriz.
            //Buradaki state Detached olarak gelir.
            urun.Fiyat = "111";//nesnede bir degisiklik yapıldı.
            await _context.SaveChangesAsync();
            //Yukarıdaki nesne takip edilmediginden SaveChanges i cagırmanın bizim icin bir baglayıcılıgı olmaz. Bosuna cagırdık yani...
            #endregion

            #region Added
            //Henuz db ye islenmemis, db ye eklenecek nesneyi ifade eder. SaveChanges islemi ile bir insert sorgusu olusturulacagı anlamına gelir. 
            Urunler urun2 = new Urunler { Fiyat="123",UrunAdi="Urun2"};
            var _state2 = _context.Entry(urun2).State; //Detached
            _context.Urunlers.AddAsync(urun2);//Bu islem neticesinde artık urun2 nesnesi CT tarafından takibe alınır. Cunku context ile ilskilendirildi.
            var _state3 = _context.Entry(urun2).State; //Added
            await _context.SaveChangesAsync();//Bu islem sonucunda db ye ilgili nesneye ait bir insert sorgusu olusturulur ve execute edilir.
            #endregion

            #region Unchanged
            /*Db ile iliskili takip altında bir nesnenin en son guncellendiginden itibaren bir degisiklige ugramadıgını ifade eder.
            Sorgu neticesinde elde edilen tum nesneler baslangıcta bu state degerindedir.*/
            var _state4 = _context.Entry(urun2).State; //UnChanged 
            //Az once db ye eklenen nesne uzerinde bir degisiklik yapılmadıgı icin burada state bir degisiklik yapılmadıgını soyler.
            
            var urunler=await _context.Urunlers.ToListAsync ();//veriler sorgulandı
            var data = _context.ChangeTracker.Entries();//Gelen verilerin state degerlerinin tamamı default olarak Unchanged gelir.
            #endregion

            #region Modified
            //Nesne uzeirnde degisiklik yapıldıgını ifade eder. SaveChanges cagırıldıgında update sorgusun calıstırılacagı anlamına gelir.
            urun2.Fiyat = "111";//Burada ise takip edilen ve daha onceden db ye ekledigimiz veride guncelleme yapıyoruz.
            var _state5 = _context.Entry(urun2).State; //Modified 
            //takip edilen bir nesne uzerinde bir degisiklik yapıldıgında state modified olarak guncellenir.
            await _context.SaveChangesAsync();//execute edildi.
            var _state6 = _context.Entry(urun2).State; //Unchanged
            /*degisiklikler uygulandıgı icin tekrar state sorgulanmak istenirse ilgili state Unchanged olarak gelir.
            Aslında arka planda AcceptAllChanges calıstırılır ve takip edilen nesneler uzeirnden takıp koparılır. Nesneler 
            sonraki degiiklige hazır hale gelirler. Eger 
            
            await _context.SaveChangesAsync(false); seklinde execute islemi yapacak olsaydık eger CT baglantısı kopmadıgı icin bundan 
            
            sonraki 
            
            var _state6 = _context.Entry(urun2).State; //Modified

            seklindeki state sorgusunda ilgili state degismezdi. Cunku nesne hala takip altında kalırdı. Sonrasında

            _conext.ChangeTracker.AcceptAllChanges(); 

            seklinde ilgili butun bagları manuel olarak koparabilirdik. 
            Daha sonrasında tekrar bir state sorgusu yapılınca ilgili state in unchanged a cekilmis oldugunu gorurduk.
            Cunku  _conext.ChangeTracker.AcceptAllChanges(); ile baglar koparılmıs oldu. Nesnelerin CT takibi sonlandırıldı. Resetlendi.
            Yeniden bir takibe hazır hale getirildi. 
             */
            #endregion

            #region Deleted
            _context.Urunlers.Remove(urun2);//urunun silinmesi gerektigini takip mekanizmasına soyluyoruz.
            var _state7 = _context.Entry(urun2).State;//Deleted
            //Burada state deleted olarak guncellenir.
            _context.SaveChangesAsync ();//Ilgili sorgu execute edilir.
            #endregion

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ContextNesnesiUzerindenChangeTrackerDetayları()
        {
            //_context.ChangeTracker ... ile butun entity lere dair CT islemlerini yapabiliriz.
            //_context.Entry(urun); seklinde de spesifik bir entity ye dait CT islemlerinde bulunabiliriz.

            var urun = await _context.Urunlers.FirstOrDefaultAsync(x => x.Id == 55);//bir urun elde edildi.
            urun.Fiyat = "10";
            urun.UrunAdi = "Silgi";//ilgili urun modified/update edildi. 

            //Burada urun nesnesine dair degistirilmeden onceki verilere erisebilirim.

            #region OriginalValues Prop
            /*Ilgili entity nin orijinal degerlerini getirir. Entity uzerinde runtime da bazı degisiklikler yapılmıs olabilir. 
             Bu degisiklikler db ye yansıtılmadan once asıl db de kayıtlı olan guncellenmeden onceki degerlere ulasılmak istenirse
            OriginalValues property si kullanılır.*/
            var fiyat = _context.Entry(urun).OriginalValues.GetValue<string>(nameof(urun.Fiyat));
            var isim = _context.Entry(urun).OriginalValues.GetValue<string>(nameof(urun.UrunAdi));
            //Burada elde edilen degerler db de kayıtlı olan degerlerdir. Her kullanımda db ye sorgu atılır.
            #endregion

            #region CurrentValues Prop
            //Ilgili entity nin db deki degil, heap teki degerini, bellekteki halindeki degerini getirir.
            var fiyat2 = _context.Entry(urun).CurrentValues.GetValue<string>(nameof(urun.Fiyat));//10
            var isim2 = _context.Entry(urun).CurrentValues.GetValue<string>(nameof(urun.UrunAdi));//Silgi
            #endregion

            #region GetDatabaseValues Prop
            //OriginalValues da oldugu gibi db deki degerleir getirir. Farkı ise kolonların degerlerini degil
            //komple entity nin kendisini getirmesidir. Db deki en guncel hali getirir.
            var urun2 = await _context.Entry(urun).GetDatabaseValuesAsync();
            //gelen deger bir PropertyValues tipindedir. Bunu entity ye cast edip kullanabiliriz.
            //urun2.EntityType ile entity turunu ogrenerek donusum saglanabilir.
            #endregion

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTrackerMekanizmasiniInterceptorOlarakKullanma()
        {
            /*Bir veri ekliyoruz, SaveChanges() i cagırıyoruz.
             Bir veri siliyoruz SaveChanges() i cagırıyoruz vs vs bircok yerde ameleus olarak SaveChanges() i 
            kullanıyoruz. DbContext nesnemize gidersek eger SaveChanges() yapısı virtual yani override edilebilir bir yapıda gelir.
            Bu override neticesinde SaveChanges() i interceptor mantıgında calıstırabiliriz. 
            DBContext te bunun konfigurasyonunu yapalım. 
            Ayrıca ilgili kodlar yorum satırı olarak asagıya eklendi. Bakılabilir.*/

            //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
            //{
            //    //Ilgili metodu override ettim. Biz ne kadar saveChanges i tetiklersek tetikleyelim gun sonunda yalnızca bu fonksyon calısıyordu.
            //    //Simdi ise kendi custom islemleirmizi yazalım.
            //    var entries = ChangeTracker.Entries();//Butun entity lerin entry leri elde edildi.
            //    foreach (var entry in entries)//donguye alındı.
            //    {
            //        if (entry.State == EntityState.Added)
            //        {
            //            //Eger db ye bir kayıt eklenecekse oncesinde bu islemleir yap cs cs . Yani araya giriyoruz.
            //            //Bu ise bir interceptor mantıgıdır. Bu sekilde turlu operasyonlar yapılabilir.
            //        }
            //    }
            //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            //}

            return Ok();
        }

    }
}
