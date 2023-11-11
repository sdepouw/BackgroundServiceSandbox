namespace Core7Library.BearerTokenStuff;

public class AuthToken
{
    public string Token { get; set; } = "";
    public int SecondsUntilExpiration { get; set; }
}