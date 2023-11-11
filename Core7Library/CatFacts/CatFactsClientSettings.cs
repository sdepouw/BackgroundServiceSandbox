using System.ComponentModel.DataAnnotations;

namespace Core7Library.CatFacts;

public class CatFactsClientSettings
{
    [Required]
    public string Host { get; set; } = "";
    [Required]
    public string GetTheFactsRoute { get; set; } = "";
}
