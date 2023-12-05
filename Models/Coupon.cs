using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Coupon
{
    public decimal Id { get; set; }

    public string CouponCode { get; set; } = null!;

    public decimal? DiscountValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ProductCustomer> ProductCustomers { get; set; } = new List<ProductCustomer>();
}
