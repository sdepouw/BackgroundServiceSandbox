using System.ComponentModel.DataAnnotations;

namespace Core7Library;

public class CatFactsClientSettings
{
    [Required]
    public string Host { get; set; } = "";
}