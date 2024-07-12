using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _6_IliskiselSenaryolardaVeriGuncellemeDavranislari : ControllerBase
    {
        /*Burada kast edilen sey bir satırdaki verilerin guncellemmesi degil iki iliskisel tablolar arasındaki verilerin iliskilerinin 
         guncellenmesidir. Butun  bu iliskisel senaryolarda verilere ihtiyac olacak. Bu verilere sabing adlı region lardan ualsılabilir.
        */
        AppDBContext db=new AppDBContext();

        #region OneToOne İlişkisel Senaryolarda Veri Guncelleme
        public async Task<IActionResult> OneToOneRelationChange()
        {
            #region Saving
            _Person person = new _Person()
            {
                Name = "Hasan",
                Address = new _Address()
                {
                    PersonAddress = "Merkez/Bayburt"
                }
            };
            _Person person2 = new _Person()
            {
                Name = "Hasan",
            };
            await db.AddAsync(person);
            await db.AddAsync(person2);
            await db.SaveChangesAsync();
            #endregion

            return Ok();

            #region 1-Durum | Esas Tablodaki Veriye Bagımlı Veriyi Degistirme
            /*Bir tabloda ilgili veriyi yanlıs eklemis olabiliriz ve bu yanlıs eklenen veriyide guncellemek isteyebiliriz.
             Bu durumda veride bazı kolonlar guncellenebilir. Bu bilinen veri guncelemedir. Lakin biz burada bundan bahsetmiyoruz. 
            Bizzat iki farklı veri arasındaki iliskiyi guncellemekten bahsediyoruz. 
            Bu durumda iliskisi degisecek olan verinin kolonları uzerinde bir oynama yapılmaz. Bu durumda yapılan sey veriler arasındaki 
            iliskiyi koparmakta degildir cunku tutarlılık saglanmaz, dependent olan ve bagı koparılan bu verinin esas tabloda karsılıgı olmmaz.
            Bu durumda yapılan sey iliskisi degistiirilmek istenen dependent tablodaki veriyi  komple silip yeni bir veri ve dolayısıyla 
            yeni bir iliski eklemektir. */

            //_Person _person = await db.Persons.FirstOrDefaultAsync(p=>p.ID==1);
            //oncelikle veri elde edilir. Lakin bu perosn tek basına bana yetmez. Bunun address bilgisi de lazım. Bu yuzden join kullanıyorum.
            _Person ?_person = await db.Persons.
                Include(p=>p.Address)//sorguya address de eklendi.
                .FirstOrDefaultAsync(p => p.ID == 1);
            //veri getirildi

            db.Addresses.Remove(_person.Address);
            //address tablosundan ilgili veriyi komple kaldırdık.
            _person.Address = new()
            {
                PersonAddress = "Gencosman Mahallesi/Bayburt"
            };//farklı bir veri daha ekledik. Burad abir guncelleme islemi yapıldı. 
            await db.SaveChangesAsync();//guncelleme islemi yapıldıgından dolayı artık tek yapılması gereken saveChanges i cagırmaktır.
            /*Buradaki tek mesele persondaki address bilgisini guncellemek ise basıt bir guncelleme islemi yapılabilir. 
             Lakin burada yapılmak istenen nesneler arası iliskiyi guncellemektir.*/
            #endregion

            #region 2-Durum | Bagumlı Verilerin İliskisel Oldugu Ana Veriyi Guncelleme
            /*Mesela bagımlı veri ana veride farklı bir veri ile iliskilendiirlmis olsun. Burada aklımıza ilk gelen bagımlı verinin esas 
             tabloya karsılık gelen id sini silmektir. Lakin burada bir silme islemi yapılamaz. Donulen hata da oncelikle bagımlı verinin 
            silinip sonradan tekrardan eklenmesini onerir.*/
            _Address _address=await db.Addresses.FindAsync(1);//address bilgisi elde edildi.
            db.Addresses.Remove(_address);//db den silindi.
            await db.SaveChangesAsync();//db guncellendi.

            //address verisi db den silindi ancak inmemory de hala elimde.
            _Person person3 = await db.Persons.FindAsync(2);//2id li person elde edildi.
            _address.Person=person3;//Bu sekilde yeni bir iliskilendirme yaptım.
            /*biz burada mevcut bir person ile iliskilendirme yaptık. Burada address e de yeni bir person ekleyebilirdik.
            Bu ise yeni bir person eklenmesine neden olurdu.
            _address.Person=new ()
            {
                Name="A kisisi"
            };//Burada person i inline olarak olusturduk. 
            
            await db.Addresses.AddAsync(_address);//address i tekrardan db ye ekledim.
            await db.SaveChangesAsync();//db yi guncelledik
            
            bu sekilde de islemi yapabilirdik. Lakin burada db den silinen address bilgisini farklı bir person ile yeniden ekliyoruz 
            ve iliski kuruyoruz. Buna dikkat edelim.
             */

            await db.Addresses.AddAsync(_address);//address i tekrardan db ye ekledim.
            await db.SaveChangesAsync();//db yi guncelledik.
            //Aslında eski address i sildik. Yerine yeni bir address ekledik. Eski iliski kayboldu, yeni iliski baslatıldı.
            


            #endregion

        }
        #endregion

        #region OneToMany İlişkisel Senaryolarda Veri Guncelleme
        public async Task<IActionResult> OneToManyRelationChange()
        {
            #region Saving
            _Blog blog = new()
            {
                Name = "Hasan",
                Posts = new List<_Post>()
                {
                    new _Post() {Title="1.Post"},
                    new _Post() {Title="1.Post"},
                    new _Post() {Title="1.Post"}
                }
            };
            await db.Blogs.AddAsync(blog);
            await db.SaveChangesAsync();
            #endregion
            return Ok();
            #region 1-Durum | Esas Tablodaki Veriye Bagımlı Veriyi Degistirme
            /*/Yine aynı mantık. Varolanları silmeliyiz. Yeni verileri ekleyebiliriz ve bu bize yeni bir iliskiler getirebilir.
            Once blog u ve bu blog ile iliskide olan butun post ları elde edecez. Burada yan hangi post ları degistireceksek onları silip 
            yerine  baska post lar ekleyecez. Ya da illa bir seyleri silmeden yeni postları ekleyecez. */
            _Blog _blog2 = await db.Blogs
                .Include(b=>b.Posts)
                .FirstOrDefaultAsync(b=>b.ID==1);
            /*ilgili blog kaydı post lari ile beraber artık elimde.
            Diyelim ki bu blog da 2 id sine sahip olan post u silecem ve 4.Post ve 5.post title ina sahip yeni post lari ekleyecem. */
            _Post silinecekPost = _blog2.Posts.FirstOrDefault(p=>p.ID==2);//_blogs2 nesnesinin post ları sorguya dahil edildigi icin dolu gelir. 2 id li post u elde ettim.
            _blog2.Posts.Remove(silinecekPost);//bu post u blog lardan kaldırdım.
            _blog2.Posts.Add(new() { Title="4.Post"});
            _blog2.Posts.Add(new() { Title="5.Post"});
            /*İlgili blog ile yeni post lar iliskilendirildi.
            Eger buradaki Posts dolu gelmeseydi de hata almazdık cunku _Blog class ında bir post ları ctor da baslattık. Bakılabilir.*/
            await db.SaveChangesAsync();//db ye aktarım yapılıyor.

            //Bu sekilde islemler yapılabilir. 2id li post ile blog iliskisi kesildi. Yeni post lar ile iliskiler eklendi.
            #endregion

            #region 2-Durum | Bagumlı Verilerin İliskisel Oldugu Ana Veiryi Guncelleme
            /*Mesela 5.post 1 degerine sahip Blogid ile iliskilendirilmis olsun. Lakin ben bunu aslında 2 degerli blog id ile 
             iliskilendirmek istiyor olayım. Bunun gibi senaryolardan bahsediyoruz.*/

            _Post post = await db.Posts.FindAsync(1);//ilgili post elde edildi.
            post.Blog = new _Blog
            {
                Name = "2.Blog"
            };//post ile yeni bir blog nesnesi iliskilendirildi. Burada yeni bir blog olusturulur. İlgili post 2.blog ile iliskilendirilir.
            await db.SaveChangesAsync();

            //Peki mesela 5.post u direkt 2.blog a verelim.
            _Post post2 = await db.Posts.FindAsync(5);//ilgili post elde edildi.
            _Blog blog1 = await db.Blogs.FindAsync(2);//ilgili blog u elde ediyorum.
            post.Blog = blog1;//iliskilendiriyorum.
            await db.SaveChangesAsync();//db ye ilgili update i aktarıyorum. 

            //Bu sekilde islemler de yapabiliriz. 5.Post direkt olarak 2.blog ile iliskilendirilmis oldu.
            #endregion
        }
        #endregion

        #region ManyToMany İlişkisel Senaryolarda Veri Guncelleme
        public async Task<IActionResult> ManyToManyRelationChange()
        {
            #region Saving
            _Book _book = new _Book() { BookName = "1.Kitap" };
            _Book _book2 = new _Book() { BookName = "2.Kitap" };
            _Book _book3= new _Book() { BookName = "3.Kitap" };

            _Author author = new _Author() { AuthorName = "1.Yazar" };
            _Author author2 = new _Author() { AuthorName = "2.Yazar" };
            _Author author3 = new _Author() { AuthorName = "3.Yazar" };

            _book._Authors.Add(author);
            _book._Authors.Add(author2);

            _book2._Authors.Add(author);
            _book2._Authors.Add(author2);
            _book2._Authors.Add(author3);

            _book3._Authors.Add(author3);

            await db.AddAsync(_book);
            await db.AddAsync(_book2);
            await db.AddAsync(_book3);
            await db.SaveChangesAsync();
            #endregion
            return Ok();

            #region Alıstırma 1
            //1.kitap ile 3.yazarın iliskisi yok. Bir iliski kurmaya calısalım.
            _Book book = await db.Books.FindAsync(1);
            _Author author1 = await db.Authors.FindAsync(3);
            book._Authors.Add(author1);//elde edilen book a elde edilen author eklendi.
            await db.SaveChangesAsync();//bu sekilde ilgili veriler db ye aktarıldı.
                                        //Dikkat edilirse db ye bir add islemi yapılmadı. Cunku burada bir sey eklemiyoruz. Bir iliski guncelliyoruz.
            #endregion


            #region Alıstırma 2
            //3 id li yazar bircok kitap ile iliskili. 3 id li yazarın 1 id li kitaplar dısında butun kitaplarla iliskisini koparmaya calısalım.
            _Author author4 = await db.Authors
                //Probleme yazardan yola cıktık bu yuzden cozume yazardan baslıyorum. Cunku yazardaki iliskileri koparmak istiyorum.
                .Include(a => a._Books)//kitaplar ile iliskide oynama yapacagım icin aynı zamanda kitapları elde etmem gerekiyor.
                .FirstOrDefaultAsync(a => a.ID == 3);
            //3 id li yazarı ve bu yazara ait butun kitapları elde ettik. Simdi manuel ya da donguyle bagları koparalım.
            foreach (var inlineBook in author4._Books)
            {
                if (inlineBook.ID != 1)//eger ilgili kitabın id si 1 degilse 
                    author4._Books.Remove(inlineBook);//ilgili yazardan o kitabı dolayısıyla iliskiyi siliyorum.
            }
            await db.SaveChangesAsync();//bu sekilde ilgili veriler db ye aktarıldı.
            /*Bu sekilde 3 id li yazarın 1 id li kitap dısında butun iliskileri koparılmıs oldu.
            Burada kullanılan yapı n-n oldugu icin bu iliski cross table dan silinir. Cunku n-n iliskide verilerin tutuldugu tablolar 
            principal table dir. Cross table ise dependent table olur.*/
            #endregion

            #region Alıstırma 3
            /*2 id li kitabı elde edelim ve 1 numaralı yazar ile iliskisini koparalım. 3 id li yazar ile iliskisini saglayalım. Ardından 
            4.yazar idb ye ekleyelim ve aynı sekilde 2 id li kitabın bu yazar ile iliskisini saglayalım.*/
            _Book book1 = await db.Books.
                Include(b => b._Authors)
                .FirstOrDefaultAsync(b => b.ID == 2);
            _Author silinecekYazar = book1._Authors.FirstOrDefault(a=>a.ID==1);//kitabın yazarlarından 1 id li olan elde edildi
            book1._Authors.Remove(silinecekYazar);//cross table dan ilgili ilsiki koparıldı.
            /*Burada dkkat edersek db den bir kayıt silinmedi. Sadece cross table dan 2 id li kitap ile 1 id li yazarın iliskisi silindi.
            Yani cross table dan bu kolonlar silindi lakin kitap vs yazar kayıtlarında bir degisiklik yapılmamıstır.*/

            _Author author5 = await db.Authors.FindAsync(3);//3 id li yazar elde edildi.
            book1._Authors.Add(author5);/*2 id li kitap ile  3 id li yazar iliskilendirildi. Yani aslında cross
            table a bu sekilde bir kayıt eklenmis oldu.*/
            book1._Authors.Add(new _Author { AuthorName="4. Yazar" });//Burada ise yeni bir yazar kaydı eklendi ve iliski yapıldı.
            //Burada oncelikle yeni yazarın  db ye kaydı insert islemi ile yapılır. Sonradan iliski kurulur.

            await db.SaveChangesAsync();//Butun bu degisiklikler db ye yansıtıldı.

            /*Bu sekilde bir kitabı elde edip bir yazar ile komple iliskisini kesip ardından baska bir yazar ve yeni eklenmis bir yazar ile 
            iliskisini saglayan kompleks islemler de yapabiliriz.*/

            #endregion
        }
        #endregion
    }

    class _Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public _Address Address { get; set; }
    }
    class _Address
    {
        public int ID { get; set; }
        public string  PersonAddress { get; set; }
        public _Person Person { get; set; }
    }
    class _Blog
    {
        public _Blog()
        {
            Posts = new HashSet<_Post>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<_Post> Posts { get; set; }
    }
    class _Post
    {
        public int ID { get; set; }
        public string Title{ get; set; }
        public  _Blog Blog{ get; set; }
    }
    class _Book
    {
        public int ID { get; set; }
        public string BookName { get; set; }
        public ICollection<_Author>_Authors  { get; set; }
    }
    class _Author
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public ICollection<_Book>  _Books { get; set; }
    }
    class AppDBContext:DbContext
    {
        public DbSet<_Person> Persons { get; set; }
        public DbSet<_Address> Addresses{ get; set; }
        public DbSet<_Blog> Blogs{ get; set; }
        public DbSet<_Post> Posts { get; set; }
        public DbSet<_Book> Books{ get; set; }
        public DbSet<_Author> Authors{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConntectionString]");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address_>()
                .HasOne(a => a.Person)
                .WithOne(p => p.Address)
                .HasForeignKey<Address_>(a => a.ID);
            modelBuilder.Entity<Post_>()
                .HasOne(p => p.Blog)
                .WithMany(b => b.Posts);
        }
    }
}
