namespace ToyStore.Application.Common.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string name, object key)
        : base($"\"{name}\" ({key}) tapılmadı.") { }
}