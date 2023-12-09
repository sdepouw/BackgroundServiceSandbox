namespace Core8WebAPI;

public interface IExampleHandler
{
    Task<string> GimmeAsync(CancellationToken cancellationToken);
}
