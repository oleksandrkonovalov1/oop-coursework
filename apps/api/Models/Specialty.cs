namespace AbiturientDirectory.Models;

/// <summary>Спеціальність, що викладається у конкретному вузі.</summary>
public class Specialty
{
    /// <summary>Унікальний ідентифікатор спеціальності.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Ідентифікатор вузу, якому належить спеціальність.</summary>
    public Guid UniversityId { get; set; }

    /// <summary>Код спеціальності (наприклад, "121").</summary>
    public string Code { get; set; } = "";

    /// <summary>Назва спеціальності.</summary>
    public string Name { get; set; } = "";

    /// <summary>Вартість контрактного навчання, грн/рік.</summary>
    public decimal ContractPrice { get; set; }

    /// <summary>Конкурс минулого року за формами навчання.</summary>
    public Competition Competition { get; set; } = new();
}
