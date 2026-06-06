import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../../api";
import type { Specialty, SpecialtyInput, UniversityInput } from "../../types";
import SpecialtyTable from "../../components/SpecialtyTable";
import RowActions from "../../components/RowActions";
import SpecialtyForm from "../../components/SpecialtyForm";
import UniversityForm from "../../components/UniversityForm";
import ConfirmDialog from "../../components/ConfirmDialog";

/** Адмін-сторінка вузу: деталі + повний CRUD його спеціальностей і самого вузу. */
export default function AdminUniversityDetailsPage() {
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
    onSuccess: () => navigate("/admin"),
  });

  if (isLoading) return <p className="text-gray-500">Завантаження…</p>;
  if (!data) return <p className="text-gray-500">Вуз не знайдено. <Link to="/admin" className="text-blue-700">До списку</Link></p>;

  return (
    <div>
      {/* Breadcrumb: «Вузи / <назва>» */}
      <nav className="text-sm text-gray-500 mb-3">
        <Link to="/admin" className="text-blue-700 hover:underline">Вузи</Link>
        <span className="mx-1">/</span>
        <span>{data.university.name}</span>
      </nav>

      <div className="flex items-start justify-between mb-6">
        <div>
          <h1 className="text-2xl font-semibold">{data.university.name}</h1>
          <p className="text-gray-500 text-sm mt-0.5">Адреса: {data.university.address}</p>
        </div>
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
        <SpecialtyTable
          specialties={data.specialties}
          renderActions={(s) => (
            <RowActions
              onEdit={() => { setEditingSpec(s); setSpecFormOpen(true); }}
              onDelete={() => setDeletingSpec(s)}
            />
          )}
        />
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
