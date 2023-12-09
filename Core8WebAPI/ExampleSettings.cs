using System.ComponentModel.DataAnnotations;

namespace Core8WebAPI;

public class ExampleSettings
{
    [Required] public string Name { get; set; } = "";
}