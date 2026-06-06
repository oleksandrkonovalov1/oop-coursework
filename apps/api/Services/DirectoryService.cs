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

    // ===== Спеціальності =====

    /// <summary>Повертає всі спеціальності вказаного вузу («все щодо обраного вузу»).</summary>
    public List<Specialty> GetUniversitySpecialties(Guid universityId) =>
        _store.Specialties.Where(s => s.UniversityId == universityId)
            .OrderBy(s => s.Code).ThenBy(s => s.Name).ToList();

    /// <summary>Додає спеціальність до вузу після валідації та зберігає базу.</summary>
    public Specialty AddSpecialty(Guid universityId, SpecialtyInput input)
    {
        GetUniversity(universityId); // KeyNotFoundException, якщо вузу нема
        var validated = ValidateSpecialty(input, universityId, excludeId: null);
        var spec = new Specialty
        {
            UniversityId = universityId,
            Code = validated.Code,
            Name = validated.Name,
            ContractPrice = validated.Price,
            Competition = validated.Competition
        };
        _store.Specialties.Add(spec);
        _store.Save();
        return spec;
    }

    /// <summary>Оновлює спеціальність після валідації та зберігає базу.</summary>
    public Specialty UpdateSpecialty(Guid id, SpecialtyInput input)
    {
        var spec = _store.Specialties.FirstOrDefault(s => s.Id == id)
                   ?? throw new KeyNotFoundException("Спеціальність не знайдено");
        var validated = ValidateSpecialty(input, spec.UniversityId, excludeId: spec.Id);
        spec.Code = validated.Code;
        spec.Name = validated.Name;
        spec.ContractPrice = validated.Price;
        spec.Competition = validated.Competition;
        _store.Save();
        return spec;
    }

    /// <summary>Видаляє спеціальність і зберігає базу.</summary>
    public void DeleteSpecialty(Guid id)
    {
        var spec = _store.Specialties.FirstOrDefault(s => s.Id == id)
                   ?? throw new KeyNotFoundException("Спеціальність не знайдено");
        _store.Specialties.Remove(spec);
        _store.Save();
    }

    /// <summary>
    /// Видаляє вуз разом з усіма його спеціальностями (каскадно, для цілісності даних).
    /// Повертає кількість видалених спеціальностей.
    /// </summary>
    public int DeleteUniversity(Guid id)
    {
        var uni = GetUniversity(id);
        var removed = _store.Specialties.RemoveAll(s => s.UniversityId == id);
        _store.Universities.Remove(uni);
        _store.Save();
        return removed;
    }

    // ===== Запити завдання =====

    /// <summary>Повертає всі відомі назви спеціальностей (без дублів, за абеткою).</summary>
    public List<string> GetSpecialtyNames() =>
        _store.Specialties.Select(s => s.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// «Все щодо обраної спеціальності»: пропозиції всіх вузів за назвою спеціальності,
    /// з необов'язковим фільтром за максимальною вартістю контракту.
    /// </summary>
    public List<SpecialtyOffer> GetOffers(string name, decimal? maxPrice)
    {
        var query = _store.Specialties
            .Where(s => string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        if (maxPrice.HasValue)
            query = query.Where(s => s.ContractPrice <= maxPrice.Value);
        return query
            .Select(s => new SpecialtyOffer(GetUniversity(s.UniversityId), s))
            .OrderBy(o => o.University.Name)
            .ToList();
    }

    /// <summary>
    /// Пошук мінімального конкурсу з даної спеціальності за обраною формою навчання.
    /// Вузи, де форма не ведеться (null), пропускаються. Повертає null, якщо даних немає.
    /// </summary>
    public MinCompetitionResult? GetMinCompetition(string name, StudyForm form)
    {
        var best = _store.Specialties
            .Where(s => string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(s => s.Competition.ByForm(form).HasValue)
            .OrderBy(s => s.Competition.ByForm(form)!.Value)
            .FirstOrDefault();
        return best is null
            ? null
            : new MinCompetitionResult(GetUniversity(best.UniversityId), best, form,
                best.Competition.ByForm(form)!.Value);
    }

    private (string Code, string Name, decimal Price, Competition Competition) ValidateSpecialty(
        SpecialtyInput input, Guid universityId, Guid? excludeId)
    {
        var errors = new Dictionary<string, string>();
        var code = (input.Code ?? "").Trim();
        var name = (input.Name ?? "").Trim();

        if (name.Length == 0) errors["name"] = "Вкажіть назву спеціальності";
        else if (name.Length > 200) errors["name"] = "Назва не може бути довшою за 200 символів";
        else if (_store.Specialties.Any(s =>
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
