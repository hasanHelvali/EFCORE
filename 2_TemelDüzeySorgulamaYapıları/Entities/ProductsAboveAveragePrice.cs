using System;
using System.Collections.Generic;

namespace _2_TemelDüzeySorgulamaYapıları.Entities;

public partial class ProductsAboveAveragePrice
{
    public string ProductName { get; set; } = null!;

    public decimal? UnitPrice { get; set; }
}
