namespace Core8WebAPI.Handlers;

public class SecretHandler : ISecretHandler
{
    public Task<string> HandleAsync(CancellationToken token) => Task.FromResult("You found the secret code!");
}