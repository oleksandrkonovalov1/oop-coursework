export interface Competition {
  fullTime: number | null;
  evening: number | null;
  partTime: number | null;
}

export interface University {
  id: string;
  name: string;
  address: string;
}

export interface Specialty {
  id: string;
  universityId: string;
  code: string;
  name: string;
  contractPrice: number;
  competition: Competition;
}

export interface UniversityDetails {
  university: University;
  specialties: Specialty[];
}

export interface SpecialtyOffer {
  university: University;
  specialty: Specialty;
}

export type StudyForm = "FullTime" | "Evening" | "PartTime";

export interface MinCompetitionResult {
  university: University;
  specialty: Specialty;
  form: StudyForm;
  value: number;
}

export interface UniversityInput {
  name: string;
  address: string;
}

export interface SpecialtyInput {
  code: string;
  name: string;
  contractPrice: number;
  competition: Competition;
}

/** Помилки валідації з бекенда: поле → повідомлення українською. */
export type FieldErrors = Record<string, string>;
