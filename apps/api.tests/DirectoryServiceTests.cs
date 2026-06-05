using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;
using AbiturientDirectory.Storage;

namespace AbiturientDirectory.Tests;

public class DirectoryServiceTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "abitur-svc-" + Guid.NewGuid());
    private readonly DirectoryService _svc;
    private readonly JsonDataStore _store;

    public DirectoryServiceTests()
    {
        _store = new JsonDataStore(_dir);
        _store.Load();
        _svc = new DirectoryService(_store);
    }

    public void Dispose() { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }

    // --- CRUD вузів ---

    [Fact]
    public void AddUniversity_Valid_AddsAndPersists()
    {
        var uni = _svc.AddUniversity(new UniversityInput("ХНУРЕ", "пр. Науки, 14"));
        Assert.Equal("ХНУРЕ", uni.Name);
        var reloaded = new JsonDataStore(_dir); reloaded.Load();
        Assert.Single(reloaded.Universities);
    }

    [Fact]
    public void AddUniversity_EmptyName_ThrowsValidation()
    {
        var ex = Assert.Throws<ValidationException>(() => _svc.AddUniversity(new UniversityInput("  ", "адреса")));
        Assert.Contains("name", ex.Errors.Keys);
    }

    [Fact]
    public void AddUniversity_DuplicateName_ThrowsValidation()
    {
        _svc.AddUniversity(new UniversityInput("ХНУРЕ", "пр. Науки, 14"));
        var ex = Assert.Throws<ValidationException>(() => _svc.AddUniversity(new UniversityInput("хнуре", "інша")));
        Assert.Contains("name", ex.Errors.Keys);
    }

    [Fact]
    public void AddUniversity_TooLongFields_ThrowsValidation()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _svc.AddUniversity(new UniversityInput(new string('а', 201), new string('б', 301))));
        Assert.Contains("name", ex.Errors.Keys);
        Assert.Contains("address", ex.Errors.Keys);
    }

    [Fact]
    public void UpdateUniversity_Valid_Updates()
    {
        var uni = _svc.AddUniversity(new UniversityInput("ХНУРЕ", "пр. Науки, 14"));
        _svc.UpdateUniversity(uni.Id, new UniversityInput("ХНУРЕ (Нурівський)", "пр. Науки, 14"));
        Assert.Equal("ХНУРЕ (Нурівський)", _store.Universities.Single().Name);
    }

    [Fact]
    public void UpdateUniversity_Unknown_ThrowsKeyNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() =>
            _svc.UpdateUniversity(Guid.NewGuid(), new UniversityInput("X", "Y")));
    }

    [Fact]
    public void SearchUniversities_FiltersByNameOrAddressCaseInsensitive()
    {
        _svc.AddUniversity(new UniversityInput("ХНУРЕ", "Харків, пр. Науки, 14"));
        _svc.AddUniversity(new UniversityInput("КПІ", "Київ, пр. Берестейський, 37"));
        Assert.Single(_svc.SearchUniversities("харків"));
        Assert.Single(_svc.SearchUniversities("кпі"));
        Assert.Equal(2, _svc.SearchUniversities(null).Count);
        Assert.Empty(_svc.SearchUniversities("львів"));
    }
}
