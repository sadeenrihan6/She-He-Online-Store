using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Payment
{
    public decimal Id { get; set; }

    public string? ExpiryDate { get; set; }

    public decimal? Balance { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CardNumber { get; set; }

    public string? Cvv { get; set; }
}
