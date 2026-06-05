namespace AbiturientDirectory.Services;

/// <summary>
/// Помилка валідації введених користувачем даних.
/// Містить словник «поле → зрозуміле повідомлення українською».
/// </summary>
public class ValidationException : Exception
{
    /// <summary>Повідомлення про помилки за полями форми.</summary>
    public IReadOnlyDictionary<string, string> Errors { get; }

    /// <summary>Створює виняток із переліком помилок за полями.</summary>
    public ValidationException(IReadOnlyDictionary<string, string> errors)
        : base("Дані заповнені з помилками") => Errors = errors;
}
