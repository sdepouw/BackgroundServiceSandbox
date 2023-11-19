namespace Core8Library;

public class HostValidationException(string message)
    : ApplicationException($"Exception occurred when validating the created Host: {message}");
