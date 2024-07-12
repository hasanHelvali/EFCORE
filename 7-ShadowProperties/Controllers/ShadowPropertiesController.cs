using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace _8_ShadowProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShadowPropertiesController : ControllerBase
    {
        ApplicationDbContext context = new();
        #region Foreign Key - Shadow Properties
        /*Entity sınıflarında fiziksel olarak tanımlanmayan/modellenmeyen ancak EF Core tarafından ilgili entity için var olan/var olduğu
        kabul edilen property'lerdir.
        Tabloda gösterilmesini istemediğimiz/lüzumlu görmediğimiz/entity instance'ı üzerinde işlem yapmayacağımız kolonlar için
        shadow propertyler kullanılabilir.
        Shadow property'lerin değerleri ve stateleri Change Tracker tarafından kontrol edilir.*/

        /*İlişkisel senaryolarda foreign key property'sini tanımlamadığımız halde EF Core tarafından dependent entity'e
        eklenmektedir. İşte bu shadow property'dir.*/
        #endregion



        #region Foreign Key - Shadow Properties
        public async Task<IActionResult> ShadowProperties()
        {
            /* İlişkisel senaryolarda foreign key property'sini tanımlamadığımız halde EF Core tarafından dependent entity'e 
                eklenmektedir. İşte bu shadow property'dir.*/
            var blogs = await context.Blogs.Include(b => b.Posts)
                .ToListAsync();
            Console.WriteLine();
            return Ok();
        }
        #endregion

        #region Shadow Property Olusturma 
        public async Task<IActionResult> ShadowPropertyCreate()
        {
            //Bir entity üzerinde shadow property oluşturmak istiyorsanız eğer Fluent API'ı kullanmanız gerekmektedir.
            //modelBuilder.Entity<Blog>()
            //            .Property<DateTime>("CreatedDate");
            //seklinde bir konfigurasyonla bir shadow property olusturmus olduk.x
            return Ok();
        }
        #endregion

        #region Shadow Property'e Erişim Sağlama
        #region ChangeTracker İle Erişim
        public async  Task<IActionResult> ShadowProperty_ChangeTrackerErisimi()
        {
            //Shadow property'e erişim sağlayabilmek için Change Tracker'dan istifade edilebilir.
            var blog = await context.Blogs.FirstAsync();

            var createDate = context.Entry(blog).Property("CreatedDate");//olutururken de erisim gosterirken de entry kullanıyoruz.
            Console.WriteLine(createDate.CurrentValue);//memory deki onbellekte degistirilmis veya degistirilmemis deger
            Console.WriteLine(createDate.OriginalValue);//db deki deger

            createDate.CurrentValue = DateTime.Now;//Bu sekilde ilgilil degere mudahale de edebiliriz.
            await context.SaveChangesAsync();//ilgili shadow property nin degerini db ye de aktarabiliriz.
            return Ok();
        }
        #endregion

        #region EF.Property İle Erişim
        public async Task<IActionResult> ShadowProperty_EFPropertyErisimi()
        {
            //Özellikle LINQ sorgularında Shadow Propery'lerine erişim için EF.Property static yapılanmasını kullanabiliriz.
            var blogs = await context.Blogs.OrderBy(b => EF.Property<DateTime>(b, "CreatedDate")).ToListAsync();
            //EF.Property ile shadow property ye ulasıyoruz. CreatedDate e gore veri sıralama islemini yapmıs olduk.

            var blogs2 = await context.Blogs.Where(b => EF.Property<DateTime>(b, "CreatedDate").Year > 2020).ToListAsync();
            //Bu sekilde daha detaylı sorgulamalar yapabiliyoruz.
            Console.WriteLine();
            return Ok();
        }
        #endregion

        #endregion
    }

    class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool lastUpdated { get; set; }

        public Blog Blog { get; set; }
    }
    class ApplicationDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDb;User ID=SA;Password=1q2w3e4r+!");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property<DateTime>("CreatedDate");//Bu sekilde shadow bvir property olusturmus olduk.
            //Shadow property olusturmak icin Property fonskiyonunun generic olanını kullanırız. DateTime turunde adı da
            //CreatedDate olan bir shadow property olusturmus olduk.

            base.OnModelCreating(modelBuilder);
        }
    }

}

/*LastUpdated gibi property ler gelistiricinin ilgilenecegi degil efcore un ilgilenecegi verilerdir. Bu verilerden entity yi 
 arındırmak icin Shadow property leri kullanabiliriz.*/