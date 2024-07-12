using _2_TemelDüzeySorgulamaYapıları.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace _2_TemelDüzeySorgulamaYapıları.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemelSorgulamaController : ControllerBase
    {
        private readonly NorthwindDbContext _context;

        public TemelSorgulamaController(NorthwindDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> EnTemelSorgulamaYapıları()
        {

            #region MethodSyntax 
            //Eger metotları ullanarak sorgulama yapıyorsak bu bir method syntax uygulamadır.
            var urunler = await _context.Urunler.ToListAsync();//Butun urunler liste olarak getirilir.
            //return Ok(urunler);
            #endregion

            #region QuerySyntax(Linq sorgulama Yapısı)
            var urunler2 = await (from urun in _context.Urunler
                                  select urun).ToListAsync();
            /*Burada kullandıgımız yapı ise query syntax yapıdır. Bu yapı da from ile baslıyoruz. urun adlı bir degisken olusturduk. 
             context teki Urunler tablosundaki yapıları urun degiskeni uzeirne aldık. Daha sonra bu urun nesnesini duruma gore select operasyonlarına 
            tabii tutup Listesini donduk. */
            return Ok(urunler2);
            #endregion

            #region IQueryable nedir
            //Sorguya karsılık gelir. EFCore uzeirnden yapılmıs sorgunun execute edilmemis halidir.
            var a = from urun in _context.Urunler
                    select urun;
            //Bu yapı bir IQueryable yapsııdır. Veri yoktur. Bu sadece bir sorgudur.
            #endregion

            #region IEnumerable nedir
            //EFCore uzeirnden sorgunun calıstırılıp/execute edilip inmemory ye yuklenmis halini ifade eder.
            //IEnumerable aynı zamanda bellekteki koleksiyonel yapılanmaları temsil eden genel ata tiptir.
            var b = await (from urun in _context.Urunler
                           select urun).ToListAsync();
            //Bu da bir IEnumerable yapıdıır. ToListAsync ile sorgunun execute edilio verinin elde edilmis halidir.
            //ToListAsync dıısnda bircok IEnumerable yani execute etme yapısı da  vardır.
            #endregion


            #region Sorguyu Execute edebilmek icin ne yapmak gerekir ve deffered Execution
            /*Az once bahsedildigi gibi IQueryable olan yapıyı IEnumerable a cevirecek metotları kullanmamız gerekir.
             Bunun dısında foreach de kullanılabilir.*/

            var c = from urun in _context.Urunler
                    select urun;
            foreach (var urun in c)
            {
                Console.WriteLine(urun);
            }
            /*Buradaki yapıda foreach bir IEnumerable uzerinde donuyor. IEnumerable iterasyonel verilerin genel bir interface i olarak nitelendirilebilir.
             İteratif bir yapıdır. EFCore burada IQueryable verinin foreach icinde cagrıuldıgında execute edilmesi gerektiigni anlar. Execute eder. IEnumerable a 
            donusturur. Daha sonra ilgili donguyu isletir ve butun urunleri urun icine alıp ekranda yazdırabiliriz. Iste biz buradaki calısmaya 
            
            Deffered Execution (Ertelenmiş Calısma)

            denir.

             */

            int urunId = 5;
            var d = from urun in _context.Urunler
                    where urun.ID > urunId
                    select urun;

            urunId = 200;
            foreach (var urun in d)
            {
                Console.WriteLine(urun);
            }
            /*Bu sorguda istedigimiz sonucu almayız. Oncelikle sorgu urunId>5 e gore filtrelendi. Lakin daha sonra urunId nin degistirilmesi sorguyu etkiler. Cunku 
             sorgu hala olustuurulmamıstır. EFCore da sorgular sablon hazırken degil execute etmeden hemen once uygulanır. Dolayısıyşa foreach de 
            execute edilmeden hemen once urunId 200 ile guncellendigi icin ilgili yapılanma 200 id sine gore filtrelenmistir. Iste tam olarak bu duruma 
            deffered execution denir. 
            Kısacası IQueryable calısmalarında ilgili kod yazıldıgı noktada calıstırılmaz. Execute edildigi yani IEnumerable a cevrilfigi yerde 
            sorgular fiziksel olarak olusturulur. Bu durum da deffered execution olarak tanımlanmıstır.
            
             Deffered execution a sadece foreach degil ToList vs de neden olur. Kısacası IQueryable ın IEnumerable a cevrildigi her yerde bu davranıs olabilir. 
            Bu yuzden yapılan sorgularda dikkat etmek gerekir.*/
            #endregion
        }


        [HttpGet("CogulVeriSorgulamaYapıları")]
        public async Task<IActionResult> CogulVeriSorgulamaYapıları()
        {
            #region ToList
            /*Uretile sorguyu execute ettirmemize yarayan fonksiyondur. IQueryable dan IEnumerable a gecisi saglar.
            Bunun metod ve query olarak 2 turlu syntax i mevcuttur.*/

            //Method syntax
            var urunler = _context.Urunler.ToListAsync();//Uurnler tablosuna select*from atar.

            //Query Syntax
            var urunler2 = from urun in _context.Urunler
                           select urun;//Bu yapı IQueryable dir.
            await urunler2.ToListAsync();//Bu sekilde IENumerable a donusur yani execute edilri.

            //var urunler2 = (from urun in _context.Urunler
            //               select urun).ToListAsync(); //seklinde de yazılabilirdi.
            #endregion


            #region Where
            //Olsuturulan sorguya where sartı eklememizi saglayan bir fonksiyondur.

            //Method Syntax
            var urunler3 = await _context.Urunler.Where(x => x.ID > 500).ToListAsync();
            //id si 500den buyuk olan urunleri getirir.
            var urunler4 = await _context.Urunler.Where(x => x.UrunAdi.StartsWith("a")).ToListAsync();
            //a ile baslayan butun urunleri getirir.
            var urunler5 = await _context.Urunler.Where(x => x.UrunAdi.Contains("araba")).ToListAsync();
            //adında araba text ini iceren butun urunleri getirir.

            //QuerySyntax
            var urunler6 = (from urun in _context.Urunler
                            where urun.ID > 500 || urun.UrunAdi.StartsWith("a") || urun.UrunAdi.Contains("araba")
                            select urun).ToListAsync();
            //Bu ise kompleks bir linq yapısıdır. Id si 500 den buyuk veya a ile baslayan veya adında araba olan urunleri getirir.
            //Buradaki startWith Endwith Contains gibi yapılar aynı zamanda db deki Like yapılarına karsılık gelir.
            #endregion

            #region OrderBy
            //Bilinen sıralama fonksiyonudur. Sorguya sıralama operasyonu ceker.

            //method syntax
            var urunler7 = await _context.Urunler.Where(u => u.ID > 500 || u.UrunAdi.EndsWith("2")).OrderBy(u => u.UrunAdi).ToListAsync();
            /*Burada ise sorgu yapılır. Sorgudan sonra gelen veriler sıralansın diye orderBy kullandık. Ada gore urunleri sırala dedik.
             Daha sonra execute ettik. OrderBy yapısı default olarak Ascending sıralama yapar yani 0-1 A-Z sıralama yapar.*/

            //Query syntax
            var urunler8 = from urun in _context.Urunler
                           where urun.ID > 500 || urun.UrunAdi.EndsWith("7")
                           orderby urun.UrunAdi ascending//zaten default olarak ascending dir ancak istersek yazabilirizde ...
                           select urun;
            await urunler8.ToListAsync();
            //Aynı sorguyu burada yapmıs olduk. Tablo belirlendi. Sartlar belirlendi. Sıralama yapıldı. Secildi. Execute edildi.

            #endregion


            #region thenBy
            //OrderBy üzeirnde yapılan sıralama islemini farklı kolonlarada yapabilmemize yarayan fonksiyondur.

            //Method Syntax
            var urunler9 = await _context.Urunler.Where(u => u.ID > 500 || u.UrunAdi.EndsWith("2"))
                .OrderBy(u => u.UrunAdi).ThenBy(x => x.Fiyat).ThenBy(z => z.ID).ToListAsync();
            //Once urun adına gore sıralama yapar. Aynı olanlar varsa fiyata gore, yine aynı olanlar varsa id ye gore default ascending sıralama yapar.

            //QuerySyntax
            var urunler10 = from urun in _context.Urunler
                            where urun.ID > 500 || urun.UrunAdi.StartsWith("2")
                            orderby urun.UrunAdi
                            orderby urun.Fiyat
                            orderby urun.ID
                            select urun;
            await urunler10.ToListAsync();
            //Aynı yapı bu sekilde de kurulabilir.
            #endregion

            #region OrderBy Descending Ve ThenByDescending
            //Sıfralama isleminde descending olarak sıralama yapmamızı saglayan bir fonksiyondur.

            var urunler11 = await _context.Urunler.Where(u => u.ID > 500 || u.UrunAdi.EndsWith("2"))
            .OrderByDescending(u => u.UrunAdi).ThenByDescending(x => x.Fiyat).ThenBy(z => z.ID).ToListAsync();
            /*Tablo alındu. Like lar ile sart verildi. Daha sonra isme gore tersten aynı dusen varsa fiyata gore tersten, aynı dusen varsa 
             id ye gore normal sorgu yapıldı ve execute edildi.*/

            //Query Syntax
            var urunler12 = await (from urun in _context.Urunler
                                   where urun.ID > 500 || urun.UrunAdi.EndsWith("2")
                                   orderby urun.UrunAdi descending
                                   orderby urun.Fiyat descending
                                   orderby urun.ID ascending
                                   select urun).ToListAsync();
            //Bu sekilde ilgili sorguyu tekrardan guncelleyebiliriz. Query yapıda thenby yerine tekrardan orderBy kullanılır.
            #endregion
            return Ok();
        }

        [HttpGet("TekilVeriSorgulamaYapıları")]
        public async Task<IActionResult> TekilVeriSorgulamaYapıları()
        {
            /*Bu yapılar execute eidldiginde db deki belirli davranıslara uygun tek satırı getiren fonksiyonlardır.*/

            #region Single
            /*Eger ki sorgu neticesinde birden fazla veri geliyorsa ya da hic veri gelmiyorsa hata fırlatan bir yapıdır.
             Yani sadece bir veri gelirse basarıyla calısır.*/

            //Method Syntax
            var urun = _context.Urunler.SingleAsync(x => x.ID == 55);
            /*Verilen sarta gore uygun bir veri varsa getirir. Birden fazla veri varsa veya veri yoksa hata fırlatır. IQueryable doner
            lakin kendi kendini execute eder.*/

            #endregion

            #region SingleOrDefault
            /*Eger ki sorgu neticesinde birden fazla veri geliyorsa exeption fırlarır, hic veri gelmezse null doner.*/

            //Method Syntax
            var urun2 = _context.Urunler.SingleOrDefaultAsync(x => x.ID == 55);
            //Belirtilen sarta gore tek veri geliyorsa sorgu basarılı olur,birden fazla veri gelirse hata fırlatır, hic veri gelmezse null doner.
            #endregion

            #region First
            //Sorgu neticesinde elde edilen bir veya daha fazla verinin ilkini getirir. Hic veri gelmezse hata fırlatır.

            //Method Syntax
            var urun3 = await _context.Urunler.FirstAsync(x => x.ID == 55);
            //Ilgili sarta gore bir veri varsa getirir, birden fazla veri varsa ilkini getirir, hic veri yoksa exeption fırlatır. 
            #endregion

            #region FirstOrDefault
            //Sorgu neticesinde elde edilen bir veya daha fazla verinin ilkini getirir. Hic veri gelmezse null doner.

            //Method Syntax
            var urun4 = await _context.Urunler.FirstOrDefaultAsync(x => x.ID == 55);
            //Ilgili sarta gore bir veri varsa getirir, birden fazla veri varsa ilkini getirir, hic veri yoksa null doner. 
            #endregion

            #region Find 
            /*Bu fonksiyon primary key kolonuna ozel bir hızlı bir sorgu yapmamıza yarayan bir fonksiyondur.
            Ayrıca Find fonksiyonu once bellege, inmemory ye, context e bakar. Burada ilgili veri yoksa db ye gidip sorgu atar.
             Lakin diger fonksiyonlar her zaman db ye sorgu atarlar. Bu Find fonksiyonuna performans optimizasyonu kazandırır.
            Bir diger husus ise find fonksiyonu ile sadece primary key alanlar sorgulanır. Diger kolonlar uzerinde bir sorgu yapılmaz. 
            Diger sorgularda where ile farklı kolonlarda kayıt getirme yapılabilir.
            Yine bir baska husus olarak find fonksiyonu ilgili veriyi bulamazsa null doner. Dıger fonksiyonların davranısları bu durumda degisiklik 
            gosterebilir.
            var urun4 = await _context.Urunler.FirstOrDefaultAsync(x => x.ID == 55);
            seklinde bir sorgu yapılabilir lakin direkt olarak id ile bir arama yapmabilmekteyiz.*/

            //Method Syntax
            var urun5 = await _context.Urunler.FindAsync(55);//Bu sekilde primary key e ozel bir sorgu yazmıs olduk.
            //Bu fonksiyo ile reflection dan vs kurtuluyoruz. Genelde Repository Dessign pattern kullanırken id bazlı sorgulamalarda kullanılır.

            /*Ayrıca bu fonksiyon composite olan yapılanmalarda da hızlıca arama yapabilmemize yarar. Bu composite yapıyı UrunParca sınıfı ve
            context te OnModelCreating kongigurasyonu ile kurdum. Sımdi bu yapıyı kullanalım.*/
            UrunParca urunParca = _context.UrunParcas.Find(2, 5);
            UrunParca urunParca2 = await _context.UrunParcas.FindAsync(2, 5);
            /*Bu sorgu urunID si 2, parcaID si 5 olan bir veriyi bize geitrebilir. Bu sekilde composite yapılanmalarda Find fonksiyonu etkili bir 
             bicimde kullanılabilir.*/
            #endregion

            #region Last
            /*First ile aynı mantıga sahiptir lakin tersidir yani gelen ilk veriyi degil son veriyi alır. Bir veri gelmezse hata fırlatır.
            Lakin bu fonksiyonu kullanırken oncesinde orderby kullanmamız gerekir. Asc veya desc olması farketmez, bir sıralama kuralı olması 
            yeterlidir.*/
            //Method Syntax
            var urun6 = await _context.Urunler.OrderBy(x => x.UrunAdi).LastAsync(u => u.ID > 55);
            //Gelen verilerden en sonuncusunu alır. Veri yoksa hata fırlatır.
            #endregion

            #region LastOrDefault
            /*FirstOrDefault ile aynı mantıga sahiptir lakin tersidir yani gelen ilk veriyi degil son veriyi alır. Bir veri gelmezse null doner.
            Lakin bu fonksiyonu kullanırken oncesinde orderby kullanmamız gerekir. Asc veya desc olması farketmez, bir sıralama kuralı olması 
            yeterlidir.*/

            //Method Syntax
            var urun7 = await _context.Urunler.OrderByDescending(x => x.UrunAdi).LastOrDefaultAsync(u => u.ID > 55);
            //Sorguda gelen verilerden en son kuruga ekleneni getirir. Verim yoksa null doner.
            #endregion

            return Ok();
        }

        [HttpGet("DigerSorgulamaFonksiyonları")]
        public async Task<IActionResult> DigerSorgulamaFonksiyonları()
        {
            #region Count
            /*Olusturulan sorgunun execute edilmesi neticesinde kac adet satırın elde edilecegini sayısal olarak (int) elde etmemizi 
             saglayan fonksiyondur. Sartlı sorgu da yapılıp sayısı ogrenilebilir.*/
            int sayi = (await _context.Urunler.ToListAsync()).Count();
            //Bu sekildeki bir sorguda once butun bilgiler inmemory ye cekilir ve sonradan sayısı alınır. Bu maliyetli bir surectir.
            int sayi2 = await _context.Urunler.CountAsync();
            /*Bu sekildeki bir sorguda ise direkt olarak sql sorgusunun icine count yazılır. Hicbir veri getirilmeden veri sayısı db den gonderilir.
            Dolayısıyla bu sorgu cok daha az malityetli bir sureci ifade eder.*/
            int sayi3 = await _context.Urunler.CountAsync(x => x.ID > 55);
            //Bu ise sartlı sorgu halidir.
            #endregion

            #region LongCount
            /*Diyelim ki bizim 1 milyar tane verimiz var. Daha dogrusu int veri turunun karsılayamayacagı kadar bir verimiz var. Bu kadar verinin sayısını 
             almak icin ise LongCount kullanırız. Kısacası 
            olusturulan sorgunun execute edilmesi neticesinde kac adet satırın elde edilecegini sayısal olarak (long) elde etmemizi 
             saglayan fonksiyondur.*/
            long sayi4 = await _context.Urunler.LongCountAsync();//Bu sekilde basitce kullanılabilir.
            long sayi5 = await _context.Urunler.LongCountAsync(x => x.Fiyat >= 5000);//Bu sekilde sartlı sorgu da yapılabilir.
            //Tahmin edilecegi gibi sartlı sorgularda db raw sql e bakılırsa sorguya where operasyonu uygulanır.
            #endregion

            #region Any 
            //Sorgu neticesinde veirnin gelip gelmedigini bool turunde belirten fonksiyondur. true ise veri var, false ise veri yoktur.
            bool urunVarMı = await _context.Urunler.AnyAsync();
            //Bu sekilde kullanılır. Arkada Cast when yapısını calıstırır. Geriye 1 ve 0 dondurur. 1 c# taki true ya 0 ie false karsılık gelir.
            bool urunVarMı2 = await _context.Urunler.Where(u => u.UrunAdi.Contains("a")).AnyAsync();
            //Bu sekilde sartlı sorgu da yapılabilir. Arkada where ve cast when fonksiyonlarını calıstırır.
            bool urunVarMı3 = await _context.Urunler.AnyAsync(x => x.Fiyat > 10000);
            //Bu sekilde sartı any icerisinde de yazabiliriz
            #endregion

            #region Max
            //olusturulan soruda ilgili kolonda varolan en yuksek degeri bize doner.
            var fiyat = await _context.Urunler.MaxAsync(x => x.Fiyat);//En yuksek fiyatı bize fiyatın turunde getirir.
            /*Araya baska yapılar atılabilir ve max oyle de alınabilir. IQueryable yapılarda calısırken bu esneklik vardır.
            Biz en optimizasyonlu secenekleri kullanmalıyız.*/
            #endregion

            #region Min
            var fiyat2 = await _context.Urunler.MinAsync(x => x.Fiyat);//En düsük fiyatı bize fiyatın turunde getirir.
            /*Araya baska yapılar atılabilir ve max oyle de alınabilir. IQueryable yapılarda calısırken bu esneklik vardır.
            Biz en optimizasyonlu secenekleri kullanmalıyız.*/
            #endregion

            #region Distinct
            //Sorguda mükerrer yani tekrarlı kayıtları tekillestiren bir fonksiyondur. Arkadaki sorguya Dist keyword unu ekler.
            var urunler = _context.Urunler.Distinct();//Tekrarlı kayıtlardan sadece bir tanesini alır. IQueryable doner. Yani hala sorgu kısmındayız.
            urunler.ToListAsync();//Bu sekilde execute ediyoruz.
            #endregion

            #region All
            /*Bir sorgu neticesinde gelen verilerin, verilen sarta uyup uymadıgını kontrol etmektedir. Eger ki tum veriler sarta uyuyorsa true, 
             uymuyorsa false doner.*/
            var urunler2 = await _context.Urunler.AllAsync(u => u.Fiyat > 5000);//Butun urunlerin fiyatı 5000 den buyukse true degilse false doner.
            /*Arka plandaki sorguda notexist ile 5000den kucuk veri var mı kontrol eder. varsa 0 yoksa 1 dondurur. Bu daperformanslı calısmasını 
             saglıyor.*/
            var urunler3 = await _context.Urunler.AllAsync(u => u.UrunAdi.Contains("a"));//Butun urunlerin adında a varsa true yoksa false doner.
            #endregion

            #region Sum
            //Verilen bir sayısal kolonun/property nin toplamını alır. Toplam fonksiyonudur.
            var top = await _context.Urunler.SumAsync(u => u.Fiyat);//Urunler tablosundaki her urune karsılık fiyatın kümülatif toplamını dondurur.
            #endregion

            #region Average
            //Verilen sayısal property nin aritmetik ortalamasını alır. Kusurat ortaya cıkabilecegi icin float cinsinden bir deger dondurur.
            var avg = await _context.Urunler.AverageAsync(u => u.Fiyat);
            //Urunler tablosundaki her urune karsılık fiyatın kümülatif toplamını ortalamasını dondurur.
            #endregion

            #region Contains
            //İcinde gecen seklinde Like sorgusu '%...%' olusturmamızı saglar.
            /*var urunler4= await _context.Urunler.Contains()... seklindeki sorgu kastedilmez.
            Cunku sql komutlarında da araya bir like girecekse bunu where ile yaparız. 
            Select*From Where Kolon Like '...'  ... seklinde bir sorgudan bahsediyoruz. Bu sebeple Burada where ile kullanılan 
            contains den bahsediyoruz.*/

            /*EFCore where sorgusu icinde contains kullanılınca bunun bir like sorgusu oldugunu anlar ve IQueryable sorgusunda olundugu surece 
             bunu generate edilmesi gereken sorguya ekleyecektir.*/
            var urunler4 = await _context.Urunler.Where(x => x.UrunAdi.Contains("a")).ToListAsync();
            //Bu sekilde hedef veriler elde edilmis olur.
            #endregion

            #region StartWith
            /*Bir like sorgusu olusturup istenen deger ile baslayanları '...%' getirmemizi saglar. Yine raw sorguda like kullanılabilmesi icin
            Where icinde kullanılması gerekir.*/
            var urunler5 = await _context.Urunler.Where(x => x.UrunAdi.StartsWith("a")).ToListAsync();
            //Bu sekilde ilgili veriler elde edilmis olur.
            #endregion

            #region EndWith
            /*Bir like sorgusu olusturup istenen deger ile bitenleri '%...' getirmemizi saglar. Yine raw sorguda like kullanılabilmesi icin
            Where icinde kullanılması gerekir.*/
            var urunler6 = await _context.Urunler.Where(x => x.UrunAdi.EndsWith("a")).ToListAsync();
            //Bu sekilde ilgili veriler elde edilmis olur.
            #endregion

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SorguSonucuDonusumFonksiyonları()//Onemli
        {
            /*Bu kısımda ise elde edilen verileri farklı bir sekilde projeksiyon elde edebilmemizi saglayacak olan farklı bir deyişle 
             elde edilen verileri farklı ture donusturmemize yarayacak olan fonksiyonları gorelim.*/

            #region ToDictionaryAsync
            //Bir sorgu sonucunda ToList ile execute islemini yapıyorduk. Bu fonksiyonlar ise ToList e alternatif execute yontemlerindendir.
            var urunler = await _context.Urunler.ToDictionaryAsync(u => u.UrunAdi, u => u.Fiyat);
            /*ToDictionary diyerek ilgili verileri sozluk formatında cekebiliyoruz. Bu fonksiyonun 3.overload unda once bir key seciyoruz.
             Sonra bu key e karsılık bir value degeri seciyoruz. Gelen veri formatı 
            {[Urun1,1000],
            [Urun2,2000],
            [Urun3,3000],
            ...
            }
            seklinde bir veri formatı elde etmis oluruz. ToList gelen veriyi List<TEntity> formatına donustururken ToDictionary ise gelen sorgu 
            neticesini Dictionary turunden bir koleksiyona donusturur.
             */
            #endregion

            #region ToArray
            //Bu fonksiyonda toList in bir muadilidir yani calıstırılan sorguyu execute eder. Gelen veriyi ise TEntity turunde bir dizide tutar.
            var urunler2 = await _context.Urunler.ToArrayAsync();
            /*Bu fonksiyonun kaynak kodu
             async Task<TSource[]> ToArrayAsync<TSource>(... seklindedir. Yani diger yapılar gibi T turune baglı olarak T turu verildiginde gerite bir 
            T turu donerim der. Yani Urunler uzerinde calısıyorsak eger geriye donulecek yapı yine Urunler tipinde bir yapı olur.*/
            #endregion

            #region Select
            //Daha once sorgularda btuun kolonları cekiyorduk. Bu fonksiyon ise belirli kolonları ceker ve farklı davranısları vardır.

            //1-Generate edilecek sorgunun cekilecek kolonlarını ayarlamamızı saglar.
            var urunler3 = await _context.Urunler.ToListAsync();
            /*urunler3.Select()... bu sekilde de kulanılabilir lakin bu sorguyu etkilemez. Veriler zaten gelmistir. Bizim amacımız
            gerekli kolonları cekip performanslı bir sorgu yapmaktır. Bu sebeple Select ifadesi ToList den once kullanılmalıdır.
            Select ifadesinden sonra IQueryable devam eder. Dolayısıyla nasıl istenirse o sekilde execute edilebilir. Ister ToList ister baska bir 
            fonksiyon kullanılablir.*/
            var urunler4 = await _context.Urunler.Select(x => x.UrunAdi).ToListAsync();//Bana gelecek urunun sadece UrunAdi bilgisini getir demis oldum.
            var urunler5 = await _context.Urunler.Select(x => new Urun
            {
                ID = x.ID,
                Fiyat = x.Fiyat
            }).ToListAsync();
            /*Bana gelecek urunun sadece id ve fiyat bilgisini getir demis oldum. Bos kalan prop lara null olarka kalır. Veya uygun baska 
             bir nesneden instance alınabilir
            */

            //2-Select fonksiyonu gelen verileri farklı turlerde karşılamamızı saglar. Bu turler herhangi bir T turu veya anonim bir tur olabilir.
            var urunler6 = await _context.Urunler.Select(x => new
            {
                Key = x.ID,
                Ad = x.UrunAdi,
            }).ToListAsync(); //Bu sekilde ilgili yapılar birer anonim türe atanabilir. Boylece luzumsuz yere null deger alan propert ler de olmaz.
                              //urunler6[0].Key //Bu sekilde ilgili alanlara da erisebilirim.
                              //Ayrıca bu sorguda da sadece istenen sadece istenen kolonlar sorguya dahil edilir. Bu da maliyeti dusurur.

            var urunler7 = await _context.Urunler.Select(x => new UrunDTO
            {
                Key = x.ID,
                Price = x.Fiyat,
            }).ToListAsync();
            /*Veya bu sekilde ilgili baska bir ture deger giydirme islemi yapılabilir. Yine bu sorgunun da maliyeti istenen kolonlar
            cekildiginden dolayı daha dusuktur*/

            #endregion

            #region SelectMany
            //Select ile aynı amaca hizmet eder.Lakin iliskisel tablolar yoluyla gelen koleksiyonel verileri de tekillesitirip projeksiyon etmemizi saglar

            var urunler8 = await _context.Urunler.Include(u => u.Parcalar).Select(x => new Urun//T Type
            {
                ID = x.ID,
                Fiyat = x.Fiyat,
                //Parcalar=x.Parcalar...
            }).ToListAsync();
            var urunler9 = await _context.Urunler.Include(u => u.Parcalar).Select(x => new //Anonimous Type
            {
                x.ID,
                x.UrunAdi,
                x.Fiyat,
                //x.Parcalar...
                //Parcalar=x.Parcalar...
            }).ToListAsync();

            /*urunler8 ve urunler9 a bakalım. Her bir urune ait adı, fiyatı ve iliskisel olarak urunlere baglo bulunan parcanın adını tek 
             nesneye yazdıracak sekilde elde etmek istiyorum. Iste bu durumlarda kullanılır.
            Sorguya bakıldıgı zaman yeni olusacak olan urun nesnesinin Parcalar property sine bir deger atamak istedigimde x.Parcalar bana bir koleksiyonel 
            veri olarak geliyor. Dolayısıyla bu urunun parcalarının hangi parcasının adını yazdıracam ? Yapamıyorum. Parcalar. deyip herhangi bir parcaya 
            gidemiyorum. x.Parcalar.ToList()[0].ParcaAdi vs gibi bir sorguyu tercih etmek icin bir suanlık yok.
            Iste boyle bir durumda yapılması gereken SelectMany kullanmaktır. */

            var urunler10 = await _context.Urunler.Include(u => u.Parcalar).SelectMany(x => x.Parcalar, (u, p) => new
            {
                u.ID,
                u.UrunAdi,
                u.Fiyat,
                p.ParcaAdi
            }).ToListAsync();
            /*Burada SelectMany kullanıldı. Bu kullanılan yapı 7.overload unda bizden ilk parametre olarak ilgili sorguda 
             lazım olan koleksiyonel yapılanmayı alır. Daha sonra üzerinde çalısılan veri kümesinin temsilini ve bu veri kumesiyle 
            iliskili olan diger veri kumesinin temsilini alır. Yani (u,p) yapısında u ile urunler alınır. p ile parca alınır.
            Bu referanslar uzerinden ilgili sorguda urun ve urun ile iliskili olan parcalar yapılanması yani parcalar tablosu 
            sorguda kullanılabilir.
            
             Bu yapının arkada yaptıgı Sql sorgusu 
            
            SELECT [u].[ID],[u].[UrunAdi],[u].[Fiyat],[p].[ParcaAdi]
            FROM [Urunler] as [u]
            inner join [Parcalar] as [p] on [u].[ID]==[p].[UrunID]
            
            seklindedir. Yani bir join islemi uygulanmaktadır. Kompleks bir yapıdır. Arkada tablolar birlestirilerek bir sonuc dondurulur.
            */

            /*Kısacası iliskisel veriler uzerinden de bu sekilde tekil nesneler olusturmak bu sekilde bir projeksiyon ortaya koymak
            istiyorsak SelectMany fonksiyonu kullanırız. Cok yogunlukla kullanırız.
            Select fonksiyonu ilgili tabloya ozel bir projeksiyon saglarken SelectMany ile iliskisel yapılanmalar da projeksiyonu saglayabiliyoruz.*/

            #endregion
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GroupByFonksiyonu()
        {
            /*Sorguya GroupBy operasyonunu cekmemizi saglayan bir fonksiyondur.
            Aynı fiyat bilgisine saip kac tane urunum var? gibi bir soruya cevap aradıgımızı dusunelim.
            Bu ihtiyaca gore veri tabanında bir kolona karsılık bir agregate fonksiyonu calıstıırlmalıdır. Yani
            
            SELECT Fiyat, Count(*) FROM Urunler
            
            gibi bir sorgu yapısına ihtiyacımız var. Burada Urunler tablosundan fiyat kolonuna dair kac kayıt var sorgusunu 
            yapmıs olduk. Lakin bu sorgu patlar. Cunku bu tip bir sorguda yani bir kolona karsılık bir agregate fonksiyonu varsa burada
            group by kullanmamız gerekir. Ilgili sorgu

            SELECT Fiyat, Count(*) FROM Urunler
            GROUP by Fiyat

            seklinde guncellenir. Bu sorgu sonucunda fiyatlar gruplandırılır. Gelen sonuc 
            Fiyat    NoColumnName
            1000     1
            2000     2
            3000     3
            seklinde olur.
            Yani 1000 liralık urunler gruplandırıldı ve sayısı 1 tane, 2000 liralık urunlerın sayısı 2 tane vs anlamını tasır.
            Iste group by kullanımının mantıgı budur.En ozet haliyle istatistiksel verileri bize sunar.
            Bize gelecek olan yapılar ise artık urunler degil veriler olmus olur. Simdi bu islemin efcore karsılıgını gorelim.
             */

            //Method Syntax
            var datas = _context.Urunler.GroupBy(u => u.Fiyat);
            /*GroupBy fonksiyonu ilgili T entity ye karsılık olarak ilgili kolonu alır. Entity bazı overload larda ilk parametrelerde de 
            verilebilir. Lakin biz _context.Uurnler diyerek tip guvenlikli calıstıgımız icin bu kolon ismini vermem yeterlidir.
            Bu sorgu urunler tablosunu fiyatlara gore grupla demektir. Sorgu bu haliyle hala bir IQueryable dir. Yani execute 
            edilmemistir. Dolayısıyla bundan sonra projeksiyon yapmam gerekir. Yani eldeki verileri secıp gostermem, elde etmem gerekir.
            Veriden bilgileri elde etmeye baslayalım.*/
            await datas.Select(group => new
            {
                Count = group.Count(),
                Fiyat = group.Key
            }).ToListAsync();
            /*Bu sorguda gelen veriler group uzerine alınır. Oncelikle anonim bir tur olusturdum. Bu türun Count property sinde gelen 
             verilerin sayısını aldım. Fiyat property sine ise ,sorguda secilen Fiyat kolonu atandı ve bu kolon ilgili sorguda group nesnesindeki
            Key e karsılık gelir. Yani Key property sinde gruplama islemini yaptıgımız kolonu elde ederiz. Bundan sonrası ise sorguyu execute 
            etmektir. Bu sekilde istenen veriler elde edilmiş olur. Db de yapılan az önceki sorgu ile bu yapının bir farkı yoktur. Cıktı aynıdır.*/

            //Query Syntax
            //Aynı yapıyı linq ile kuralım.
            var datas2 = await (from _urun in _context.Urunler
                                group _urun by _urun.Fiyat //urunu grupla, hangi kolonu? fiyat kolonunu 
                         into @group //ilgili gruba bir isim verdik. group bir keyword e karsılık geldiginden direkt kullanılmaz.
                                     //Basına @ getirilir. Lakin isim yerine grup vs deseydik @ kullanmaya gerek kalmazdı.
                                     //select grup; //direkt olarak grup da geri dondurulebilirdi. Lakin ben secim yapmak istiyorum.
                                select new//gruplanan veriyi istenen sekilde sunuyoruz
                                {
                                    Fiyat = @group.Key, //gruplanan kolon burada da key icinde verimistir.
                                    Count = @group.Count()
                                    //Gruplanan kolona karsılık yani Key e karsılık yani o an ki fiyata karsılık akc adet urun varsa sayısını getiriyorum.
                                }).ToListAsync();//Bu yapı hala bir IQueryable yapıdır. Tum bu yapıyı paranteze alıp veya baska bir satırda execute edebiliriz.

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ForeachFonksiyonu()
        {
            /*Bu fonksiyon bir sorgulama fonksiyonu degildir.ü
            Lakin bu fonksiyon cogul veri getiren fonksiyonlardan sonra, ozellikle cogul veri getiren fonksiyonlardan sonra kullanılabilir.
            Kullanılmak istenebilir. Bu sebeple buna deginmemiz bize katkı sunacaktır.
            Sorgulama neticesinde elde edilen veriler uzerinde iterasyonel olarak donmemizi ve teker teker verileri elde edip islemler
            yapabilmemize yarayan bir fonksiyondur. En kısa tabirle foreach dongusunun fonksiyon halidir.*/

            var datas = await (from _urun in _context.Urunler
                                group _urun by _urun.Fiyat 
                                into grup
                                select new
                                {
                                    Fiyat = grup.Key, 
                                    Count = grup.Count()
                                }).ToListAsync();
            foreach (var data in datas)
            {
                //ilgili islemler
            }
            datas.ForEach(x =>
            {
                //yapılacak islemler.
                //x.Fiyat vs iligli peroperty lere erisebiliriz.
            });
            return Ok();
        }
    }



    public class Urun
    {
        public int ID { get; set; }
        public string UrunAdi { get; set; }
        public int Fiyat { get; set; }

        public ICollection<Parca> Parcalar { get; set; }
    }

    public class Parca
    {
        public int ID { get; set; }
        public string ParcaAdi { get; set; }
    }

    public class UrunParca()
    {
        public int UrunID { get; set; }
        public int ParcaID { get; set; }
        public Urun Urun { get; set; }
        public Parca Parca { get; set; }
    }

    public class UrunDTO
    {
        public int Key { get; set; }
        public int Price { get; set; }
    }
}
