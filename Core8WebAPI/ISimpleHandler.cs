namespace Core8WebAPI;

public interface ISimpleHandler
{
    Task<string> HandleAsync(CancellationToken cancellationToken);
}
