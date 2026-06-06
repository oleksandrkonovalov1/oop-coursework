using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;

namespace AbiturientDirectory.Validation;

/// <summary>
/// Валідація даних вузу (SRP — окремо від сервісу).
/// Чисті статичні методи: нормалізують значення або кидають <see cref="ValidationException"/>.
/// </summary>
public static class UniversityValidator
{
    /// <summary>
    /// Перевіряє та нормалізує дані вузу.
    /// </summary>
    /// <param name="input">Введені дані форми вузу.</param>
    /// <param name="existing">Поточний перелік вузів для перевірки унікальності назви.</param>
    /// <param name="excludeId">Ідентифікатор вузу, який слід виключити з перевірки унікальності (під час редагування).</param>
    /// <returns>Кортеж нормалізованих назви та адреси.</returns>
    /// <exception cref="ValidationException">Якщо дані не пройшли перевірку.</exception>
    public static (string Name, string Address) Validate(
        UniversityInput input, IReadOnlyList<University> existing, Guid? excludeId)
    {
        // Інваріант: ключі словника — camelCase і збігаються з іменами полів форм фронтенда
        var errors = new Dictionary<string, string>();
        var name = (input.Name ?? "").Trim();
        var address = (input.Address ?? "").Trim();

        if (name.Length == 0) errors["name"] = "Вкажіть найменування вузу";
        else if (name.Length > 200) errors["name"] = "Найменування не може бути довшим за 200 символів";
        else if (existing.Any(u =>
                     u.Id != excludeId &&
                     string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase)))
            errors["name"] = "Вуз із таким найменуванням уже є в довіднику";

        if (address.Length == 0) errors["address"] = "Вкажіть адресу вузу";
        else if (address.Length > 300) errors["address"] = "Адреса не може бути довшою за 300 символів";

        if (errors.Count > 0) throw new ValidationException(errors);
        return (name, address);
    }
}
