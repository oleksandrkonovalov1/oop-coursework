using AbiturientDirectory.Models;

namespace AbiturientDirectory.Contracts;

public record UniversityDetailsResponse(University University, IReadOnlyList<Specialty> Specialties);
