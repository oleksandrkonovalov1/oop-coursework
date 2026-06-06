import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "../../api";
import UniversitiesTable from "../../components/UniversitiesTable";
import SearchInput from "../../components/SearchInput";

/** Публічний список вузів: пошук за назвою/адресою + таблиця (лише читання). */
export default function UniversitiesPage() {
  const [query, setQuery] = useState("");

  const { data: universities, isLoading } = useQuery({
    queryKey: ["universities", query],
    queryFn: () => api.searchUniversities(query),
  });

  return (
    <div>
      <div className="flex items-center gap-3 mb-5">
        <h1 className="text-2xl font-bold">Вузи</h1>
        {universities != null && (
          <span className="rounded-full bg-violet-200 px-2 py-0.5 text-sm font-medium text-violet-800">
            {universities.length}
          </span>
        )}
      </div>

      <div className="mb-4">
        <SearchInput
          value={query}
          onChange={setQuery}
          placeholder="Пошук (назва або адреса)"
          ariaLabel="Пошук вузів"
        />
      </div>

      {isLoading ? (
        <p className="text-gray-500">Завантаження…</p>
      ) : universities && universities.length > 0 ? (
        <UniversitiesTable universities={universities} hrefFor={(u) => `/universities/${u.id}`} />
      ) : (
        <p className="text-gray-500">Нічого не знайдено. Змініть умову пошуку.</p>
      )}
    </div>
  );
}
