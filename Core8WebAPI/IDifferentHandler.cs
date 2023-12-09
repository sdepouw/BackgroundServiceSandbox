namespace Core8WebAPI;

public interface IDifferentHandler
{
    Task<string> GimmeLongWaitAsync(CancellationToken cancellationToken);
}