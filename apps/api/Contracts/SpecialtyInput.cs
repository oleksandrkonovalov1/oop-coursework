namespace AbiturientDirectory.Contracts;

/// <summary>Дані форми створення/редагування спеціальності.</summary>
/// <param name="Code">Код спеціальності.</param>
/// <param name="Name">Назва спеціальності.</param>
/// <param name="ContractPrice">Вартість контракту, грн/рік.</param>
/// <param name="Competition">Конкурс за формами навчання.</param>
public record SpecialtyInput(string? Code, string? Name, decimal? ContractPrice, CompetitionInput? Competition);
