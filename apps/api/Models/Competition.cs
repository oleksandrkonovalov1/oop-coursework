namespace AbiturientDirectory.Models;

/// <summary>
/// Конкурс минулого року за формами навчання (осіб на місце).
/// Значення null означає, що відповідна форма навчання не ведеться.
/// </summary>
public class Competition
{
    /// <summary>Конкурс на денну форму навчання або null.</summary>
    public decimal? FullTime { get; set; }

    /// <summary>Конкурс на вечірню форму навчання або null.</summary>
    public decimal? Evening { get; set; }

    /// <summary>Конкурс на заочну форму навчання або null.</summary>
    public decimal? PartTime { get; set; }

    /// <summary>Чи задано конкурс хоча б за однією формою навчання.</summary>
    [Newtonsoft.Json.JsonIgnore] // доменна логіка, не частина JSON-контракту і файлів даних
    public bool HasAnyForm => FullTime.HasValue || Evening.HasValue || PartTime.HasValue;

    /// <summary>Повертає конкурс за вказаною формою навчання.</summary>
    public decimal? ByForm(StudyForm form) => form switch
    {
        StudyForm.FullTime => FullTime,
        StudyForm.Evening => Evening,
        StudyForm.PartTime => PartTime,
        _ => null
    };
}
