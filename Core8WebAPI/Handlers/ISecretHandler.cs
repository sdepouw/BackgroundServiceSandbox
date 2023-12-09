namespace Core8WebAPI.Handlers;

public interface ISecretHandler
{
    Task<string> HandleAsync(CancellationToken token);
}