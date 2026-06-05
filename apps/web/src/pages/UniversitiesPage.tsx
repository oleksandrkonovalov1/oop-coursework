import { useState } from "react";
import { Link } from "react-router";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../api";
import type { University, UniversityInput } from "../types";
import UniversityForm from "../components/UniversityForm";
import ConfirmDialog from "../components/ConfirmDialog";
import { inputCls } from "../lib/ui";

/** Сторінка «Вузи»: пошук за назвою/адресою, таблиця, CRUD через модальні вікна. */
export default function UniversitiesPage() {
  const [query, setQuery] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<University | null>(null);
  const [deleting, setDeleting] = useState<University | null>(null);
  const qc = useQueryClient();

  const { data: universities, isLoading } = useQuery({
    queryKey: ["universities", query],
    queryFn: () => api.searchUniversities(query),
  });
  const { data: details } = useQuery({
    queryKey: ["university", deleting?.id],
    queryFn: () => api.getUniversity(deleting!.id),
    enabled: deleting != null,
  });

  const invalidate = () => qc.invalidateQueries({ queryKey: ["universities"] });
  const save = async (input: UniversityInput) => {
    if (editing) await api.updateUniversity(editing.id, input);
    else await api.addUniversity(input);
    invalidate();
  };
  const remove = useMutation({
    mutationFn: (id: string) => api.deleteUniversity(id),
    onSuccess: () => {
      setDeleting(null);
      invalidate();
    },
  });

  return (
    <div>
      {/* Page header: H1 + badge + primary button (wireframe-01) */}
      <div className="flex items-center gap-3 mb-5">
        <h1 className="text-2xl font-bold">Вузи</h1>
        {universities != null && (
          <span className="rounded-full bg-violet-200 px-2 py-0.5 text-sm font-medium text-violet-800">
            {universities.length}
          </span>
        )}
        <button
          onClick={() => {
            setEditing(null);
            setFormOpen(true);
          }}
          className="ml-auto px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 text-sm"
        >
          Додати вуз
        </button>
      </div>

      {/* Search */}
      <div className="mb-4">
        <input
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Пошук (назва або адреса)"
          aria-label="Пошук вузів"
          className={inputCls}
        />
      </div>

      {isLoading ? (
        <p className="text-gray-500">Завантаження…</p>
      ) : universities && universities.length > 0 ? (
        <table className="w-full bg-white rounded-lg shadow overflow-hidden">
          <thead className="bg-gray-100 text-left text-sm">
            <tr>
              <th className="px-4 py-3">Найменування</th>
              <th className="px-4 py-3">Адреса</th>
              <th className="px-4 py-3 w-32">Дії</th>
            </tr>
          </thead>
          <tbody>
            {universities.map((u) => (
              <tr key={u.id} className="border-t border-gray-100">
                <td className="px-4 py-3">
                  <Link to={`/universities/${u.id}`} className="text-blue-700 hover:underline">
                    {u.name}
                  </Link>
                </td>
                <td className="px-4 py-3 text-gray-600">{u.address}</td>
                {/* Small bordered buttons per wireframe-02 approved deviations */}
                <td className="px-4 py-3">
                  <div className="flex gap-2">
                    <button
                      onClick={() => {
                        setEditing(u);
                        setFormOpen(true);
                      }}
                      aria-label="Редагувати"
                      title="Редагувати"
                      className="px-2 py-1 text-xs rounded border border-gray-300 hover:bg-gray-100"
                    >
                      Ред.
                    </button>
                    <button
                      onClick={() => setDeleting(u)}
                      aria-label="Видалити"
                      title="Видалити"
                      className="px-2 py-1 text-xs rounded border border-red-300 text-red-600 hover:bg-red-50"
                    >
                      Вид.
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p className="text-gray-500">
          Нічого не знайдено. Змініть умову пошуку або додайте вуз.
        </p>
      )}

      <UniversityForm
        open={formOpen}
        initial={editing}
        onSubmit={save}
        onClose={() => setFormOpen(false)}
      />
      <ConfirmDialog
        open={deleting != null}
        message={`Видалити вуз «${deleting?.name}»? Разом з ним буде видалено спеціальностей: ${details?.specialties.length ?? "…"}.`}
        onConfirm={() => deleting && remove.mutate(deleting.id)}
        onCancel={() => setDeleting(null)}
      />
    </div>
  );
}
