using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _7_IliskiselSenaryolardaVeriSilmeVeCascadeDeleteDavranislari : ControllerBase
    {
        AppDbContext context = new AppDbContext();

        #region OneToOneSenaryolardaVeriSilme
        public async Task<IActionResult> OneToOneDeleteRelation()
        {
            /*Dependent table da principal table a baglı olan herhangi bir veriyi silelim.
            Person tablosunda 1 id sine sahip olan personel in address ini silmek istiyorum.
            İliskisel senaryolarda bu islemi nasıl yapacagım onemli. Aksi halde addresses tablosundan 1 id sine 
            sahip olan degeri silmek cok kolay bir islemdir. Bir sorgu olsuturmam lazım. Bu sorguya person dan yola cıkmam, 
            sonra bu sorguya address leri eklemem lazım ve sonunda hedef person ı adresleriyle beraber elde etmem gerek.*/
            Person_? person_ = await context.Persons
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.ID == 1);//1 id li person a ulastım.
            context.Addresses.Remove(person_.Address);//address ini address tablosundan sildim.
            await context.SaveChangesAsync();
            //1-1 iliski yapılanmasında bu sekilde dependent table dan veriyi silmis olduk. Person a karsılık address verisini ilsikisel olarak sildik.
            return Ok();
        }
        #endregion
        #region OneToManySenaryolardaVeriSilme
        public async Task<IActionResult> OneToManyDeleteRelation()
        {
            //Hemen hemen 1-1 ile aynı mantıktır.
            Blog_? blog_ = await context.Blogs
               .Include(p => p.Posts)
               .FirstOrDefaultAsync(p => p.ID == 1);//1 id li blog a ve bu blog ile iliskili post lara ulastım. postlar bir Icollection olarak geliyor.
            Post_ post_=blog_.Posts.FirstOrDefault(p=>p.ID == 2);//blog un postlarından 2 id li post elde edildi.
            context.Posts.Remove(post_);//ilgili post u db den sildim.
            await context.SaveChangesAsync();
            return Ok();

            //Bu sekilde 1-n bir iliskide blog a karsılık n tane post tan birini elde edip sildim.
        }
        #endregion
        #region ManyToManySenaryolardaVeriSilme
        public async Task<IActionResult> ManyToManyDeleteRelation()
        {
            /* n-n ilsikide bir kitaba karsılık bir yazarı sil gibi bir islem yapmayız. Cunku silinen yazara baglı baska kitaplar da olabilir.
             Bunun yerine cross table da eslestirilmis olan yazarlar silinebilirler. 1 kitabına karsılık 2 yazarını sil deyince 1 kitabının 
            yazarını sil demis olmayız. Cross table daki 1 kitabının 2 kitabı ile iliskisini silmis oluruz. Yani kayıtların iliskileri ile 
            ilgili degisiklikler cross table dan guncellenir.*/
            Book_? book_ = await context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.ID == 1);//1 id li book a karsılık butun yazarlar elde edildi.
            //Bu kitap icin 2id li yazar ile iliskiyi koparmaya calısalım.
            Author_ author_ = book_.Authors.FirstOrDefault(a => a.ID == 2);//2 id li yazar elde edildi.
            
            //context.Authors.Remove(author_);
            /* Bu sekilde bir silme islemi yaparsak eger direkt olarak yazarı silmis oluruz. 
             Bu yazara baglı diger tablolarda soruna yol acar ve direkt veri kaybına neden olur.
            Hem cross table daki ayzar iliskilerini hem yazarın kendisini silmis oluruz.
            Biz bunu yapmak istemiyoruz. Yapmak istedigimiz kitap ile yazarın iliskisel bagını koparmak yani cross table da islem yapmamız gerekiyor.*/
            book_.Authors.Remove(author_);
            /*Bu sekilde bir islem yaparsak ilgili book un author larından ilgili author kaldıırlır. Buradaki islem cross table tarafında yapılır.
            Bu yontemin aynısı 1-n iliskide de uygulanabilirdi. Lakin orada direkt olarak varlıgı da kaldırabiliyoruz. Bu sayede iliski ile oynayabiliyoruz.
            n-n iliskide ise sadece bu yontem var.*/
            //Yani dogru islem budur. Bu nihayetinde bir update islemidir. Bu sebeple saveChanges cagrılmalıdır.
            await context.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region CascadeDelete
        /*Principal table dan silinen verilerin dependent tabloda varlıgı tutarlı olmaz. Bu sebeple cascade delete yapıları gelistirilmistir.
         Bu davranısların tamamı fluent api ile konfigure edilir.
        
         Ayrıca n-n iliskilerde sime davranısları her zaman cascade uzerine kurulur. Bu sebeple n-n i */
        #region Cascade
        /*Principal tablodan silinen veriye karsılık, karsı tablodan yani bagımlı yani dependent tablodan iliskili verilerin silinmesini saglar. */
        public async Task<IActionResult> CascadeDelete()
        {
            Blog_ blog_ = await context.Blogs.FindAsync(1);//direkt olarak principal entity elde edildi. Dependent entity ile isimiz yok.
            context.Blogs.Remove(blog_);//ilgili blog silindi. Db de buna karsılık butun post lar silinir.
            await context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region SetNull
        /*Principal tablodan silinen veriye karsılık, karsı tablodan yani bagımlı yani dependent tablodan iliskili verilere null deger atanmasını  saglar.
         
        1-1 iliskilerde foreign key ile primary key aynı kolon ile karsılanıyorsa hata alınır cunku set null dependent table daki iliskideki 
        verilerin foreing key ine null ataması yapar ve primary key null olamaz. Bunu cozmek icin 1-1 iliskilerde foreing key ile primary key default 
        convention ın dısına cıkılarak iki ayrı property ile temsil edilmelidir.
        Bunun dısında iliskisel senaryolarda direkt olarak yine kullanılamazlar. Foreing key kolonunun null olabilecegi  isRequiref(false) ile 
        bildirilmelidir ve class lardaki foreing key property leri nullable yapılmalıdır ki null degerler karsılanabilsin.
         */
        public async Task<IActionResult> SetNull()
        {
            Blog_ blog_ = await context.Blogs.FindAsync(1);//direkt olarak principal entity elde edildi. Dependent entity ile isimiz yok.
            context.Blogs.Remove(blog_);//ilgili blog silindi. Db de buna karsılık butun post ların foreing key leri olan BlogID lere null deger atanır.
            await context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Restrict
        /*Principal tablodan silinen veriye karsılık, karsı tabloda yani bagımlı yani dependent tabloda iliskili veriler varsa 
         eger bu verilerin silinmesini kısıtlar, engeller. */

        public async Task<IActionResult> Restrict()
        {
            Blog_ blog_ = await context.Blogs.FindAsync(2);//direkt olarak principal entity elde edildi. Dependent entity ile isimiz yok.
            context.Blogs.Remove(blog_);/* Ilgili blog silinmeye calısılır fakat burada efcore programı patlatır, hata fırlatılır. Cunku ilgili 
            konfigurasyon iliskisel verilerin silinmesi durumunda restrict davranması seklinde ayarlanmıstır. Yani principal entity de olan ve 
            dependent entity de karsılıgı olan bir verinin silinmeye calısılması durumunda program hata fırlatacak sekilde ayarlanmıstır.
            Haliyle ilgili veri silinemez.*/
            await context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #endregion


        #region Saving Data
        /*
        Person_ person = new Person_()
        {
            Name = "Hasan",
            Address = new Address_()
            {
                PersonAddress = "Merkez/Bayburt"
            }
        };
        Person_ person2 = new Person_()
        {
            Name = "Hasan",
        };
        await db.AddAsync(person);
        await db.AddAsync(person2);
        await db.SaveChangesAsync();

        Blog_ blog = new()
        {
            Name = "Hasan",
            Posts = new List<Post_>()
                {
                    new Post_() {Title="1.Post"},
                    new Post_() {Title="1.Post"},
                    new Post_() {Title="1.Post"}
                }
        };
        await db.Blogs.AddAsync(blog);
        await db.SaveChangesAsync();

        Book_ book = new Book_() { BookName = "1.Kitap" };
        Book_ book2 = new Book_() { BookName = "2.Kitap" };
        Book_ book3 = new Book_() { BookName = "3.Kitap" };

        Author_ author = new Author_() { AuthorName = "1.Yazar" };
        Author_ author2 = new Author_() { AuthorName = "2.Yazar" };
        Author_ author3 = new Author_() { AuthorName = "3.Yazar" };

        book.Authors.Add(author);
            book.Authors.Add(author2);

            book2.Authors.Add(author);
            book2.Authors.Add(author2);
            book2.Authors.Add(author3);

            book3.Authors.Add(author3);

            await db.AddAsync(_book);
        await db.AddAsync(_book2);
        await db.AddAsync(_book3);
        await db.SaveChangesAsync();
        */
        #endregion
    }
    class Person_
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Address_? Address { get; set; }
    }
    class Address_
    {
        public int ID { get; set; }
        public string PersonAddress { get; set; }
        public Person_ Person { get; set; }
    }
    class Blog_
    {
        public Blog_()
        {
            Posts = new HashSet<Post_>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Post_> Posts { get; set; }
    }
    class Post_
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int ?BlogID { get; set; }
        public Blog_ Blog { get; set; }
    }
    class Book_
    {
        public int ID { get; set; }
        public string BookName { get; set; }
        public ICollection<Author_> Authors { get; set; }
    }
    class Author_
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public ICollection<Book_> Books { get; set; }
    }

    class AppDbContext : DbContext
    {
        public DbSet<Person_> Persons { get; set; }
        public DbSet<Address_> Addresses { get; set; }
        public DbSet<Blog_> Blogs { get; set; }
        public DbSet<Post_> Posts { get; set; }
        public DbSet<Book_> Books { get; set; }
        public DbSet<Author_> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConntectionString]");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address_>()
                .HasOne(a => a.Person)
                .WithOne(p => p.Address)
                .HasForeignKey<Address_>(a => a.ID)

                //cascade delete baslıyor

                //.OnDelete(DeleteBehavior.Cascade); 
                //persondan veri silinirse karsılıgında address tablosundaki veriyi sil
                /*Şu halde migration basmıs olsak migratrion da bir degisiklik olmaz. Cunku efcore default olarak tabloları olustururken 
                 .OnDelete(DeleteBehavior.Cascade); davranısını konfigurasyon dosyalarına ekler. Migration atıldıgında ilgili migration 
                dosyalarına bakılabilir. Dolayısıyla migrate ettikten sonra bu davranısı eklersek yeni migration da bir degisiklik olmaz.*/

                //.OnDelete(DeleteBehavior.SetNull); 
                /*Bu configurasyona efcore izin vermez. Cunku birebir iliskisel senaryolarda id kolonu hem primary key hem de foreign key 
                 olarak kullanılabilir. Eger boyle bir kullanım varsa bu kolonun null olması hata fırlatır. Foreign key null alabilecek fakat 
                primary ket null alamayacagı icin hata alırız.
                1-1 iliskisel senaryolarda setnull kullanmanın tek yolu foreign key kolonunu ayrı bir propery tarafından temsil etmektir.
                Bunun sonucunda kullanılabilir.*/

                .OnDelete(DeleteBehavior.Restrict);//Ana tablodaki veriler icin bagımlı tabloda iliskisel olan veriler varsa ana tablodaki verinin silinmesini engeller.

            modelBuilder.Entity<Post_>()
                .HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                
                //.OnDelete(DeleteBehavior.Cascade);
                
                //.OnDelete(DeleteBehavior.SetNull)
                //.IsRequired(false)
                /*blog dan veri silinirse karsılıgında post tablosundaki foreign key i null haline getir
                Artık default davranıs degisti. Bu sebeple yeni migraiton larda bu configurasyon gorulur.
                Burada .IsRequired(false) konfigurasyonu post daki foreign key kolonunun illa dolu olmasının gerekmedigini 
                konfigure eder. Boylece post daki blogId adlı kolona yani foreign key e null deger atanabilir. Ayrıca bunu yaparken 
                Post class ındaki BlogID foreign key kolonunu da nullable yapmamız gerekir.*/

                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book_>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);
                /*.OnDelete... n-n senaryolarda OnDelete(...) tanımlı degildir. Tanımlı olmadıgından kullanılınca hata fırlatılır.
                n-n senaryolarda default ve bir tane davranıs vardır bu da cascade veri silme davranısıdır.
                Bunun sebebi n-n tablolarda iki entity de principal yapıdadır ve foreign key ve primary key leri birlestirilerek kullanılır. 
                Iliskiler dependent olan cross table uzerinden belirlenir. Bir entity de bir silme islemi yapıldıgında diger tablodaki iliskisel
                verilerin foreign key lerine null atanırsa, bu sefer null atanan foreing key bir onceki tabloda neye karsılık gelir bu bilinemez
                ve foreign key + primary key kullanımı bozulur. Kısacası tutarlılık saglanamaz. Bu tutarlılıgı saglamak adında bir 
                entity de bir veri silinirse buna karsılık diger tablodaki veriler silinir. Bu islem veriler arasında tutarlılıgı saglamak adına yapılır.
                Ayrı yeten cross table daki guncellemeler principal tablodaki kayıtları etkilemeyecegi icin buradan da bir yere varılamıyor.
                Iste tum bu senaryoda bu sebeple sadece cascade delete islemine izin verilmistir.
                Haricen n-n senaryosunda cross table da programatik olarak manuel bir sekilde modelleniyorsa burada cascade dısındaki 
                davranıslar kullanılabilmektedir.*/

        }

    }
}
