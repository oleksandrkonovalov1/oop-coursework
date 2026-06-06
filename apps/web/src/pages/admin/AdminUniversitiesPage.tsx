import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../../api";
import type { University, UniversityInput } from "../../types";
import UniversitiesTable from "../../components/UniversitiesTable";
import SearchInput from "../../components/SearchInput";
import RowActions from "../../components/RowActions";
import UniversityForm from "../../components/UniversityForm";
import ConfirmDialog from "../../components/ConfirmDialog";

/** Адмін-список вузів: пошук, таблиця з повним CRUD через модальні вікна. */
export default function AdminUniversitiesPage() {
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
        <UniversitiesTable
          universities={universities}
          hrefFor={(u) => `/admin/universities/${u.id}`}
          renderActions={(u) => (
            <RowActions
              onEdit={() => {
                setEditing(u);
                setFormOpen(true);
              }}
              onDelete={() => setDeleting(u)}
            />
          )}
        />
      ) : (
        <p className="text-gray-500">Нічого не знайдено. Змініть умову пошуку або додайте вуз.</p>
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
