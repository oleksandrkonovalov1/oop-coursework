namespace AbiturientDirectory.Contracts;

/// <summary>Конкурс за формами навчання у формі введення (null — форма не ведеться).</summary>
/// <param name="FullTime">Денна форма, осіб на місце.</param>
/// <param name="Evening">Вечірня форма, осіб на місце.</param>
/// <param name="PartTime">Заочна форма, осіб на місце.</param>
public record CompetitionInput(decimal? FullTime, decimal? Evening, decimal? PartTime);
