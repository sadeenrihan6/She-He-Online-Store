using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ShoppingCart
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public decimal? ProductId { get; set; }

    public decimal? Quantity { get; set; }

    public DateTime? CartDate { get; set; }

    public string? CouponCode { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
