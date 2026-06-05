using AbiturientDirectory.Models;

namespace AbiturientDirectory.Contracts;

/// <summary>Рядок запиту «все щодо обраної спеціальності»: вуз + його пропозиція спеціальності.</summary>
/// <param name="University">Вуз, що викладає спеціальність.</param>
/// <param name="Specialty">Дані спеціальності в цьому вузі.</param>
public record SpecialtyOffer(University University, Specialty Specialty);
