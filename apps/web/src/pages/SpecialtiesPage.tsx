import { useState } from "react";
import { Link } from "react-router";
import { useQuery } from "@tanstack/react-query";
import { api } from "../api";
import type { StudyForm } from "../types";

const FORM_LABELS: Record<StudyForm, string> = {
  FullTime: "денна", Evening: "вечірня", PartTime: "заочна",
};
const dash = (v: number | null) => (v == null ? "—" : v.toFixed(1));

/** Сторінка запитів: «все щодо обраної спеціальності», мінімальний конкурс, фільтр оплати. */
export default function SpecialtiesPage() {
  const [name, setName] = useState("");
  const [maxPriceText, setMaxPriceText] = useState("");
  const [form, setForm] = useState<StudyForm>("FullTime");
  const maxPrice = maxPriceText.trim() === "" ? null : Number(maxPriceText);
  const priceInvalid = maxPrice != null && (Number.isNaN(maxPrice) || maxPrice <= 0);

  const { data: names } = useQuery({ queryKey: ["specialty-names"], queryFn: api.specialtyNames });
  const { data: offers, isLoading } = useQuery({
    queryKey: ["offers", name, maxPrice],
    queryFn: () => api.offers(name, priceInvalid ? null : maxPrice),
    enabled: name !== "",
  });
  const { data: min } = useQuery({
    queryKey: ["min-competition", name, form],
    queryFn: () => api.minCompetition(name, form),
    enabled: name !== "",
  });

  return (
    <div>
      {/* Page header: H1 (no badge/button needed for query page) */}
      <h1 className="text-2xl font-bold mb-5">Спеціальності</h1>

      {/* Filters */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div>
          <label className="block text-sm font-medium mb-1" htmlFor="spec-select">Спеціальність</label>
          <select id="spec-select" value={name} onChange={(e) => setName(e.target.value)}
            className="w-full rounded-md border border-gray-300 px-3 py-2 bg-white">
            <option value="">— оберіть спеціальність —</option>
            {names?.map((n) => <option key={n} value={n}>{n}</option>)}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium mb-1" htmlFor="max-price">Оплата до, грн/рік</label>
          <input id="max-price" type="number" value={maxPriceText}
            onChange={(e) => setMaxPriceText(e.target.value)}
            placeholder="без обмеження"
            className="w-full rounded-md border border-gray-300 px-3 py-2" />
          {priceInvalid && <p className="text-sm text-red-600 mt-1">Введіть додатне число</p>}
        </div>
      </div>

      {name === "" ? (
        <p className="text-gray-500">Оберіть спеціальність, щоб побачити пропозиції вузів.</p>
      ) : isLoading ? (
        <p className="text-gray-500">Завантаження…</p>
      ) : (
        <>
          {/* Min competition card — wireframe-05: amber bg, label+selector left, university link center, big number right */}
          <div className="bg-amber-100 border border-amber-300 rounded-lg p-4 mb-4 flex items-center gap-4">
            <div className="flex flex-col gap-1 shrink-0">
              <span className="font-medium text-sm">Мінімальний конкурс</span>
              <select value={form} onChange={(e) => setForm(e.target.value as StudyForm)}
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

          {offers && offers.length > 0 ? (
            <table className="w-full bg-white rounded-lg shadow overflow-hidden text-sm">
              <thead className="bg-gray-100 text-left">
                <tr>
                  <th className="px-3 py-2">Вуз</th>
                  <th className="px-3 py-2 text-right">Денна</th>
                  <th className="px-3 py-2 text-right">Вечірня</th>
                  <th className="px-3 py-2 text-right">Заочна</th>
                  <th className="px-3 py-2 text-right">Контракт, грн/рік</th>
                </tr>
              </thead>
              <tbody>
                {offers.map((o) => (
                  <tr key={o.specialty.id} className="border-t border-gray-100">
                    <td className="px-3 py-2">
                      <Link to={`/universities/${o.university.id}`} className="text-blue-700 hover:underline">
                        {o.university.name}
                      </Link>
                    </td>
                    <td className="px-3 py-2 text-right">{dash(o.specialty.competition.fullTime)}</td>
                    <td className="px-3 py-2 text-right">{dash(o.specialty.competition.evening)}</td>
                    <td className="px-3 py-2 text-right">{dash(o.specialty.competition.partTime)}</td>
                    <td className="px-3 py-2 text-right">{o.specialty.contractPrice.toLocaleString("uk-UA")}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="text-gray-500">Жоден вуз не відповідає умовам. Змініть фільтр оплати.</p>
          )}
        </>
      )}
    </div>
  );
}
