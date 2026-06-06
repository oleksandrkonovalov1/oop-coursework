import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "../../api";
import type { StudyForm } from "../../types";
import { inputCls } from "../../lib/ui";
import MinCompetitionCard from "../../components/MinCompetitionCard";
import OffersTable from "../../components/OffersTable";

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
      <h1 className="text-2xl font-bold mb-5">Спеціальності</h1>

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
            className={inputCls} />
          {priceInvalid && <p className="text-sm text-red-600 mt-1">Введіть додатне число</p>}
        </div>
      </div>

      {name === "" ? (
        <p className="text-gray-500">Оберіть спеціальність, щоб побачити пропозиції вузів.</p>
      ) : isLoading ? (
        <p className="text-gray-500">Завантаження…</p>
      ) : (
        <>
          <MinCompetitionCard min={min} form={form} onFormChange={setForm} />

          {offers && offers.length > 0 ? (
            <OffersTable offers={offers} />
          ) : (
            <p className="text-gray-500">Жоден вуз не відповідає умовам. Змініть фільтр оплати.</p>
          )}
        </>
      )}
    </div>
  );
}
