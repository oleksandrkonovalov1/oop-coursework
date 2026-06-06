using AbiturientDirectory.Models;

namespace AbiturientDirectory.Storage;

/// <summary>
/// Сховище даних довідника: інкапсулює колекції вузів і спеціальностей,
/// гарантує потокобезпечний доступ та автоматичне збереження після кожної мутації.
/// </summary>
public interface IDirectoryRepository
{
    /// <summary>Ознака, що під час завантаження виявлено пошкоджений файл даних.</summary>
    bool LoadProblem { get; }

    /// <summary>Завантажує дані з файлів; за відсутності або пошкодження — стартує з порожніми колекціями.</summary>
    void Load();

    /// <summary>Повертає незмінний знімок усіх вузів (копія під блокуванням).</summary>
    IReadOnlyList<University> Universities();

    /// <summary>Повертає незмінний знімок усіх спеціальностей (копія під блокуванням).</summary>
    IReadOnlyList<Specialty> Specialties();

    /// <summary>Додає новий вуз і зберігає базу.</summary>
    void AddUniversity(University u);

    /// <summary>Зберігає базу після зміни вже наявного в колекції вузу (сутність змінюється за посиланням).</summary>
    void UpdateUniversity(University u);

    /// <summary>
    /// Видаляє вуз разом з усіма його спеціальностями (каскадно) одним збереженням.
    /// Повертає кількість видалених спеціальностей.
    /// </summary>
    int RemoveUniversity(University u);

    /// <summary>Додає нову спеціальність і зберігає базу.</summary>
    void AddSpecialty(Specialty s);

    /// <summary>Зберігає базу після зміни вже наявної в колекції спеціальності (сутність змінюється за посиланням).</summary>
    void UpdateSpecialty(Specialty s);

    /// <summary>Видаляє спеціальність і зберігає базу.</summary>
    void RemoveSpecialty(Specialty s);
}
