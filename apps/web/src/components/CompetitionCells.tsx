import { dash } from "../lib/ui";
import type { Competition } from "../types";

export function CompetitionHeadCells() {
  return (
    <>
      <th className="px-3 py-2 text-right">Денна</th>
      <th className="px-3 py-2 text-right">Вечірня</th>
      <th className="px-3 py-2 text-right">Заочна</th>
      <th className="px-3 py-2 text-right">Контракт, грн/рік</th>
    </>
  );
}

export function CompetitionBodyCells({
  competition,
  contractPrice,
}: {
  competition: Competition;
  contractPrice: number;
}) {
  return (
    <>
      <td className="px-3 py-2 text-right">{dash(competition.fullTime)}</td>
      <td className="px-3 py-2 text-right">{dash(competition.evening)}</td>
      <td className="px-3 py-2 text-right">{dash(competition.partTime)}</td>
      <td className="px-3 py-2 text-right">{contractPrice.toLocaleString("uk-UA")}</td>
    </>
  );
}
