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

    // --- Спеціальності ---

    private University Uni() => _svc.AddUniversity(new UniversityInput("ХНУРЕ", "пр. Науки, 14"));

    private static SpecialtyInput ValidSpec() => new("121", "Інженерія програмного забезпечення",
        32000m, new CompetitionInput(7.5m, null, 2.1m));

    [Fact]
    public void AddSpecialty_Valid_AddsLinkedToUniversity()
    {
        var uni = Uni();
        var spec = _svc.AddSpecialty(uni.Id, ValidSpec());
        Assert.Equal(uni.Id, spec.UniversityId);
        Assert.Equal(7.5m, spec.Competition.FullTime);
        Assert.Null(spec.Competition.Evening);
    }

    [Fact]
    public void AddSpecialty_NoFormsFilled_ThrowsValidation()
    {
        var uni = Uni();
        var input = new SpecialtyInput("121", "ІПЗ", 32000m, new CompetitionInput(null, null, null));
        var ex = Assert.Throws<ValidationException>(() => _svc.AddSpecialty(uni.Id, input));
        Assert.Contains("competition", ex.Errors.Keys);
    }

    [Fact]
    public void AddSpecialty_NegativeCompetitionOrPrice_ThrowsValidation()
    {
        var uni = Uni();
        var input = new SpecialtyInput("121", "ІПЗ", 0m, new CompetitionInput(-1m, null, null));
        var ex = Assert.Throws<ValidationException>(() => _svc.AddSpecialty(uni.Id, input));
        Assert.Contains("contractPrice", ex.Errors.Keys);
        Assert.Contains("competition", ex.Errors.Keys);
    }

    [Fact]
    public void AddSpecialty_UnknownUniversity_ThrowsKeyNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() => _svc.AddSpecialty(Guid.NewGuid(), ValidSpec()));
    }

    [Fact]
    public void UpdateSpecialty_Valid_Updates()
    {
        var uni = Uni();
        var spec = _svc.AddSpecialty(uni.Id, ValidSpec());
        _svc.UpdateSpecialty(spec.Id, new SpecialtyInput("122", "Комп'ютерні науки", 28000m,
            new CompetitionInput(5m, 1m, null)));
        var updated = _store.Specialties.Single();
        Assert.Equal("Комп'ютерні науки", updated.Name);
        Assert.Equal(1m, updated.Competition.Evening);
        Assert.Null(updated.Competition.PartTime);
    }

    [Fact]
    public void DeleteSpecialty_RemovesAndPersists()
    {
        var uni = Uni();
        var spec = _svc.AddSpecialty(uni.Id, ValidSpec());
        _svc.DeleteSpecialty(spec.Id);
        Assert.Empty(_store.Specialties);
    }

    // --- Каскадне видалення ---

    [Fact]
    public void DeleteUniversity_CascadesSpecialties_ReturnsCount()
    {
        var uni = Uni();
        _svc.AddSpecialty(uni.Id, ValidSpec());
        _svc.AddSpecialty(uni.Id, new SpecialtyInput("122", "Комп'ютерні науки", 28000m,
            new CompetitionInput(5m, null, null)));
        var other = _svc.AddUniversity(new UniversityInput("КПІ", "Київ"));
        _svc.AddSpecialty(other.Id, ValidSpec());

        var removed = _svc.DeleteUniversity(uni.Id);

        Assert.Equal(2, removed);
        Assert.Single(_store.Universities);
        Assert.Single(_store.Specialties);
        Assert.All(_store.Specialties, s => Assert.Equal(other.Id, s.UniversityId));
    }
}
