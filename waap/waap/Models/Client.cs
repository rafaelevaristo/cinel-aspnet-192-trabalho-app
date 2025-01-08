using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace wapp.Models;
public class Client
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name must not exceed 100 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, ErrorMessage = "Address must not exceed 200 characters.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, ErrorMessage = "City must not exceed 100 characters.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Postal code is required.")]
    [StringLength(20, ErrorMessage = "Postal code must not exceed 20 characters.")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "NIF is required.")]
    [RegularExpression(@"\d{9}", ErrorMessage = "NIF must be exactly 9 digits.")]
    public string NIF { get; set; }

    [Required(ErrorMessage = "Client number is required.")]
    [StringLength(20, ErrorMessage = "Client number must not exceed 20 characters.")]
    public string ClientNumber { get; set; }

    [ValidateNever] // Prevent validation of navigation property
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
