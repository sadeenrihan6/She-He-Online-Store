using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Testimonial
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public string? TestimonialsText { get; set; }

    public string? TestimonialsStatus { get; set; }

    public virtual Customer? Customer { get; set; }
}
