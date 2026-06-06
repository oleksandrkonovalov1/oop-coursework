namespace AbiturientDirectory.Models;

public class Specialty
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UniversityId { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public decimal ContractPrice { get; set; }

    public Competition Competition { get; set; } = new();
}
