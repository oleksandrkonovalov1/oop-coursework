namespace AbiturientDirectory.Models;

/// <summary>Вищий навчальний заклад — основна сутність довідника.</summary>
public class University
{
    /// <summary>Унікальний ідентифікатор вузу.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Найменування вузу.</summary>
    public string Name { get; set; } = "";

    /// <summary>Адреса вузу.</summary>
    public string Address { get; set; } = "";
}
