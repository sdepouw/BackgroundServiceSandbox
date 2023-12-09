namespace Core8WebAPI.Handlers;

public interface ISimpleHandler
{
    Task<string> HandleAsync(CancellationToken cancellationToken);
}
