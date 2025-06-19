using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeBeanApi.Models;

public class Colour
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    [JsonIgnore]
    public ICollection<CoffeeBean> CoffeeBeans { get; set; } = new List<CoffeeBean>();
}