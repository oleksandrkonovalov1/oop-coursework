namespace AbiturientDirectory.Services;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string> errors)
        : base("Дані заповнені з помилками") => Errors = errors;
}
