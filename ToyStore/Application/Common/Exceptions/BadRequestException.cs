namespace ToyStore.Application.Common.Exceptions;

public class BadRequestException : ApiException
{
    public IDictionary<string, IEnumerable<string>>? Errors { get; }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, IDictionary<string, IEnumerable<string>> errors)
        : base(message)
    {
        Errors = errors;
    }
}