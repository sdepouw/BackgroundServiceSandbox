using System.ComponentModel.DataAnnotations;
using Core7Library.CatFacts;

namespace WorkServiceEight;

public class MySettings
{
    [Required]
    public string Foo { get; set; }
    public int Bar { get; set; }
    public bool Fizz { get; set; }
    public DateTimeOffset Buzz { get; set; }
    public CatFactsClientSettings CatFactsClientSettings { get; set; }
}
