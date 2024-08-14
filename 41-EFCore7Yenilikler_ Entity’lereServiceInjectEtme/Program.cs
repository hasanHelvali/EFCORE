
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

ApplicationDbContext context = new();

#region Entity'de Service Inject Etme
var persons = await context.Persons.ToListAsync();
foreach (var person in persons)
{
    person.ToString();
}
#endregion

public class PersonServiceInjectionInterceptor : IMaterializationInterceptor //Bu sekilde bir arayüz var. Bu arayüz inject i saglar.
{//Burasi bir interceptor gorevi gorur.
    public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)//Bu metot implement ettirilir.
    {
        if (instance is IHasPersonService hasPersonService)
        {
            hasPersonService.PersonService = new PersonLogService();
        }
        //Burada inject islemi yapılır.
        return instance;
    }
}
public interface IHasPersonService
{
    IPersonLogService PersonService { get; set; }
}
public interface IPersonLogService
{
    void LogPerson(string name);
}
public class PersonLogService : IPersonLogService
{//Bu sekilde custom bir abstraction a yani bir arayuze sahip custom bir servisimiz oldugunu dusunelim.
    public void LogPerson(string name)
    {
        Console.WriteLine($"{name} isimli kişi loglanmıştır.");
    }
}
public class Person : IHasPersonService//Person Entity si IHasPersonService arayüzü ile PersonLogService i implemente etmektedir.
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        PersonService?.LogPerson(Name);
        return base.ToString();
    }

    [NotMapped]
    public IPersonLogService? PersonService { get; set; }
    //implementasyon neticesinde bu sekilde ilgili arayuzu bir property olarak temsil ediyoruz. IPersonLogService enjeksiyonu
    //yapıldıgında buradaki property kullanılabilir olur. Burada araya bir interceptor koyarak buradaki property ye karsılık
    //bir instance gonderilir. Yani buradkai inject islemi interceptor aracılıgıyla yapılır.
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");

        optionsBuilder.AddInterceptors(new PersonServiceInjectionInterceptor());
    }
}