namespace AbiturientDirectory.Contracts;

/// <summary>Відповідь на видалення вузу: кількість каскадно видалених спеціальностей.</summary>
/// <param name="DeletedSpecialties">Кількість видалених спеціальностей.</param>
public record DeleteUniversityResponse(int DeletedSpecialties);
