using AbiturientDirectory.Models;

namespace AbiturientDirectory.Contracts;

/// <summary>Відповідь «все щодо обраного вузу»: сам вуз та перелік його спеціальностей.</summary>
/// <param name="University">Обраний вуз.</param>
/// <param name="Specialties">Спеціальності цього вузу.</param>
public record UniversityDetailsResponse(University University, IReadOnlyList<Specialty> Specialties);
