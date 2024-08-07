﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

Console.WriteLine();

#region OnModelCreating
//Genel anlamda veritabanı ile ilgili konfigürasyonel operasyonların dışında entityler üzeirnde konfigürasyonel
//çalışmalar yapmamızı sağlayan bir fonskiyodundur.
#endregion

#region IEntityTypeConfiguration<T> Arayüzü
//Entity bazlı yapılacak olan konfigürasyonları o entitye özel harici bir dosya üzerinde yapmamızı sağlayan bir arayüzdür.

//Harici bir dosyada konfigürasyonların yürütülmesi merkezi bir yapılandırma noktası oluşturmamıızı sağlamaktadır.
//Bu Solid Prensiplerinden Single Responsibility icin yapılmıstır.
//Harici bir dosyada konfigüarsyonların yürültülmesi entity sayısının fazla olduğu senaryolarda yönetilebilirliği
//artturacak ve yapılandırma ile ilgili geliştiricinin yükünü azaltacaktır.
#endregion

#region ApplyConfiguration Metodu
//Bu metot harici konfigürasyonel sınıflarımızı EF Core'a bildirebilmek için kullandığımız bir metotdur.
#endregion

#region ApplyConfigurationsFromAssembly Metodu
//Uygulama bazında oluşturulan harici konfigürasyonel sınıfların her birini OnModelCreating metodunda ApplyCOnfiguration
//ile tek tek bildirmek yerine bu sınıfların bulunduğu Assembly'i bildirerek IEntityTypeConfiguration
//arayüzünden türeyen tüm sınıfları ilgili entitye karşılık konfigürasyonel değer olarak baz almasını tek kalemde yapmamızı
//sağlayan bir metottur.
#endregion


class Order
{
    public int OrderId { get; set; }
    public string Description { get; set; }
    public DateTime OrderDate { get; set; }
}

class OrderConfiguration : IEntityTypeConfiguration<Order>
    //Bu sekilde ilgili interface i implemente ederek ilgili configurasyonları uygulayabiliriz.
{
    public void Configure(EntityTypeBuilder<Order> builder)//zorunlu implemente ettirilen fonskiyon
    {
        builder.HasKey(x => x.OrderId);
        builder.Property(p => p.Description)
            .HasMaxLength(13);
        builder.Property(p => p.OrderDate)
            .HasDefaultValueSql("GETDATE()");
        //Bu sekilde ilgili konfigurasyonlar order a ayarlandı.
    }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        //Bu sekilde order a ait konfigurasyonları dahil etmiş olduk.

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        //Assembly.GetExecutingAssembly() ile o anda calısılan assembly icerisinde araman gereken konfigurasyonu ara demis olduk.
        //1 tane degilde 100 tan enetity miz olabilirdi. Bunların her birini 
        //modelBuilder.ApplyConfiguration(new OrderConfiguration());
        //seklinde dahil etmek yerine burada yaptıgımız gibi tek kalemde tek kodda hepsini dahil edebiliriz.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!");
    }
}