using AbiturientDirectory.Models;

namespace AbiturientDirectory.Storage;

public interface IDirectoryRepository
{
    bool LoadProblem { get; }

    void Load();

    IReadOnlyList<University> Universities();

    IReadOnlyList<Specialty> Specialties();

    void AddUniversity(University u);

    void UpdateUniversity(University u);

    int RemoveUniversity(University u);

    void AddSpecialty(Specialty s);

    void UpdateSpecialty(Specialty s);

    void RemoveSpecialty(Specialty s);
}
