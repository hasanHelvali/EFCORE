using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;

namespace _6_BackingFields.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackingFieldsController : ControllerBase
    {
        BackingFieldDbContext context = new();

        #region Backing Fields
        public async Task<IActionResult> BackingFields()
        {
            /*Tablo içerisindeki kolonları, entity class'ları içerisinde property'ler ile değil field'larla temsil etmemizi sağlayan bir özelliktir.
            Bu bizim ne isimie yarar?
            Veri tabanından gelecek ve veri tabanına gidecek verileri kapsulleyerek dısarıya acabiliriz.*/

            var person = await context.Persons.FindAsync(1);//Bu sekilde ilgili operasyona uygun verileri elde ediyoruz.

            //veri set lemeye bakalım.
            BFPerson person2 = new()
            {
                Name = "Person 101",
                Department = "Department 101"
            };
            await context.Persons.AddAsync(person2);
            await context.SaveChangesAsync();
            /*Seklinde ilgili manipulasyona gore verileri set lemis oluyoruz. Veriler modelde kapsulleme operasyonundaki gibi setlenir
            yani ilk 3 harfi kayıt edilir.*/

            return Ok();
        }
        #endregion

        #region BackingField Attributes
        public async Task<IActionResult> BackingFieldsAttributes()
        {
            var person = await context.Persons2.FindAsync(1);
            //Bu sekildeki sorguda da ilgili filed in ilgili property ile aynı degerde geldigini goruruz.

            return Ok();
        }
        #endregion

        #region BackingFieldFluent
        public async Task<IActionResult> BackingFieldsFluent()
        {
            //FluentApi de ilgili duzenleme yapılmıs ve name field i Name property sine baglanmıstır.
            var person = await context.Persons3.FindAsync(1);//Bu sekilde ilgili veriler name field i ile de elde edilir.
            return Ok();
        }
        #endregion

        #region Field And Property Access
        /*EF Core sorgulama sürecinde entity içerisindeki propertyleri ya da field'ları kullanıp kullanmayacağının davranışını
        bizlere belirtmektedir.EF Core, hiçbir ayarlama yoksa varsayılan olarak propertyler üzerinden verileri işler, eğer ki backing 
        field bildiriliyorsa field üzerinden işler yok eğer backing field bildirildiği halde davranış belirtiliyorsa ne belirtilmişse ona 
        göre işlemeyi devam ettirir.*/

        //UsePropertyAccessMode üzerinden davranış modellemesi gerçekleştirilebilir.
        //Fluent api da ilgili konfigurasyona bakılabilir.
        #endregion

        #region Field-Only Properties
        /*Entitylerde değerleri almak için property'ler yerine metotların kullanıldığı
        veya belirli alanların hiç gösterilmemesi gerektiği durumlarda(örneğin primary key kolonu) kullanabilir.
        */
    }

    #region BackingFields
    class BFPerson
    {
        public int Id { get; set; }
        public string name;
        public string Name { get => name.Substring(0, 3); set => name = value.Substring(0, 3); }
        //Bu sekilde gelen veri uzerinde bir manipulasyon yapıp verinin istedigim kadarını dısarıya actım.
        public string Department { get; set; }
    }
    //seklinde name field ini Name property sine eşdeger bir yapı olarak elde etmis olduk.
    #endregion
    #region BackingFieldsAttributes
    class BFAPerson
    {
        public int Id { get; set; }
        public string name;

        [BackingField(nameof(name))]
        public string Name { get; set; }
        //BackingFields tanımlamanın ikinci yolu ise attribute tur. Buradaki property yi ilgili field a baglamıs olduk.
        public string Department { get; set; }
    }
    #endregion
    #region BackingFieldsFluent
    class BFPersonFluent
    {
        public int Id { get; set; }
        public string name;
        public string Name { get; set; }
        public string Department { get; set; }
    }
    #endregion
    #region Fields Only Properties
    class PersonFieldOnly
    {
        public int Id { get; set; }
        //public string Name { get; set; } Bu property iptalm edildi. FluentAPI da name field inin Name property si gibi davranması saglandı.
        public string name;

        public string Department { get; set; }

        //Artık name field ina veri ekleme ve veri set leme islemini yapabiliriz.
        public string GetName()
            => name;
        public string SetName(string value)
            => this.name = value;
    }
    #endregion
    class BackingFieldDbContext : DbContext
    {
        public DbSet<BFPerson> Persons { get; set; }
        public DbSet<BFAPerson> Persons2 { get; set; }
        public DbSet<BFPersonFluent> Persons3 { get; set; }
        public DbSet<PersonFieldOnly> Persons4 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=BaskingFieldDb;User ID=SA;Password=1q2w3e4r+!");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)//Fluent api kullanımına burada baslıyoruzç
        {


            //backingFields fluent
            modelBuilder.Entity<BFPersonFluent>()
                .Property(p => p.Name)
                .HasField(nameof(BFPersonFluent.name));
            //Fleunt API'da HasField metodu BackingField özelliğine karşılık gelmektedir.


            //backingFields UsePropertyAccessMode ile davranıs belirleme
            modelBuilder.Entity<BFPersonFluent>()
                .Property(p => p.Name)
                .HasField(nameof(BFPersonFluent.name))
                .UsePropertyAccessMode(PropertyAccessMode.PreferProperty);//Property yi kullan demis olduk.

            //PropertyAccessMode enum ının diger degerleri icin acıklamalar asagıdadır.
            //Field : Veri erişim süreçlerinde sadece field'ların kullanılmasını söyler. Eğer field'ın kullanılamayacağı durum söz konusu olursa bir exception fırlatır.
            //FieldDuringConstruction : Veri erişim süreçlerinde ilgili entityden bir nesne oluşturulma sürecinde field'ların kullanılmasını söyler.,
            //Property : Veri erişim sürecinde sadece propertynin kullanılmasını söyler. Eğer property'nin kullanılamayacağı durum söz konusuysa (read-only, write-only) bir exception fırlatır.
            //PreferField:Field i kullanır.
            //PreferFieldDuringConstruction:Nesne oluşturma surecinde field i kullanır.
            //PreferProperty:Property yi kullanır.

            //Field Only
            modelBuilder.Entity<PersonFieldOnly>()
                .Property(nameof(PersonFieldOnly.name));//Name field ina property muamelesinin yapılmasını soyledım.

        }
    }
}
