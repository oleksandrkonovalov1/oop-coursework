import { Link } from "react-router";
import type { SpecialtyOffer } from "../types";
import { CompetitionBodyCells, CompetitionHeadCells } from "./CompetitionCells";

/** Таблиця пропозицій вузів за обраною спеціальністю (публічні запити, лише читання). */
export default function OffersTable({ offers }: { offers: SpecialtyOffer[] }) {
  return (
    <table className="w-full bg-white rounded-lg shadow overflow-hidden text-sm">
      <thead className="bg-gray-100 text-left">
        <tr>
          <th className="px-3 py-2">Вуз</th>
          <CompetitionHeadCells />
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
            <CompetitionBodyCells competition={o.specialty.competition} contractPrice={o.specialty.contractPrice} />
          </tr>
        ))}
      </tbody>
    </table>
  );
}
