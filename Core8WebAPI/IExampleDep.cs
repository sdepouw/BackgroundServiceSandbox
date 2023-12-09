namespace Core8WebAPI;

public interface IExampleDep
{
    Task<string> GimmeAsync(CancellationToken cancellationToken);
}
