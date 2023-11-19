namespace Core8Library;

internal class HostValidationException(string message)
    : ApplicationException($"Exception occurred when validating the created Host: {message}");
