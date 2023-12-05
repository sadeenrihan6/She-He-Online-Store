using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ProductCustomer
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public decimal? Quantity { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? CouponCode { get; set; }

    public string? OrderStatus { get; set; }

    public decimal? AddressId { get; set; }

    public string? Isemailsent { get; set; }

    public virtual Address? Address { get; set; }

    public virtual Coupon? CouponCodeNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderInfo> OrderInfos { get; set; } = new List<OrderInfo>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
