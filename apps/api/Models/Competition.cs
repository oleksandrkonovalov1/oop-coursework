namespace AbiturientDirectory.Models;

public class Competition
{
    public decimal? FullTime { get; set; }

    public decimal? Evening { get; set; }

    public decimal? PartTime { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public bool HasAnyForm => FullTime.HasValue || Evening.HasValue || PartTime.HasValue;

    public decimal? ByForm(StudyForm form) => form switch
    {
        StudyForm.FullTime => FullTime,
        StudyForm.Evening => Evening,
        StudyForm.PartTime => PartTime,
        _ => null
    };
}
