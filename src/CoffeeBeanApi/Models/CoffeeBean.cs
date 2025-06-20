using CoffeeBeanApi.Models;
using System.ComponentModel.DataAnnotations;

public class CoffeeBean
{
    public int Id { get; set; }

    [StringLength(100)]
    public string OriginalId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [StringLength(1000)]
    public required string Description { get; set; }

    public int CountryId { get; set; }
    public int ColourId { get; set; }

    public Country Country { get; set; } = null!;
    public Colour Colour { get; set; } = null!;

    [Required]
    [Range(0.01, 1000.00)]
    public decimal Cost { get; set; }

    [Url]
    public string? Image { get; set; }

    public bool IsBeanOfTheDay { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}