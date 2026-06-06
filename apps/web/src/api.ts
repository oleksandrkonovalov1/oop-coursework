import type {
  FieldErrors, MinCompetitionResult, Specialty, SpecialtyInput,
  SpecialtyOffer, StudyForm, University, UniversityDetails, UniversityInput,
} from "./types";

/** Помилка валідації від сервера (статус 400) з повідомленнями за полями. */
export class ApiValidationError extends Error {
  errors: FieldErrors;
  constructor(errors: FieldErrors) {
    super("Дані заповнені з помилками");
    this.errors = errors;
  }
}

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...init,
  });
  if (res.status === 400) {
    // RFC 9457 ValidationProblemDetails: errors — об'єкт {поле: [повідомлення, …]}.
    // Сплющуємо до {поле: перше_повідомлення}, бо форми читають значення як рядок.
    const body = (await res.json().catch(() => null)) as
      | { title?: string; errors?: Record<string, string[]> }
      | null;
    const fieldErrors: FieldErrors = {};
    for (const [field, messages] of Object.entries(body?.errors ?? {}))
      if (messages?.length) fieldErrors[field] = messages[0];
    if (Object.keys(fieldErrors).length > 0) throw new ApiValidationError(fieldErrors);
    throw new Error(body?.title ?? "Помилка сервера");
  }
  if (!res.ok) throw new Error("Помилка сервера. Спробуйте ще раз.");
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

export const api = {
  status: () => request<{ loadProblem: boolean }>("/api/status"),
  searchUniversities: (query: string) =>
    request<University[]>(`/api/universities?query=${encodeURIComponent(query)}`),
  getUniversity: (id: string) => request<UniversityDetails>(`/api/universities/${id}`),
  addUniversity: (input: UniversityInput) =>
    request<University>("/api/universities", { method: "POST", body: JSON.stringify(input) }),
  updateUniversity: (id: string, input: UniversityInput) =>
    request<University>(`/api/universities/${id}`, { method: "PUT", body: JSON.stringify(input) }),
  deleteUniversity: (id: string) =>
    request<{ deletedSpecialties: number }>(`/api/universities/${id}`, { method: "DELETE" }),
  addSpecialty: (universityId: string, input: SpecialtyInput) =>
    request<Specialty>(`/api/universities/${universityId}/specialties`, { method: "POST", body: JSON.stringify(input) }),
  updateSpecialty: (id: string, input: SpecialtyInput) =>
    request<Specialty>(`/api/specialties/${id}`, { method: "PUT", body: JSON.stringify(input) }),
  deleteSpecialty: (id: string) =>
    request<void>(`/api/specialties/${id}`, { method: "DELETE" }),
  specialtyNames: () => request<string[]>("/api/specialties/names"),
  offers: (name: string, maxPrice: number | null) =>
    request<SpecialtyOffer[]>(
      `/api/specialties/offers?name=${encodeURIComponent(name)}${maxPrice != null ? `&maxPrice=${maxPrice}` : ""}`),
  // Завжди 200; тіло — MinCompetitionResult або null (за обраною формою даних немає).
  minCompetition: (name: string, form: StudyForm) =>
    request<MinCompetitionResult | null>(
      `/api/specialties/min-competition?name=${encodeURIComponent(name)}&form=${form}`),
};
