using AbiturientDirectory.Models;
using AbiturientDirectory.Storage;

namespace AbiturientDirectory.Tests;

public class JsonDirectoryRepositoryTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "abitur-tests-" + Guid.NewGuid());

    public void Dispose() { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }

    [Fact]
    public void Load_MissingFiles_StartsEmpty()
    {
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        Assert.Empty(repo.Universities());
        Assert.Empty(repo.Specialties());
    }

    [Fact]
    public void AddAndLoad_RoundTripsData()
    {
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        var uni = new University { Name = "ХНУРЕ", Address = "пр. Науки, 14" };
        repo.AddUniversity(uni);
        repo.AddSpecialty(new Specialty
        {
            UniversityId = uni.Id, Code = "121", Name = "Інженерія програмного забезпечення",
            ContractPrice = 32000m, Competition = new Competition { FullTime = 7.5m }
        });

        var repo2 = new JsonDirectoryRepository(_dir);
        repo2.Load();
        Assert.Single(repo2.Universities());
        Assert.Equal("ХНУРЕ", repo2.Universities()[0].Name);
        Assert.Single(repo2.Specialties());
        Assert.Equal(7.5m, repo2.Specialties()[0].Competition.FullTime);
        Assert.Null(repo2.Specialties()[0].Competition.Evening);
    }

    [Fact]
    public void Universities_ReturnsSnapshot_NotBackingList()
    {
        // Знімок не повинен дозволяти мутувати внутрішній стан сховища
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        repo.AddUniversity(new University { Name = "ХНУРЕ", Address = "Харків" });
        var snapshot = repo.Universities();
        Assert.NotSame(snapshot, repo.Universities());
        Assert.Single(repo.Universities());
    }

    [Fact]
    public void UpdateUniversity_PersistsReferencedEntityChange()
    {
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        var uni = new University { Name = "ХНУРЕ", Address = "Харків" };
        repo.AddUniversity(uni);
        uni.Name = "ХНУРЕ (Нурівський)";
        repo.UpdateUniversity(uni);

        var repo2 = new JsonDirectoryRepository(_dir);
        repo2.Load();
        Assert.Equal("ХНУРЕ (Нурівський)", repo2.Universities().Single().Name);
    }

    [Fact]
    public void RemoveUniversity_CascadesSpecialties_ReturnsCount()
    {
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        var uni = new University { Name = "ХНУРЕ", Address = "Харків" };
        repo.AddUniversity(uni);
        repo.AddSpecialty(new Specialty { UniversityId = uni.Id, Name = "А" });
        repo.AddSpecialty(new Specialty { UniversityId = uni.Id, Name = "Б" });
        var other = new University { Name = "КПІ", Address = "Київ" };
        repo.AddUniversity(other);
        repo.AddSpecialty(new Specialty { UniversityId = other.Id, Name = "В" });

        var removed = repo.RemoveUniversity(uni);

        Assert.Equal(2, removed);
        Assert.Single(repo.Universities());
        Assert.Single(repo.Specialties());
    }

    [Fact]
    public void RemoveSpecialty_RemovesAndPersists()
    {
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        var spec = new Specialty { Name = "А" };
        repo.AddSpecialty(spec);
        repo.RemoveSpecialty(spec);
        Assert.Empty(repo.Specialties());
    }

    [Fact]
    public void Load_CorruptedFile_StartsEmptyAndReportsProblem()
    {
        Directory.CreateDirectory(_dir);
        File.WriteAllText(Path.Combine(_dir, "universities.json"), "{ NOT JSON !!!");
        var repo = new JsonDirectoryRepository(_dir);
        repo.Load();
        Assert.Empty(repo.Universities());
        Assert.True(repo.LoadProblem); // прапорець для повідомлення користувачу
    }
}
