using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Wishlist
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public decimal? ProductId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
