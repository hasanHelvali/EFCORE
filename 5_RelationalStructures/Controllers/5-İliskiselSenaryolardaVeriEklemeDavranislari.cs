using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _5_RelationalStructures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _5_İliskiselSenaryolardaVeriEklemeDavranislari : ControllerBase
    {
        ApplicationDBContext applicationDBContext = new();
        ApplicationDBContext2 applicationDBContext2 = new();
        ApplicationDBContext3 applicationDBContext3 = new();
        ApplicationDBContext4 applicationDBContext4 = new();

        #region OneToOne İliskisel Senaryolarda Veri Ekleme
        #region 1.Yontem-Principal Entity Uzerinden Veri Ekleme
        /*her personelin bir adresi olmaz. Ama bir adres varsa bu illaki bir personele ait olmalıdır. Yani Personel burada
        principal entity dir. Eger senaryodan bunu anlamıyorsak fluent api da tanımlanan foreign key dependent entity ye aittir. Bu sekilde de 
        anlasılabilir.*/
        public async Task<IActionResult> Yontem1()
        {
            Person person =new Person();
            person.Name = "Hasan";
            person.Address = new Address { PersonAddress="Gencosman Mahallesi Bayburt"};
            /*address property si bir entity ye karsılık gelir. 
            Burada efcore person ile ilgili alanların person a address ile iliskili alanların address e
            eklenecegini verir ve ilgili iliskiyi kurar. Bu sekilde iliskisel bir senaryona veri ekleme yapabiliriz.
            */
            await applicationDBContext.AddAsync(person);
            await applicationDBContext.SaveChangesAsync();
            /*Bu islemler soncuunda db de bir person ve buna karsılık ilsikide oldugu bir address nesnesi olsuturulur.
             Person eklenir. Bu Person ın id si yakalanır ve bu person a karsılık olarak iliskisel bir sekilde address eklenir.
            Buradaki entity principal entity oldugu icin olusturulurken kendi basına olusturulabilir. Person olustururken Address 
            olusturulma zorunlulugu yoktur.*/
            return Ok();
        }
        #endregion
        
        #region 2.Yontem- Dependent Entity Uzerinden Veri Ekleme
        public async Task<IActionResult> Yontem2()
        {
             Address address= new Address
             {
                 PersonAddress = "Gencosman Mahallesi Bayburt",
                 Person = new Person() { Name="Hasan Helvali"}
             };
            await applicationDBContext.AddAsync(address);
            await applicationDBContext.SaveChangesAsync();
            return Ok();
            /*Burada Yontem1 deki islemlerin aynısı tersten uygulandı. Aynı islemler burada da yapılır. Ancak burada bir nüans farkı vardır. 
             Depedent entity uzerinden bir veri eklerken principal entity de veriyle beraber eklenmek zorundadir. Cunku burada foreign key vardır.
            Dependent entity uzerinden yapılan eklemelerde principal entity zoraki verilmek zorundadir.*/
        }

        #endregion
        #endregion

        #region OneToMany İliskisel Senaryolarda Veri Ekleme
        #region 1.Yontem-Principal Entity Uzerinden Dependent Entity Verisi Ekleme
        #region Nesne Referansı Uzerinden Ekleme
        public async Task<IActionResult> Yontem1NesneReferansı()
        {
            Blog blog = new Blog() { Name = "hhs.com" };
            //blog.Posts = new HashSet<Post>();//ctor da baslatıldı o yuzden burada gerek yok.
            blog.Posts.Add(new Post { Title = "Post1" });
            blog.Posts.Add(new Post { Title = "Post2" });
            blog.Posts.Add(new Post { Title = "Post3" });
            //Bir blog referansının uzerinden 1-n olarak verileri ekledik.
            await applicationDBContext2.AddAsync(blog);
            await applicationDBContext2.SaveChangesAsync();
            return Ok();
        }
        #endregion
        #region Object Initializer Uzerinden Ekleme
        public async Task<IActionResult> Yontem1ObjectInitializer()
        {
            Blog Blog2 = new Blog
            {
                Name = "A Blog",
                Posts = new List<Post>()
                {
                    new Post{Title="Post4"},
                    new Post{Title="Post5"},
                    new Post{Title="Post6"},
                }
            };
            /*Bu kullanım ise direkt olarak uretilen tekil obje uzerinden yapılan 1-n veri iliskilendirme yapısıdır. Bu yapıda
            ctor dan post koleksiyonunu baslatmaya da gerek yoktur.*/

            return Ok();
        }
        #endregion
        #endregion

        #region 2.Yontem-Dependent Entity Uzerinden Principal Entity Verisi Ekleme
        //Bu yontem genellikle kullanılmaz cunku 1-n davranısına aykırı bir modeldir.
        public async Task<IActionResult> OneToManyYontem2a()
        {
            //Post Post = new()
            //{
            //    Title = "Post7"
            //};
            //Bu post mutlaka bir blog nesnesi ile iliskide olmalı. Direkt olarak db ye eklenemez. Cunku bu bir dependent entity dir.

            Post Post = new()
            {
                Title = "Post7",
                Blog=new() { Name="D Blog"}//Bu sekilde principal entity de dolduruldu.
            };
            await applicationDBContext2.AddAsync(Post); 
            await applicationDBContext2.SaveChangesAsync();
            /*Bu islem sonucunda efcore öncelikle blog nesnesini db de olusturur ve sonra bu nesne ile Post u iliskilendirir.
            Bu sekilde , dogru bir sekilde insert islemini yapar.
            Lakin burada bir sorun vardır. Bu yontemle her principal entity ye karsılık olarak sadece 1 tane dependent entity eklenebilir.
            Aslında 1-1 bir iliski durumu gibi bir durum ortaya cıkar. Evet pratikte 1-n iliski kurulur. Veriler de eklenir. Lakin eklenen 
            veriler 1 er tane olurlar. Bu yuzden bu yontemden olabildigince kacınırız.
            Bir diger dezavantaj olarak principal entity den basladıgımız icin nesne icerisine mutlaka bir dependent entity vermemiz gerekir. Bunu 
            da unutmyalım.*/


            return Ok();
        }

        #endregion

        #region 3.Yontem-Foreign Key Kolonu Uzerinden Veri Ekleme
        public async Task<IActionResult> OneToManyYotem3()
        {
            /*Post nesnesinde goruldugu uzere bir BlogID seklinde bir foreign key tanımlanmıstır. Bu foreign key tanımı suan yapıldıgı gibi
             default convention ile yapılabilir, veya data annotations lar ile yapılabilir veya fluent api ile bildirilebilir.
            Hasılı bir foreign key varsa bu foreign key ile ilgili kayıtlar Blog nesnesine ihtiyac olmadan yapılabilir.*/
            Post post = new Post
            {
                BlogID = 1,
                Title = "Post 8"
            };
            /*Az once dependent entity tanımlandıgında bunun mutlaka bir principal entity nin parcası olması gerektigini soylemistik.
             Lakin burada bir principal entity vermedik. Aslında verdik. Ilgili foreign key kolonu db de karsılıgı olan bir principal 
            entity yi temsil etmelidir. Bu sekilde yeni bir blog olusturulmadan varolan bir blog nesnesine foreign key ile yeni post nesneleri 
            eklenebilir. Buradaki foreign key db den mi cekilir yoksa kullanıcıdan mı alınır orası bize kalmıs. Bu sekilde de islemler 
            yapılabilir. Hasılı 1 ve 2.yontemler var olmayan bir veri ile bir principal entity ile veri ekleme yontemi iken bu yontem 
            db de kayıtlı olan bir principal entity uzerinden veri ekleme islemidir.*/
            await applicationDBContext2.AddAsync(post);
            await applicationDBContext2.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #endregion

        #region ManyToMany İliskisel Senaryolarda Veri Ekleme
        #region Yontem1- Default Convention ile Yapılmıs n-n iliskide veri ekleme
        public async Task<IActionResult> Yontem1ManyToMany()
        {
            Book Book = new Book
            {
                BookName = "A Kitabı",
                Authors = new HashSet<Author>()
                {
                    new Author { AuthorName = "A kisisi" },
                    new Author { AuthorName = "B kisisi" }
                }
            };
            await applicationDBContext3.AddAsync(Book);
            await applicationDBContext3.SaveChangesAsync();
            /*Bu sekilde bir veri ekleme yapılabilir. Author dan veya Book dan baslamanın bir farkı yoktur. Cunku iki entity de principal 
             entity gibi davranır.*/
            return Ok();
        }
        
        #endregion
       
        #region Yontem2- Fluent api ile Yapılmıs n-n iliskide veri ekleme
        public async Task<IActionResult> Yontem2ManyToMany()
        {
            /*Eger ki fluent api kullanıyorsak ve cross table a erisebiliyorsak yani cross table kodlanmıs ise bu cross table uzerinden 
             iliskisel veriler eklenebilir. 
            Cross table daki foreign key ler ile varolan veriler uzerinden , navigation property ler ile yeni olusturulacak veriler 
            uzerinden iliskisel ekleme vs yapabiliyoruz.*/

            Author2 author = new Author2
            {
                AuthorName = "C Kisisi",
                Books = new HashSet<BookAuthor>()
                {
                    new BookAuthor
                    {
                        BookID=1,
                    },
                    new BookAuthor
                    {
                        Book=new Book2{BookName="F Kitabı"}
                    }
                }
            };
            /*Bu calısma da C kisisi ile varolan 1 id li kitap iliskilendirildi. Ayrıca F kitabı seklinde yeni bir veri girisi yapıldı ve 
              onunla da iliskilendirildi.Bu sekilde cross table uzerinden compleks veya yalın calısmalar yapılabilir. Buradaki en onemli 
             sart Cross table ın olusturulmasıdır. Cross table default convention da da bazen olusturulur lakin fluent api de kesin 
            olusturulması gerekir. Bu yuzden cross table ların kullanıldıgı durumlar genellikle fluent api in kullanıldıgı durumlardır.*/
            return Ok();
        }
        #endregion
        #endregion
    }


    #region OneToOne model And Configuragion
    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }
    class Address
    {
        public int ID { get; set; }
        public string PersonAddress { get; set; }
        public Person Person { get; set; }
    }

    class ApplicationDBContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConnectionString]");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Person)
                .WithOne(a => a.Address)
                .HasForeignKey<Address>(x => x.ID);
        }
    }
    #endregion

    #region OneToMany model And Configuragion
    //Burada fluent api ökullanılmadı. Direkt olarak default convention uzerinden iliskilendirme yapılmıstır.
    class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
            /*EFCore valısmalarında Blog nesnesi uzerinden Post a karsılık veriler eklemek istedigimiz zaman Post un null olmaması gerekir.
             Bu yuzden contructor da bu yapıyı new leyerek baslatıyoruz. Yontem1 deki Nesne uzerinden veri eklemeye bakılırsa daha iyi 
            anlasılabilir. Bu genellikle db first kullanılırken ihtiyac duyulan bir yapıdır.
            Hata almamak icin entity ler icindeki koleksiyonel nav property ler ctor da baslatılır.
            Bu yapı Yontem1 Nesne referansı ile ekleme de gorulduug uzere koddan da baslatılabilir.
            HashSet yazılmasının ozel bir sebebi yoktur. List vs de yazılabilirdi. Lakin HashSet hem unique hrm de daha performanslı bir yapıdır.
            Bu da ek bir bilgi olsun.*/
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
    class Post
    {
        public int ID { get; set; }
        public int BlogID { get; set; }
        public string Title{ get; set; }
        public Blog Blog { get; set; }
    }
    class ApplicationDBContext2 : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConnectionString]");
        }
    }
    #endregion

    #region ManyToMany model And Configuragion
    
    #region DefultConvention
    class Book
    {
        public Book()
        {
            Authors = new HashSet<Author>();
        }
        public int ID { get; set; }
        public string BookName { get; set; }
        public ICollection<Author> Authors { get; set; }
    }
    class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public ICollection<Book> Books { get; set; }
    }
    class ApplicationDBContext3 : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConnectionString]");
        }
    }

    #endregion

    #region Fluent api
    class Book2
    {
        public Book2()
        {
            Authors = new HashSet<BookAuthor>();
        }
        public int ID { get; set; }
        public string BookName { get; set; }
        public ICollection<BookAuthor> Authors { get; set; }
    }
    class Author2
    {
        public Author2()
        {
            Books = new HashSet<BookAuthor>();
        }
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public ICollection<BookAuthor> Books { get; set; }
    }
    class BookAuthor
    {
        public int BookID { get; set; }
        public int AuthorID { get; set; }
        public Book2 Book { get; set; }
        public Author2 Author{ get; set; }
    }
    class ApplicationDBContext4 : DbContext
    {
        public DbSet<Book2> Books { get; set; }
        public DbSet<Author2> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("[ConnectionString]");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.AuthorID, ba.BookID });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(a => a.Authors)
                .HasForeignKey(ba => ba.BookID);

            modelBuilder.Entity<BookAuthor>()
               .HasOne(ba => ba.Author)
               .WithMany(a => a.Books)
               .HasForeignKey(ba => ba.AuthorID);

        }
    }
    #endregion

    #endregion
}
