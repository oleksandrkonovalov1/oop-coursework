using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;

namespace AbiturientDirectory.Validation;

public static class UniversityValidator
{
    public static (string Name, string Address) Validate(
        UniversityInput input, IReadOnlyList<University> existing, Guid? excludeId)
    {
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
