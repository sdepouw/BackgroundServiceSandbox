namespace Core8WebAPI;

public interface IExampleHandler
{
    Task<string> GimmeAsync(CancellationToken cancellationToken);
    Task<string> GimmeLongWaitAsync(CancellationToken cancellationToken);
}
