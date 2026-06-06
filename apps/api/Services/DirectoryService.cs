using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Storage;
using AbiturientDirectory.Validation;

namespace AbiturientDirectory.Services;

/// <summary>
/// Основний сервіс довідника: операції над колекціями вузів і спеціальностей,
/// валідація введених даних, підтримка цілісності зв'язків та пошукові запити.
/// </summary>
public class DirectoryService
{
    private readonly IDirectoryRepository _repo;

    /// <summary>Створює сервіс поверх сховища даних.</summary>
    /// <param name="repo">Сховище даних довідника.</param>
    public DirectoryService(IDirectoryRepository repo) => _repo = repo;

    // ===== Вузи =====

    /// <summary>Повертає вузи, відфільтровані за підрядком назви або адреси (без урахування регістру).</summary>
    /// <param name="query">Підрядок для пошуку; порожній — повертає всі вузи.</param>
    /// <returns>Відсортований за назвою перелік вузів.</returns>
    public List<University> SearchUniversities(string? query)
    {
        var q = (query ?? "").Trim();
        var all = _repo.Universities();
        if (q.Length == 0) return all.OrderBy(u => u.Name).ToList();
        return all
            .Where(u => u.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                     || u.Address.Contains(q, StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.Name)
            .ToList();
    }

    /// <summary>Повертає вуз за ідентифікатором або кидає KeyNotFoundException.</summary>
    /// <param name="id">Ідентифікатор вузу.</param>
    /// <returns>Знайдений вуз.</returns>
    /// <exception cref="KeyNotFoundException">Якщо вуз із таким ідентифікатором відсутній.</exception>
    public University GetUniversity(Guid id) =>
        _repo.Universities().FirstOrDefault(u => u.Id == id)
        ?? throw new KeyNotFoundException("Вуз не знайдено");

    /// <summary>Додає новий вуз після валідації та зберігає базу.</summary>
    /// <param name="input">Дані нового вузу.</param>
    /// <returns>Створений вуз.</returns>
    public University AddUniversity(UniversityInput input)
    {
        var (name, address) = UniversityValidator.Validate(input, _repo.Universities(), excludeId: null);
        var uni = new University { Name = name, Address = address };
        _repo.AddUniversity(uni);
        return uni;
    }

    /// <summary>Оновлює дані вузу після валідації та зберігає базу.</summary>
    /// <param name="id">Ідентифікатор вузу.</param>
    /// <param name="input">Нові дані вузу.</param>
    /// <returns>Оновлений вуз.</returns>
    public University UpdateUniversity(Guid id, UniversityInput input)
    {
        var uni = GetUniversity(id);
        var (name, address) = UniversityValidator.Validate(input, _repo.Universities(), excludeId: id);
        uni.Name = name;
        uni.Address = address;
        _repo.UpdateUniversity(uni);
        return uni;
    }

    /// <summary>
    /// Видаляє вуз разом з усіма його спеціальностями (каскадно, для цілісності даних).
    /// Повертає кількість видалених спеціальностей.
    /// </summary>
    /// <param name="id">Ідентифікатор вузу.</param>
    /// <returns>Кількість видалених спеціальностей.</returns>
    public int DeleteUniversity(Guid id)
    {
        var uni = GetUniversity(id);
        return _repo.RemoveUniversity(uni);
    }

    // ===== Спеціальності =====

    /// <summary>Повертає всі спеціальності вказаного вузу («все щодо обраного вузу»).</summary>
    /// <param name="universityId">Ідентифікатор вузу.</param>
    /// <returns>Відсортований перелік спеціальностей вузу.</returns>
    public List<Specialty> GetUniversitySpecialties(Guid universityId) =>
        _repo.Specialties().Where(s => s.UniversityId == universityId)
            .OrderBy(s => s.Code).ThenBy(s => s.Name).ToList();

    /// <summary>Додає спеціальність до вузу після валідації та зберігає базу.</summary>
    /// <param name="universityId">Ідентифікатор вузу.</param>
    /// <param name="input">Дані нової спеціальності.</param>
    /// <returns>Створена спеціальність.</returns>
    public Specialty AddSpecialty(Guid universityId, SpecialtyInput input)
    {
        GetUniversity(universityId); // KeyNotFoundException, якщо вузу нема
        var validated = SpecialtyValidator.Validate(input, _repo.Specialties(), excludeId: null, universityId);
        var spec = new Specialty
        {
            UniversityId = universityId,
            Code = validated.Code,
            Name = validated.Name,
            ContractPrice = validated.Price,
            Competition = validated.Competition
        };
        _repo.AddSpecialty(spec);
        return spec;
    }

    /// <summary>Оновлює спеціальність після валідації та зберігає базу.</summary>
    /// <param name="id">Ідентифікатор спеціальності.</param>
    /// <param name="input">Нові дані спеціальності.</param>
    /// <returns>Оновлена спеціальність.</returns>
    /// <exception cref="KeyNotFoundException">Якщо спеціальність із таким ідентифікатором відсутня.</exception>
    public Specialty UpdateSpecialty(Guid id, SpecialtyInput input)
    {
        var spec = _repo.Specialties().FirstOrDefault(s => s.Id == id)
                   ?? throw new KeyNotFoundException("Спеціальність не знайдено");
        var validated = SpecialtyValidator.Validate(input, _repo.Specialties(), excludeId: spec.Id, spec.UniversityId);
        spec.Code = validated.Code;
        spec.Name = validated.Name;
        spec.ContractPrice = validated.Price;
        spec.Competition = validated.Competition;
        _repo.UpdateSpecialty(spec);
        return spec;
    }

    /// <summary>Видаляє спеціальність і зберігає базу.</summary>
    /// <param name="id">Ідентифікатор спеціальності.</param>
    /// <exception cref="KeyNotFoundException">Якщо спеціальність із таким ідентифікатором відсутня.</exception>
    public void DeleteSpecialty(Guid id)
    {
        var spec = _repo.Specialties().FirstOrDefault(s => s.Id == id)
                   ?? throw new KeyNotFoundException("Спеціальність не знайдено");
        _repo.RemoveSpecialty(spec);
    }

    // ===== Запити завдання =====

    /// <summary>Повертає всі відомі назви спеціальностей (без дублів, за абеткою).</summary>
    /// <returns>Унікальні назви спеціальностей, відсортовані за абеткою.</returns>
    public List<string> GetSpecialtyNames() =>
        _repo.Specialties().Select(s => s.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// «Все щодо обраної спеціальності»: пропозиції всіх вузів за назвою спеціальності,
    /// з необов'язковим фільтром за максимальною вартістю контракту.
    /// </summary>
    /// <param name="name">Назва спеціальності.</param>
    /// <param name="maxPrice">Максимальна вартість контракту або null (без фільтра).</param>
    /// <returns>Перелік пропозицій вузів, відсортований за назвою вузу.</returns>
    public List<SpecialtyOffer> GetOffers(string name, decimal? maxPrice)
    {
        var query = _repo.Specialties()
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
    /// <param name="name">Назва спеціальності.</param>
    /// <param name="form">Форма навчання.</param>
    /// <returns>Результат пошуку мінімального конкурсу або null, якщо даних немає.</returns>
    public MinCompetitionResult? GetMinCompetition(string name, StudyForm form)
    {
        var best = _repo.Specialties()
            .Where(s => string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(s => s.Competition.ByForm(form).HasValue)
            .OrderBy(s => s.Competition.ByForm(form)!.Value)
            .FirstOrDefault();
        return best is null
            ? null
            : new MinCompetitionResult(GetUniversity(best.UniversityId), best, form,
                best.Competition.ByForm(form)!.Value);
    }
}
