using System.ComponentModel.DataAnnotations;

namespace WorkServiceEight;

public class MySettings
{
    [Required]
    public string Foo { get; set; } = "";
    public int Bar { get; set; }
    public bool Fizz { get; set; }
    public DateTimeOffset Buzz { get; set; }
    public bool EnableHttpRequestResponseLogging { get; set; }
}
