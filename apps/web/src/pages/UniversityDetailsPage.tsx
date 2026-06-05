import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../api";
import type { Specialty, SpecialtyInput, UniversityInput } from "../types";
import SpecialtyForm from "../components/SpecialtyForm";
import UniversityForm from "../components/UniversityForm";
import ConfirmDialog from "../components/ConfirmDialog";

const dash = (v: number | null) => (v == null ? "—" : v.toFixed(1));

/** Сторінка вузу: «все щодо обраного вузу» — деталі + CRUD його спеціальностей. */
export default function UniversityDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const qc = useQueryClient();
  const [specFormOpen, setSpecFormOpen] = useState(false);
  const [editingSpec, setEditingSpec] = useState<Specialty | null>(null);
  const [deletingSpec, setDeletingSpec] = useState<Specialty | null>(null);
  const [uniFormOpen, setUniFormOpen] = useState(false);
  const [deletingUni, setDeletingUni] = useState(false);

  const { data, isLoading } = useQuery({
    queryKey: ["university", id],
    queryFn: () => api.getUniversity(id!),
  });

  const invalidate = () => {
    qc.invalidateQueries({ queryKey: ["university", id] });
    qc.invalidateQueries({ queryKey: ["universities"] });
  };

  const saveSpec = async (input: SpecialtyInput) => {
    if (editingSpec) await api.updateSpecialty(editingSpec.id, input);
    else await api.addSpecialty(id!, input);
    invalidate();
  };
  const saveUni = async (input: UniversityInput) => {
    await api.updateUniversity(id!, input);
    invalidate();
  };
  const removeSpec = useMutation({
    mutationFn: (specId: string) => api.deleteSpecialty(specId),
    onSuccess: () => { setDeletingSpec(null); invalidate(); },
  });
  const removeUni = useMutation({
    mutationFn: () => api.deleteUniversity(id!),
    onSuccess: () => navigate("/"),
  });

  if (isLoading) return <p className="text-gray-500">Завантаження…</p>;
  if (!data) return <p className="text-gray-500">Вуз не знайдено. <Link to="/" className="text-blue-700">До списку</Link></p>;

  return (
    <div>
      {/* Breadcrumb: «Вузи / <назва>» (wireframe-03 approved deviation) */}
      <nav className="text-sm text-gray-500 mb-3">
        <Link to="/" className="text-blue-700 hover:underline">Вузи</Link>
        <span className="mx-1">/</span>
        <span>{data.university.name}</span>
      </nav>

      {/* Header: назва + address сірим + ghost-кнопки праворуч */}
      <div className="flex items-start justify-between mb-6">
        <div>
          <h1 className="text-2xl font-semibold">{data.university.name}</h1>
          <p className="text-gray-500 text-sm mt-0.5">Адреса: {data.university.address}</p>
        </div>
        {/* Ghost buttons per wireframe-03 approved deviations */}
        <div className="flex gap-2 mt-1">
          <button
            onClick={() => setUniFormOpen(true)}
            className="px-3 py-1.5 text-sm rounded-md border border-gray-300 hover:bg-gray-100"
          >
            Редагувати
          </button>
          <button
            onClick={() => setDeletingUni(true)}
            className="px-3 py-1.5 text-sm rounded-md border border-red-300 text-red-600 hover:bg-red-50"
          >
            Видалити
          </button>
        </div>
      </div>

      {/* Specialties section header with badge (wireframe-03 approved deviation) */}
      <div className="flex items-center gap-3 justify-between mb-3">
        <div className="flex items-center gap-2">
          <h2 className="text-lg font-medium">Спеціальності</h2>
          <span className="rounded-full bg-violet-200 px-2 py-0.5 text-sm font-medium text-violet-800">
            {data.specialties.length}
          </span>
        </div>
        <button
          onClick={() => { setEditingSpec(null); setSpecFormOpen(true); }}
          className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 text-sm"
        >
          Додати спеціальність
        </button>
      </div>

      {data.specialties.length > 0 ? (
        <table className="w-full bg-white rounded-lg shadow overflow-hidden text-sm">
          <thead className="bg-gray-100 text-left">
            <tr>
              <th className="px-3 py-2">Код</th>
              <th className="px-3 py-2">Назва</th>
              <th className="px-3 py-2 text-right">Денна</th>
              <th className="px-3 py-2 text-right">Вечірня</th>
              <th className="px-3 py-2 text-right">Заочна</th>
              <th className="px-3 py-2 text-right">Контракт, грн/рік</th>
              <th className="px-3 py-2 w-36">Дії</th>
            </tr>
          </thead>
          <tbody>
            {data.specialties.map((s) => (
              <tr key={s.id} className="border-t border-gray-100">
                <td className="px-3 py-2">{s.code}</td>
                <td className="px-3 py-2">{s.name}</td>
                <td className="px-3 py-2 text-right">{dash(s.competition.fullTime)}</td>
                <td className="px-3 py-2 text-right">{dash(s.competition.evening)}</td>
                <td className="px-3 py-2 text-right">{dash(s.competition.partTime)}</td>
                <td className="px-3 py-2 text-right">{s.contractPrice.toLocaleString("uk-UA")}</td>
                <td className="px-3 py-2">
                  <div className="flex gap-2">
                    <button
                      onClick={() => { setEditingSpec(s); setSpecFormOpen(true); }}
                      className="px-2 py-1 text-xs rounded border border-gray-300 hover:bg-gray-100"
                    >
                      Ред.
                    </button>
                    <button
                      onClick={() => setDeletingSpec(s)}
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
        <p className="text-gray-500">У цього вузу ще немає спеціальностей.</p>
      )}

      <SpecialtyForm
        open={specFormOpen}
        initial={editingSpec}
        onSubmit={saveSpec}
        onClose={() => setSpecFormOpen(false)}
      />
      <UniversityForm
        open={uniFormOpen}
        initial={data.university}
        onSubmit={saveUni}
        onClose={() => setUniFormOpen(false)}
      />
      <ConfirmDialog
        open={deletingSpec != null}
        message={`Видалити спеціальність «${deletingSpec?.name}»?`}
        onConfirm={() => deletingSpec && removeSpec.mutate(deletingSpec.id)}
        onCancel={() => setDeletingSpec(null)}
      />
      <ConfirmDialog
        open={deletingUni}
        message={`Видалити вуз «${data.university.name}»? Разом з ним буде видалено спеціальностей: ${data.specialties.length}.`}
        onConfirm={() => removeUni.mutate()}
        onCancel={() => setDeletingUni(false)}
      />
    </div>
  );
}
