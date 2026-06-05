using AbiturientDirectory.Models;

namespace AbiturientDirectory.Contracts;

/// <summary>Результат пошуку мінімального конкурсу зі спеціальності.</summary>
/// <param name="University">Вуз із мінімальним конкурсом.</param>
/// <param name="Specialty">Спеціальність у цьому вузі.</param>
/// <param name="Form">Форма навчання, за якою шукали.</param>
/// <param name="Value">Значення мінімального конкурсу (осіб на місце).</param>
public record MinCompetitionResult(University University, Specialty Specialty, StudyForm Form, decimal Value);
