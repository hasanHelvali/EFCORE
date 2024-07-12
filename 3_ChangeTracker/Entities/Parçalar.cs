using System;
using System.Collections.Generic;

namespace _3_ChangeTracker.Entities;

public partial class Parçalar
{
    public int Id { get; set; }

    public string ParcaAdi { get; set; } = null!;

    public int? UrunId { get; set; }

    public virtual Urunler? Urun { get; set; }
}
