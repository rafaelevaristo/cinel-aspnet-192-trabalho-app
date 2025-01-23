using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using waap;


namespace wapp.Models;
public class Category
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "CategoryName")]
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
    public string Name { get; set; }

    [Display(Name = "CategoryDescription")]
    [StringLength(200, ErrorMessage = "Description must not exceed 200 characters.")]
    public string Description { get; set; }

    [ValidateNever] // Prevent validation of navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
