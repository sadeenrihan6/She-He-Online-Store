using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class SearchHistory
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public string? SearchQuery { get; set; }

    public virtual Customer? Customer { get; set; }
}
