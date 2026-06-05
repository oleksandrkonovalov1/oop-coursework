namespace AbiturientDirectory.Contracts;

/// <summary>Дані форми створення/редагування вузу.</summary>
/// <param name="Name">Найменування вузу.</param>
/// <param name="Address">Адреса вузу.</param>
public record UniversityInput(string? Name, string? Address);
