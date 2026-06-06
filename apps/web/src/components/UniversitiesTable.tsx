import type { ReactNode } from "react";
import { Link } from "react-router";
import type { University } from "../types";

interface UniversitiesTableProps {
  universities: University[];
  /** Будує URL сторінки вузу (публічна vs адмінська). */
  hrefFor: (u: University) => string;
  /** Опційний рендер комірки дій; якщо заданий — додається колонка «Дії» (адмінка). */
  renderActions?: (u: University) => ReactNode;
}

/** Таблиця вузів — спільна для публічного списку та адмінки; різниця лише в наявності дій. */
export default function UniversitiesTable({ universities, hrefFor, renderActions }: UniversitiesTableProps) {
  return (
    <table className="w-full bg-white rounded-lg shadow overflow-hidden">
      <thead className="bg-gray-100 text-left text-sm">
        <tr>
          <th className="px-4 py-3">Найменування</th>
          <th className="px-4 py-3">Адреса</th>
          {renderActions && <th className="px-4 py-3 w-32">Дії</th>}
        </tr>
      </thead>
      <tbody>
        {universities.map((u) => (
          <tr key={u.id} className="border-t border-gray-100">
            <td className="px-4 py-3">
              <Link to={hrefFor(u)} className="text-blue-700 hover:underline">{u.name}</Link>
            </td>
            <td className="px-4 py-3 text-gray-600">{u.address}</td>
            {renderActions && <td className="px-4 py-3">{renderActions(u)}</td>}
          </tr>
        ))}
      </tbody>
    </table>
  );
}
