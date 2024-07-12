using System;
using System.Collections.Generic;

namespace _4_AsNoTracking.Entities;

public partial class OrderSubtotal
{
    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
