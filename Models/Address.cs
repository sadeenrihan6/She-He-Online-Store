using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Address
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public string? StreetAddress { get; set; }

    public string? City { get; set; }

    public decimal? PhoneNumber { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ProductCustomer> ProductCustomers { get; set; } = new List<ProductCustomer>();
}
