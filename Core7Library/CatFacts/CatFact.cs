namespace Core7Library.CatFacts;

public class CatFact
{
    public string User { get; set; } = "";
    public string Text { get; set; } = "";
    public bool Used { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
