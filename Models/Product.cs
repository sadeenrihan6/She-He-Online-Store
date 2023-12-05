using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

public partial class Product
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }
    public decimal? CategoryId { get; set; }

    public string? ProductDescription { get; set; }

    public decimal? Quantityinstock { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderInfo> OrderInfos { get; set; } = new List<OrderInfo>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
