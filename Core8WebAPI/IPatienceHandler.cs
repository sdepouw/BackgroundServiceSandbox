namespace Core8WebAPI;

public interface IPatienceHandler
{
    Task<string> HandleAsync(CancellationToken cancellationToken);
}