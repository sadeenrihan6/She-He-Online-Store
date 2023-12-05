using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class OrderInfo
{
    public decimal Id { get; set; }

    public decimal? OrderId { get; set; }

    public decimal? CustomerId { get; set; }

    public decimal? ProductId { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ProductCustomer? Order { get; set; }

    public virtual Product? Product { get; set; }
}
