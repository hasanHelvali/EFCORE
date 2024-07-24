using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
ApplicationDbContext context = new();
#region Explicit Loading

//Oluşturulan iliskisel sorguya eklenecek verilerin şartlara bağlı bir şekilde/ihtiyaçlara istinaden yüklenmesini sağlayan bir yaklaşımdır.

//id si 2 olan employee un Gencay adında bir order ini elde etmeye calısalım.
//var employee = await context.Employees.Include(e=>e.Orders).FirstOrDefaultAsync(e => e.Id == 2);
//seklinde elde edebiliriz. Butun order ları getirmis olduk.

//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);//bu sekilde employee u elde edip 
//if (employee.Name == "Gençay")
//{
//    var orders = await context.Orders.Where(o => o.EmployeeId == employee.Id).ToListAsync();//varsa bu sekilde order i da sorgulayabiliriz.
//}
//Lakin goruldugu uzere cok maliyetli bir kod yazıyoruz.Bunu onlemenin yolu explicit loading dir. Reference ve Collection seklinde 2 fonksiyonla calısırız.

#region Reference

//Explicit Loading sürecinde ilişkisel olarak sorguya eklenmek istenen tablonun navigation propertysi eğer ki tekil bir türse bu
//tabloyu reference ile sorguya ekleyebilemkteyiz.

//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
////...
////...
////...
//await context.Entry(employee).Reference(e => e.Region).LoadAsync();
//employee verisine region la ilgili iliskisel verileri eklemis olduk. 

//Console.WriteLine();
#endregion
#region Collection

//Explicit Loading sürecinde ilişkisel olarak sorguya eklenmek istenen tablonun navigation propertysi eğer ki çoğul/koleksiyonel bir türse
//bu tabloyu Collection ile sorguya ekleyebilemkteyiz.

//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
//...
//...
//...
//await context.Entry(employee).Collection(e => e.Orders).LoadAsync();
//Burada da aynı islem yapılır. Ustteki employee sorgusundan elde edilip bellege yuklenen veriye ek olarak, buradaki orders sorgusu
//calıstırılır. Bellege yuklenip employee ile iliskilendirme yapılır. 

//Console.WriteLine();
#endregion

#region Collection'lar da Aggregate Operatör Uygulamak
//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
//...
//...
//...
//var count = await context.Entry(employee).Collection(e => e.Orders).Query().CountAsync();

//Burada collection ı load edip daha sonra aggregate operasyonlarını yapma zorunlulugumuz yok.
//.Query() ile IQuearable hale getirdik. Daha sonra ıste aggregate operasyonlarına devam edebiliriz. 
Console.WriteLine();
#endregion
#region Collection'lar da Filtreleme Gerçekleştirmek
//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
////...
////...
////...
//var orders = await context.Entry(employee).Collection(e => e.Orders).Query().Where(q => q.OrderDate.Day == DateTime.Now.Day).ToListAsync();
//Bu sekilde filtreleme islemleri de yapabiliriz.
#endregion
#endregion

public class Employee
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }

    public List<Order> Orders { get; set; }
    public Region Region { get; set; }
}
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }

    public Employee Employee { get; set; }
}


class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True");
    }
}