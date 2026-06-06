import type { ReactNode } from "react";
import type { Specialty } from "../types";
import { CompetitionBodyCells, CompetitionHeadCells } from "./CompetitionCells";

interface SpecialtyTableProps {
  specialties: Specialty[];
  renderActions?: (s: Specialty) => ReactNode;
}

export default function SpecialtyTable({ specialties, renderActions }: SpecialtyTableProps) {
  return (
    <table className="w-full bg-white rounded-lg shadow overflow-hidden text-sm">
      <thead className="bg-gray-100 text-left">
        <tr>
          <th className="px-3 py-2">Код</th>
          <th className="px-3 py-2">Назва</th>
          <CompetitionHeadCells />
          {renderActions && <th className="px-3 py-2 w-36">Дії</th>}
        </tr>
      </thead>
      <tbody>
        {specialties.map((s) => (
          <tr key={s.id} className="border-t border-gray-100">
            <td className="px-3 py-2">{s.code}</td>
            <td className="px-3 py-2">{s.name}</td>
            <CompetitionBodyCells competition={s.competition} contractPrice={s.contractPrice} />
            {renderActions && <td className="px-3 py-2">{renderActions(s)}</td>}
          </tr>
        ))}
      </tbody>
    </table>
  );
}
