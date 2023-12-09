namespace Core8WebAPI.Handlers;

public interface IPatienceHandler
{
    Task<string> HandleAsync(CancellationToken cancellationToken);
}