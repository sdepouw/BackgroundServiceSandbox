using System.ComponentModel.DataAnnotations;

namespace Core8WebAPI;

public class ExampleSettings
{
    [Required] public string Name { get; set; } = "";
    [Required] public int HowLongToWaitInSeconds { get; set; }
    [Required] public string APISecretKey { get; set; } = "";
    public bool LogHttpRequestResponse { get; set; }
}
