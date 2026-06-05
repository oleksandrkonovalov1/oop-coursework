using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Storage;

namespace AbiturientDirectory.Services;

/// <summary>
/// Основний сервіс довідника: операції над колекціями вузів і спеціальностей,
/// валідація введених даних, підтримка цілісності зв'язків та пошукові запити.
/// </summary>
public class DirectoryService
{
    private readonly JsonDataStore _store;

    /// <summary>Створює сервіс поверх сховища даних.</summary>
    public DirectoryService(JsonDataStore store) => _store = store;

    // ===== Вузи =====

    /// <summary>Повертає вузи, відфільтровані за підрядком назви або адреси (без урахування регістру).</summary>
    public List<University> SearchUniversities(string? query)
    {
        var q = (query ?? "").Trim();
        if (q.Length == 0) return _store.Universities.OrderBy(u => u.Name).ToList();
        return _store.Universities
            .Where(u => u.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                     || u.Address.Contains(q, StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.Name)
            .ToList();
    }

    /// <summary>Повертає вуз за ідентифікатором або кидає KeyNotFoundException.</summary>
    public University GetUniversity(Guid id) =>
        _store.Universities.FirstOrDefault(u => u.Id == id)
        ?? throw new KeyNotFoundException("Вуз не знайдено");

    /// <summary>Додає новий вуз після валідації та зберігає базу.</summary>
    public University AddUniversity(UniversityInput input)
    {
        var (name, address) = ValidateUniversity(input, excludeId: null);
        var uni = new University { Name = name, Address = address };
        _store.Universities.Add(uni);
        _store.Save();
        return uni;
    }

    /// <summary>Оновлює дані вузу після валідації та зберігає базу.</summary>
    public University UpdateUniversity(Guid id, UniversityInput input)
    {
        var uni = GetUniversity(id);
        var (name, address) = ValidateUniversity(input, excludeId: id);
        uni.Name = name;
        uni.Address = address;
        _store.Save();
        return uni;
    }

    private (string Name, string Address) ValidateUniversity(UniversityInput input, Guid? excludeId)
    {
        // Інваріант: ключі словника — camelCase і збігаються з іменами полів форм фронтенда
        var errors = new Dictionary<string, string>();
        var name = (input.Name ?? "").Trim();
        var address = (input.Address ?? "").Trim();

        if (name.Length == 0) errors["name"] = "Вкажіть найменування вузу";
        else if (name.Length > 200) errors["name"] = "Найменування не може бути довшим за 200 символів";
        else if (_store.Universities.Any(u =>
                     u.Id != excludeId &&
                     string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase)))
            errors["name"] = "Вуз із таким найменуванням уже є в довіднику";

        if (address.Length == 0) errors["address"] = "Вкажіть адресу вузу";
        else if (address.Length > 300) errors["address"] = "Адреса не може бути довшою за 300 символів";

        if (errors.Count > 0) throw new ValidationException(errors);
        return (name, address);
    }
}
