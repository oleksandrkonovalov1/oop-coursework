using AbiturientDirectory.Models;
using Newtonsoft.Json;

namespace AbiturientDirectory.Storage;

/// <summary>
/// Реалізація сховища поверх JSON-файлів: серіалізує колекції вузів і спеціальностей
/// у локальні файли. Усі операції виконуються під спільним блокуванням, тож читання
/// повертають незмінні знімки, а кожна мутація атомарно зберігає обидва файли.
/// </summary>
public class JsonDirectoryRepository : IDirectoryRepository
{
    private readonly string _dataDir;
    private readonly object _lock = new();
    private List<University> _universities = new();
    private List<Specialty> _specialties = new();

    /// <inheritdoc/>
    public bool LoadProblem { get; private set; }

    /// <summary>Створює сховище, що зберігає файли у вказаній директорії.</summary>
    /// <param name="dataDir">Шлях до директорії з файлами даних.</param>
    public JsonDirectoryRepository(string dataDir) => _dataDir = dataDir;

    /// <inheritdoc/>
    public void Load()
    {
        lock (_lock)
        {
            _universities = LoadFile<University>("universities.json");
            _specialties = LoadFile<Specialty>("specialties.json");
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<University> Universities()
    {
        lock (_lock) return _universities.ToArray();
    }

    /// <inheritdoc/>
    public IReadOnlyList<Specialty> Specialties()
    {
        lock (_lock) return _specialties.ToArray();
    }

    /// <inheritdoc/>
    public void AddUniversity(University u)
    {
        lock (_lock)
        {
            _universities.Add(u);
            Persist();
        }
    }

    /// <inheritdoc/>
    public void UpdateUniversity(University u)
    {
        // Сутність уже в колекції за посиланням — лишається тільки зберегти стан.
        lock (_lock) Persist();
    }

    /// <inheritdoc/>
    public int RemoveUniversity(University u)
    {
        lock (_lock)
        {
            var removed = _specialties.RemoveAll(s => s.UniversityId == u.Id);
            _universities.Remove(u);
            Persist();
            return removed;
        }
    }

    /// <inheritdoc/>
    public void AddSpecialty(Specialty s)
    {
        lock (_lock)
        {
            _specialties.Add(s);
            Persist();
        }
    }

    /// <inheritdoc/>
    public void UpdateSpecialty(Specialty s)
    {
        // Сутність уже в колекції за посиланням — лишається тільки зберегти стан.
        lock (_lock) Persist();
    }

    /// <inheritdoc/>
    public void RemoveSpecialty(Specialty s)
    {
        lock (_lock)
        {
            _specialties.Remove(s);
            Persist();
        }
    }

    /// <summary>Атомарно зберігає поточний стан обох колекцій у JSON-файли. Викликається лише під блокуванням.</summary>
    private void Persist()
    {
        Directory.CreateDirectory(_dataDir);
        SaveFile("universities.json", _universities);
        SaveFile("specialties.json", _specialties);
    }

    private List<T> LoadFile<T>(string fileName)
    {
        var path = Path.Combine(_dataDir, fileName);
        if (!File.Exists(path)) return new List<T>();
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path)) ?? new List<T>();
        }
        catch (JsonException)
        {
            LoadProblem = true;
            return new List<T>();
        }
    }

    private void SaveFile<T>(string fileName, List<T> items)
    {
        var path = Path.Combine(_dataDir, fileName);
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonConvert.SerializeObject(items, Formatting.Indented));
        File.Move(tmp, path, overwrite: true);
    }
}
