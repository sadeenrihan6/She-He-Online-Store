using System.ComponentModel.DataAnnotations;

public class ContactFormModel
{
    [Required]
    public string SenderName { get; set; }

    [Required]
    [EmailAddress]
    public string Sender { get; set; }

    [Required]
    public string Message { get; set; }
}
