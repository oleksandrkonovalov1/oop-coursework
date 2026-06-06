using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Storage;
using AbiturientDirectory.Validation;

namespace AbiturientDirectory.Services;

public class DirectoryService
{
    private readonly IDirectoryRepository _repo;

    public DirectoryService(IDirectoryRepository repo) => _repo = repo;


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

    public University GetUniversity(Guid id) =>
        _repo.Universities().FirstOrDefault(u => u.Id == id)
        ?? throw new KeyNotFoundException("Вуз не знайдено");

    public University AddUniversity(UniversityInput input)
    {
        var (name, address) = UniversityValidator.Validate(input, _repo.Universities(), excludeId: null);
        var uni = new University { Name = name, Address = address };
        _repo.AddUniversity(uni);
        return uni;
    }

    public University UpdateUniversity(Guid id, UniversityInput input)
    {
        var uni = GetUniversity(id);
        var (name, address) = UniversityValidator.Validate(input, _repo.Universities(), excludeId: id);
        uni.Name = name;
        uni.Address = address;
        _repo.UpdateUniversity(uni);
        return uni;
    }

    public int DeleteUniversity(Guid id)
    {
        var uni = GetUniversity(id);
        return _repo.RemoveUniversity(uni);
    }


    public List<Specialty> GetUniversitySpecialties(Guid universityId) =>
        _repo.Specialties().Where(s => s.UniversityId == universityId)
            .OrderBy(s => s.Code).ThenBy(s => s.Name).ToList();

    public Specialty AddSpecialty(Guid universityId, SpecialtyInput input)
    {
        GetUniversity(universityId);
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

    public void DeleteSpecialty(Guid id)
    {
        var spec = _repo.Specialties().FirstOrDefault(s => s.Id == id)
                   ?? throw new KeyNotFoundException("Спеціальність не знайдено");
        _repo.RemoveSpecialty(spec);
    }


    public List<string> GetSpecialtyNames() =>
        _repo.Specialties().Select(s => s.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

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
