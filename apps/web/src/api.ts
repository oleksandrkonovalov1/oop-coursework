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
    // Наш формат — {errors: {поле: "рядок"}}. Авто-400 від [ApiController] має іншу форму
    // (значення-масиви) — такі віддаємо як загальну помилку, а не в поля форми.
    const body = (await res.json().catch(() => null)) as { errors?: Record<string, unknown> } | null;
    const entries = Object.entries(body?.errors ?? {});
    if (entries.length > 0 && entries.every(([, v]) => typeof v === "string"))
      throw new ApiValidationError(body!.errors as FieldErrors);
    throw new Error("Некоректний запит до сервера");
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
  minCompetition: async (name: string, form: StudyForm) => {
    const res = await fetch(`/api/specialties/min-competition?name=${encodeURIComponent(name)}&form=${form}`);
    if (res.status === 404) return null;
    if (!res.ok) throw new Error(`Помилка сервера (${res.status})`);
    return (await res.json()) as MinCompetitionResult;
  },
};
