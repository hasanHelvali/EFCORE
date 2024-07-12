using System;
using System.Collections.Generic;

namespace _2_TemelDüzeySorgulamaYapıları.Entities;

public partial class SummaryOfSalesByQuarter
{
    public DateTime? ShippedDate { get; set; }

    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
