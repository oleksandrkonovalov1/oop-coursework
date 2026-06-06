import { Link } from "react-router";
import type { MinCompetitionResult, StudyForm } from "../types";

const FORM_LABELS: Record<StudyForm, string> = {
  FullTime: "денна", Evening: "вечірня", PartTime: "заочна",
};

interface MinCompetitionCardProps {
  min: MinCompetitionResult | null | undefined;
  form: StudyForm;
  onFormChange: (form: StudyForm) => void;
}

/** Картка мінімального конкурсу: вибір форми навчання + вуз-лідер + значення (публічні запити). */
export default function MinCompetitionCard({ min, form, onFormChange }: MinCompetitionCardProps) {
  return (
    <div className="bg-amber-100 border border-amber-300 rounded-lg p-4 mb-4 flex items-center gap-4">
      <div className="flex flex-col gap-1 shrink-0">
        <span className="font-medium text-sm">Мінімальний конкурс</span>
        <select value={form} onChange={(e) => onFormChange(e.target.value as StudyForm)}
          aria-label="Форма навчання"
          className="rounded-md border border-gray-300 px-2 py-1 bg-white text-sm">
          {Object.entries(FORM_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
        </select>
      </div>
      <div className="flex-1 text-center text-sm text-gray-700">
        {min ? (
          <>
            у вузі:{" "}
            <Link to={`/universities/${min.university.id}`} className="text-blue-700 hover:underline font-medium">
              {min.university.name}
            </Link>
          </>
        ) : (
          <span className="text-gray-500">за обраною формою навчання даних немає</span>
        )}
      </div>
      <div className="shrink-0 text-right">
        {min ? (
          <>
            <div className="text-3xl font-semibold text-gray-900">{min.value.toFixed(1)}</div>
            <div className="text-xs text-gray-500">осіб на місце</div>
          </>
        ) : (
          <div className="text-3xl font-semibold text-gray-300">—</div>
        )}
      </div>
    </div>
  );
}
