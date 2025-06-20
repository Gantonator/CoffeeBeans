using System.ComponentModel.DataAnnotations;

namespace CoffeeBeanApi.Models;

public class CoffeeBeanCreateInput
{
    [Required, StringLength(100)]
    public required string Name { get; set; }

    [Required, StringLength(1000)]
    public required string Description { get; set; }

    public int CountryId { get; set; }
    public int ColourId { get; set; }

    [Required, Range(0.01, 1000.00)]
    public decimal Cost { get; set; }

    [Url]
    public string? Image { get; set; }
}