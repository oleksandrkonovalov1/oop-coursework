using AbiturientDirectory.Models;
using Newtonsoft.Json;

namespace AbiturientDirectory.Storage;

public class JsonDirectoryRepository : IDirectoryRepository
{
    private readonly string _dataDir;
    private readonly object _lock = new();
    private List<University> _universities = new();
    private List<Specialty> _specialties = new();

    public bool LoadProblem { get; private set; }

    public JsonDirectoryRepository(string dataDir) => _dataDir = dataDir;

    public void Load()
    {
        lock (_lock)
        {
            _universities = LoadFile<University>("universities.json");
            _specialties = LoadFile<Specialty>("specialties.json");
        }
    }

    public IReadOnlyList<University> Universities()
    {
        lock (_lock) return _universities.ToArray();
    }

    public IReadOnlyList<Specialty> Specialties()
    {
        lock (_lock) return _specialties.ToArray();
    }

    public void AddUniversity(University u)
    {
        lock (_lock)
        {
            _universities.Add(u);
            Persist();
        }
    }

    public void UpdateUniversity(University u)
    {
        lock (_lock) Persist();
    }

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

    public void AddSpecialty(Specialty s)
    {
        lock (_lock)
        {
            _specialties.Add(s);
            Persist();
        }
    }

    public void UpdateSpecialty(Specialty s)
    {
        lock (_lock) Persist();
    }

    public void RemoveSpecialty(Specialty s)
    {
        lock (_lock)
        {
            _specialties.Remove(s);
            Persist();
        }
    }

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
