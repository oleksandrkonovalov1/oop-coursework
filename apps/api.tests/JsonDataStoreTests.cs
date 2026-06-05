using AbiturientDirectory.Models;
using AbiturientDirectory.Storage;

namespace AbiturientDirectory.Tests;

public class JsonDataStoreTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "abitur-tests-" + Guid.NewGuid());

    public void Dispose() { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }

    [Fact]
    public void Load_MissingFiles_StartsEmpty()
    {
        var store = new JsonDataStore(_dir);
        store.Load();
        Assert.Empty(store.Universities);
        Assert.Empty(store.Specialties);
    }

    [Fact]
    public void SaveAndLoad_RoundTripsData()
    {
        var store = new JsonDataStore(_dir);
        store.Load();
        var uni = new University { Name = "ХНУРЕ", Address = "пр. Науки, 14" };
        store.Universities.Add(uni);
        store.Specialties.Add(new Specialty
        {
            UniversityId = uni.Id, Code = "121", Name = "Інженерія програмного забезпечення",
            ContractPrice = 32000m, Competition = new Competition { FullTime = 7.5m }
        });
        store.Save();

        var store2 = new JsonDataStore(_dir);
        store2.Load();
        Assert.Single(store2.Universities);
        Assert.Equal("ХНУРЕ", store2.Universities[0].Name);
        Assert.Single(store2.Specialties);
        Assert.Equal(7.5m, store2.Specialties[0].Competition.FullTime);
        Assert.Null(store2.Specialties[0].Competition.Evening);
    }

    [Fact]
    public void Load_CorruptedFile_StartsEmptyAndReportsProblem()
    {
        Directory.CreateDirectory(_dir);
        File.WriteAllText(Path.Combine(_dir, "universities.json"), "{ NOT JSON !!!");
        var store = new JsonDataStore(_dir);
        store.Load();
        Assert.Empty(store.Universities);
        Assert.True(store.LoadProblem); // прапорець для повідомлення користувачу
    }
}
