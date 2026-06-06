namespace AbiturientDirectory.Models;

public class University
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public string Address { get; set; } = "";
}
