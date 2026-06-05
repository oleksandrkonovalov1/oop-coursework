import { NavLink, Outlet } from "react-router";

/** Каркас застосунку: шапка з навігацією та область сторінок. */
export default function App() {
  const link = ({ isActive }: { isActive: boolean }) =>
    `px-3 py-2 rounded-md text-sm font-medium ${isActive ? "bg-blue-600 text-white" : "text-gray-700 hover:bg-gray-200"}`;
  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b border-gray-200">
        <nav className="mx-auto max-w-5xl flex items-center gap-2 px-4 py-3">
          <span className="font-semibold text-lg mr-4">Довідник абітурієнта</span>
          <NavLink to="/" className={link} end>Вузи</NavLink>
          <NavLink to="/specialties" className={link}>Спеціальності</NavLink>
        </nav>
      </header>
      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
    </div>
  );
}
