using _4_AsNoTracking.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace _4_AsNoTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsNoTrackingController : ControllerBase
    {
        private readonly NorthwindDbContext _context;
        public AsNoTrackingController(NorthwindDbContext context)
        {
            _context = context;
        }
        //Bu bolumde ChangeTracker mekanizmasının davranıslarını yonetmemizi saglayacak fonksiyonları inceleyelim.
        //ChangeTracker mekanizması gelen her nesnenin efcore tarafından takip edilmesiydi. Bunu hatırlayarak baslayalım.

        /*ChnageTracker mekanizması maliyetli bir yapıdır. Mesela biz sadece listeleme yapacaksak ilgili verilerin takip edilmesinin 
         ne anlamı var? Bir anlamı yok. Dolayısıyla bu maliyete katlanmanın da bir anlamı yok. 
        Gerektiginde milyonlarca veri ile islemler yapılabiliyor. Burada katlanılmaz maliyetlerden kurtulmak yazılımcının kaitesini gosterir.
        Iste bu bolumde bu maliyetlerden nasıl kurtuluruz bunlara bakalım.*/

        [HttpGet]
        public async Task<IActionResult> AsNoTracking()
        {
            #region AsNoTracking Metodu
            /* Context uzeirnden gelen verilerin ChangeTracker mekanizması tarafından takip edilmemelerini saglar. 
             Bu fonksiyonun kullanıldıgı sorgulamalarda veriler elde edilir, istenilen noktalarda kullanılabilir lakin 
            veriler uzerinde herhangi bir degisiklik yani update yapamayız. */

            var results = await _context.Urunlers.ToListAsync();
            /*Buradaki verilerin tamamı takip mekanizmasıyla gelir. Lakin biz bunları sadece projekte etmek icin kullanacaz. 
            Dolayısıyla bu veriler icin ChangeTracker mekanizmasını koparmamız gerekiyor.
            Peki bu takibi nasıl koparabiliriz? Veriler halihazırda sorgulanma asamasındayken yani IQueryable asamasındayken AsNoTracking 
            fonksiyonu kullanılırsa verilerin takibi ChangeTracker mekanizması tarafından yapılmaz.*/

            var results2 = await _context.Urunlers.AsNoTracking().ToListAsync();
            /*Bu sekilde elde edilen verilerin takip edilmeden elimize ulasmasını saglayabiliriz.
            Lakin artık bu veriler uzeirndeki degisiklikler db ye yansıtılmaz.*/
            foreach (var result in results2)
            {
                result.UrunAdi = $"Yeni {result.UrunAdi}";//Burada her urunun adinin basına yeni eklendi. Update islemi yapıldı.
            }
            await _context.SaveChangesAsync();
            /*Burada da db ye bir kayıt islemi istendi. Lakin bu degısıklıkler asNoTracking den dolayı db ye yansıtılamazlar.
             Bir baska deyisle de bu fonskiyon cagırıldıgında db ye ilgili update sorgusu gonderilmez.
            Bu degisiklikleri db ye yansıtmanın yolu ise manuel olarak update mekanizmasını tetiklemektir.*/

            foreach (var result in results2)
            {
                result.UrunAdi = $"Yeni {result.UrunAdi}";
                _context.Urunlers.Update(result); //Burada manuel olarak update tetiklendi.
            }
            await _context.SaveChangesAsync();
            /*Simdi ise degisiklikler db ye yansıtılacaktır. Cunku oncesinde update mekanizması manuel tetiklenmistir. Eger bagları 
             koparmasaydık update i tetiklemeden saveChanges i cagırarak update islemi yapabilirdik. Lakin bagı kopardıgımız icin update i 
            manuel olarak tetiklememiz gerek. Bu tetiklemeden sonra ChangeTracker tekrardan ilgili veriler icin devreye girer.
            Bundan sonra saveChanges i cagırarak ilgili degisiklikleri db ye kaydedebiliriz.
            Takip mekanizmasını kullanmadık. Performans artısı sagladık. Lakin bu bize verileri manuel olarak degistirmeye itti ve 
            changeTracker tekrardan etkinlestirildi. Bu islem sadece update icin degil add, remove vs islemleri icin de uygulanabilir.
            Lakin gunun sonunda aynı yere cıkıyor oldugumuzdan dolayı asNoTracking metodunu sadece _context uzerinden gelen veriler
            uzerinde bir degisiklik yapmayacagımız zaman, sadece projekte edecegimiz veriler icin kullanılır.
            kullanıyoruz.*/
            #endregion

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AsNoTrackingWithIdentityResolution()
        {
            /*CT mekanizması yinelenen verileri tekil instance olarak getirir. Burada fazladan bir performans artısı soz konusudur.
             
             Bizler yaptıgımız sorgularda asNoTracking ile takip mekanizmasının maliyetini kırmak isterken bazen maliyete sebeiyet 
            verebiliriz. Cunku takip mekanizması koparıldıgından efcore bellekte hangi instance dan ne kadar vardır bilemez ve her nesne icin 
            bir instance olusturur.  Bu durum ozellikle ilişkisel sorgularda ortaya cıkar. 
            CT mekanizması bunun onune geciyordu. Lakin CT mekanizmasını devre dısı bırakırsak bunun muhasebesini bizim yapmamız gerekecek.
            
             Boyle durumlarda hem takip mekanizmasının maliyetini ortadan kaldırmak hem de yinelenen dataları tek instance uzerinden uzerinden 
            karsılamak istiyorsak AsNoTrackingWithIdentityResolution fonksiyonunu kullanırız. Yani bellek optimizasyonunun zirvesine cıkıyoruz.
            Maliyet acısından bu fonksiyon AsNoTrackng den fazla lakin CT mekanizmasından daha az maliyetlidir.
            
             Daha iyi anlayabilmek icin 
            Kullanıcı1 ve Kullanıcı2 Role1 ile iliskili olsun. Kullanıcı3 ve Kullanıcı4 Role2 ile iliskili olsun. Kullanıcı5 ise Role3 ile 
            iliskili olsun. CT mekanizması Kullanıcı1 ve Kullanıcı2 icin Role1 i ortak kullanır. İki instance uretmez. Aynı durum Kullanıcı3 ve 
            Kullnıcı4 icin Role2 ozelinde de gerceklesir. CT burada kazanc saglar. 
            Lakin AsNoTracking kullanırsak Kullanıcı 1 ve Kullanıcı2 icin iki ayrı Role1 uretilir. Aynısı Kullanıcı3 ve Kullanıcı4 icin Role2 
            ozelinde de yapılır. Cunku efcore, CT bagları koparıldıgı icin bellekte hangi nesneden ne kadar var bilemez. 
            Bu durumda AsNoTracking ile tasarruf saglayalım derken, yani 5 kullanıcı + 3 role nesnesi yani 8 nesne ile durumu kotarabilecekken 
            5 Kullanıcı +5 Role nesnesi ile islem yaparak bellek acısından performans dususuyle karsılasabiliriz. Yani AsNoTracking, CT mekanizmasına 
            gore astarı yüzünden pahalıya gelmis olur.
            Iste tam bu durumda AsNoTrackingWithIdentityResolution fonksiyonu kullanılır. Hem takip yapılmaz, hem tekrarlı veriler icin fazlaca 
            instance olusturulmasının onune gecilir.*/

            var urunler = await _context.Urunlers.Include(k => k.Parçalars).ToListAsync();
            /*Urunler ve Parcalar sınıflarının ctor larına Cw atarak kac kez calıstırıldıklarını buradan gorebiliriz. 
            Bu sorguda her bir parca iliskide oldugu urun icin ortak kullanılır.*/

            var urunler2 = await _context.Urunlers.Include(k => k.Parçalars).AsNoTracking().ToListAsync();
            //Burada ise her bir parca icin bir urun olsuturulur. Burada maliyet artısı olur.

            var urunler3 = await _context.Urunlers.Include(k => k.Parçalars).AsNoTrackingWithIdentityResolution().ToListAsync();
            /*Burada ise her parca iliskide oldugu urun icin ortak kullanılır. Aynı zamanda takip mekanizması da yoktur.
            CT gibi nesne optimizasyonu sagladıgından ve takip mekanizması olmadııgndan CT dan daha az maliyetlidir.
            Gereksiz nesne uretilmesine engel oldugundan ise AsNoTracking den daha az maliyetlidir.
            Bu gibi durumlarda en optimum cozum bu fonksiyon ile saglanır. Iliskisel veriler cekilmiyorsa eger tabii ki AsNoTracking daha 
            performanslıdır. AsNoTrackingWithIdentityResolution ise AsnoTracking ve CT arasında bir performansa sahip olur. */
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AsTracking()
        {
            /*Bu fonksiyon ise AsNoTracking in tersidir. Nesnelerin CT tarafından takip edilmesini istiyorsak bu fonksiyonu kullanırız.
            Context den gelen dataların takip edilmesini iradeli bir bicimde tetiklemek gerekebiliriz. Iste o zaman kullanırılır.
            CT mekanizması zaten default geliyorken bu fonksiyona neden ihtiyac duyulmus? Cunku ChangeTracker ihtiyac halinde koparılır.
            Koparılan bu bagı tekrardan devreye almak icin bu fonksiyon kullanılır. 
            UseQueryTrackingBehavior metodunun davranısı geregi uygulama seviyesinde CT in default olarak devrede olup olmaması da 
            ayrıca ayarlanabilir. Eger ki default olarak pasif hale getirilirse ihtiyac durumlarında tekrardan CT mekanizmasını tekrardan 
            etkin hale getirmek icin kullanabiliriz. Bu fonksiyonda yine sorgu IQueryable haldeyken kullanılır.
             */
            var results = await _context.Urunlers.ToListAsync();//Burada CT default olarak kapalı oldugu varsayılsın.
            var results2 = await _context.Urunlers.AsTracking().ToListAsync();//Bu sekilde CT mekanizmasını devreye alabiliriz.

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> UseQueryTrackingBehavior()
        {
            /*EFCore seviyesnde yani uygulama seviyesinde ilgili context ten gelen verilerin uzerinde CT mekanizmasının davranısı temel 
             seviyede belirlememizi saglayan fonksiyondur.
            Bu fonksiyon DBContext konfigurasyonunda kullanılır.*/
            
            //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            //{
            //     optionsBuilder.UseSqlServer("Data Source=HASANHELVALI;Initial Catalog=NORTHWND; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;" +
            //    "Application Intent=ReadWrite;Multi Subnet Failover=False");
            //    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            //}

            /*Bu sekilde DbContext i konfigure ediyoruz. 
             optionsBuilder uzeirnden cagrılan UseQueryTrackingBehavior fonksiyonu QueryTrackingBehavior adında bir enum bekler.
            Bu enum ise gelecek olan verilerin tracking mekanizması ile iliskisini belirten parametreler icerir.
            
            QueryTrackingBehavior.NoTracking => gelen nesneler default olarak track edilmesin,
            QueryTrackingBehavior.NoTrackingWithIdentityResolution => gelen veriler takip edilmesin ve iliskisel verilerin kullandıgı nesneler  tekrarlanmasın,
            QueryTrackingBehavior.TrackAll => Butun veriler default olarak CT ile takip edilsin,

            seklinde parametreler vererek dbcontext i gelen veriler uzeirnde hangi default tracking davranısını alacagı konusunda 
            konfigure edebiliriz. QueryTrackingBehavior.NoTracking kullanıyorsak artık gelen butun veriler default olarak takip edilmezler.
            Eger takip edilmelerini istiyorsak Iqueryable asamasında verileri cekerken execute etmeden once AsTracking ile verileri cekebiliriz.
            */
            return Ok();

        }


    }
}
