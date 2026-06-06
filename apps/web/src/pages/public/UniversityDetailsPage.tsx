import { Link, useParams } from "react-router";
import { useQuery } from "@tanstack/react-query";
import { api } from "../../api";
import SpecialtyTable from "../../components/SpecialtyTable";

export default function UniversityDetailsPage() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading } = useQuery({
    queryKey: ["university", id],
    queryFn: () => api.getUniversity(id!),
  });

  if (isLoading) return <p className="text-gray-500">Завантаження…</p>;
  if (!data) return <p className="text-gray-500">Вуз не знайдено. <Link to="/" className="text-blue-700">До списку</Link></p>;

  return (
    <div>
      <nav className="text-sm text-gray-500 mb-3">
        <Link to="/" className="text-blue-700 hover:underline">Вузи</Link>
        <span className="mx-1">/</span>
        <span>{data.university.name}</span>
      </nav>

      <div className="mb-6">
        <h1 className="text-2xl font-semibold">{data.university.name}</h1>
        <p className="text-gray-500 text-sm mt-0.5">Адреса: {data.university.address}</p>
      </div>

      <div className="flex items-center gap-2 mb-3">
        <h2 className="text-lg font-medium">Спеціальності</h2>
        <span className="rounded-full bg-violet-200 px-2 py-0.5 text-sm font-medium text-violet-800">
          {data.specialties.length}
        </span>
      </div>

      {data.specialties.length > 0 ? (
        <SpecialtyTable specialties={data.specialties} />
      ) : (
        <p className="text-gray-500">У цього вузу ще немає спеціальностей.</p>
      )}
    </div>
  );
}
