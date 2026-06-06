import { useQuery } from "@tanstack/react-query";
import { api } from "../api";

/** Банер про проблеми читання файлів даних на бекенді — спільний для публічного й адмін-каркасів. */
export default function LoadProblemBanner() {
  const { data: status } = useQuery({ queryKey: ["status"], queryFn: api.status });
  if (!status?.loadProblem) return null;
  return (
    <div role="status" className="bg-amber-100 border-b border-amber-300 text-amber-900 text-sm px-4 py-2 text-center">
      Частину файлів даних не вдалося прочитати — відповідні колекції розпочато порожніми.
    </div>
  );
}
