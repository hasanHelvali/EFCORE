using System;
using System.Collections.Generic;

namespace _5_RelationalStructures.Entities;

public partial class Urunler
{
    public int Id { get; set; }

    public string UrunAdi { get; set; } = null!;

    public string Fiyat { get; set; } = null!;

    public virtual ICollection<Parçalar> Parçalars { get; set; } = new List<Parçalar>();
}
