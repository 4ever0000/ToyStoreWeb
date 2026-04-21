namespace ToyStore.Application.Common.Exceptions;

public class DeleteFailureException : ApiException
{
    public DeleteFailureException(string name, object key, string message)
        : base($"\"{name}\" ({key}) silinməsi uğursuz oldu. {message}") { }
}