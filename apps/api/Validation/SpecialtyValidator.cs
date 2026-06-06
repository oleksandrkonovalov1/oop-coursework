using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;

namespace AbiturientDirectory.Validation;

/// <summary>
/// Валідація даних спеціальності (SRP — окремо від сервісу).
/// Чисті статичні методи: нормалізують значення або кидають <see cref="ValidationException"/>.
/// </summary>
public static class SpecialtyValidator
{
    /// <summary>
    /// Перевіряє та нормалізує дані спеціальності.
    /// </summary>
    /// <param name="input">Введені дані форми спеціальності.</param>
    /// <param name="existing">Поточний перелік спеціальностей для перевірки унікальності назви в межах вузу.</param>
    /// <param name="excludeId">Ідентифікатор спеціальності, яку слід виключити з перевірки унікальності (під час редагування).</param>
    /// <param name="universityId">Ідентифікатор вузу, у межах якого перевіряється унікальність назви.</param>
    /// <returns>Кортеж нормалізованих коду, назви, вартості та конкурсу.</returns>
    /// <exception cref="ValidationException">Якщо дані не пройшли перевірку.</exception>
    public static (string Code, string Name, decimal Price, Competition Competition) Validate(
        SpecialtyInput input, IReadOnlyList<Specialty> existing, Guid? excludeId, Guid universityId)
    {
        var errors = new Dictionary<string, string>();
        var code = (input.Code ?? "").Trim();
        var name = (input.Name ?? "").Trim();

        if (name.Length == 0) errors["name"] = "Вкажіть назву спеціальності";
        else if (name.Length > 200) errors["name"] = "Назва не може бути довшою за 200 символів";
        else if (existing.Any(s =>
                     s.Id != excludeId && s.UniversityId == universityId &&
                     string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)))
            errors["name"] = "Спеціальність із такою назвою у цього вузу вже є";

        if (code.Length > 10) errors["code"] = "Код не може бути довшим за 10 символів";

        if (input.ContractPrice is null or <= 0)
            errors["contractPrice"] = "Вкажіть вартість контракту — число, більше за нуль";
        else if (input.ContractPrice > 1_000_000m)
            errors["contractPrice"] = "Вкажіть реальну вартість — не більше 1 000 000 грн/рік";

        var c = input.Competition ?? new CompetitionInput(null, null, null);
        if (c.FullTime is < 0 || c.Evening is < 0 || c.PartTime is < 0)
            errors["competition"] = "Конкурс не може бути від'ємним";
        else if (c.FullTime is > 100 || c.Evening is > 100 || c.PartTime is > 100)
            errors["competition"] = "Конкурс не може перевищувати 100 осіб на місце";
        else if (c.FullTime is null && c.Evening is null && c.PartTime is null)
            errors["competition"] = "Заповніть конкурс хоча б за однією формою навчання";

        if (errors.Count > 0) throw new ValidationException(errors);
        return (code, name, input.ContractPrice!.Value,
            new Competition { FullTime = c.FullTime, Evening = c.Evening, PartTime = c.PartTime });
    }
}
