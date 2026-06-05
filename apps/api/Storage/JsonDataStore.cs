using AbiturientDirectory.Models;
using Newtonsoft.Json;

namespace AbiturientDirectory.Storage;

/// <summary>
/// Локальне сховище даних: серіалізує колекції вузів і спеціальностей
/// у JSON-файли та завантажує їх при старті застосунку.
/// </summary>
public class JsonDataStore
{
    private readonly string _dataDir;
    private readonly object _lock = new();

    /// <summary>Колекція всіх вузів довідника.</summary>
    public List<University> Universities { get; private set; } = new();

    /// <summary>Колекція всіх спеціальностей довідника.</summary>
    public List<Specialty> Specialties { get; private set; } = new();

    /// <summary>Ознака, що під час завантаження виявлено пошкоджений файл даних.</summary>
    public bool LoadProblem { get; private set; }

    /// <summary>Створює сховище, що зберігає файли у вказаній директорії.</summary>
    public JsonDataStore(string dataDir) => _dataDir = dataDir;

    /// <summary>Завантажує дані з файлів; за відсутності або пошкодження — стартує з порожніми колекціями.</summary>
    public void Load()
    {
        Universities = LoadFile<University>("universities.json");
        Specialties = LoadFile<Specialty>("specialties.json");
    }

    /// <summary>Зберігає поточний стан колекцій у JSON-файли (атомарно, через тимчасовий файл).</summary>
    public void Save()
    {
        lock (_lock)
        {
            Directory.CreateDirectory(_dataDir);
            SaveFile("universities.json", Universities);
            SaveFile("specialties.json", Specialties);
        }
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
