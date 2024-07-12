using System;
using System.Collections.Generic;

namespace _2_TemelDüzeySorgulamaYapıları.Entities;

public partial class SalesByCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal? ProductSales { get; set; }
}
