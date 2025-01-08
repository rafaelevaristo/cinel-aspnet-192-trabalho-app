using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace wapp.Models;
public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Identifier must not exceed 50 characters.")]
    public string Identifier { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public DateTime Time { get; set; }

    [Required]
    public int ClientId { get; set; }

    [ForeignKey("ClientId")]
    [ValidateNever]
    public Client Client { get; set; }

    public List<SaleProduct> SaleProducts { get; set; }

    [StringLength(500, ErrorMessage = "Observations cannot exceed 500 characters.")]
    public string Observations { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Final value must be greater than 0.")]
    public decimal FinalValue { get; set; }

    [Required]
    public SaleState State { get; set; }

    [Required]
    public bool IsPaid { get; set; }
}
