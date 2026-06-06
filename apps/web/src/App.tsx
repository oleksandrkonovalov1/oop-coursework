import { Link, NavLink, Outlet } from "react-router";
import HelpDialog from "./components/HelpDialog";
import LoadProblemBanner from "./components/LoadProblemBanner";
import { useHelpDialog } from "./lib/useHelpDialog";

/** Публічний каркас (абітурієнт): шапка з навігацією, хоткей «?», область сторінок. */
export default function App() {
  const { helpOpen, openHelp, closeHelp } = useHelpDialog();

  const link = ({ isActive }: { isActive: boolean }) =>
    `px-3 py-2 rounded-md text-sm font-medium ${isActive ? "bg-blue-600 text-white" : "text-gray-700 hover:bg-gray-200"}`;
  return (
    <div className="min-h-screen bg-gray-50">
      <LoadProblemBanner />
      <header className="bg-white border-b border-gray-200">
        <nav className="mx-auto max-w-5xl flex items-center gap-2 px-4 py-3">
          <span className="font-semibold text-lg mr-4">Довідник абітурієнта</span>
          <NavLink to="/" className={link} end>Вузи</NavLink>
          <NavLink to="/specialties" className={link}>Спеціальності</NavLink>
          <button onClick={openHelp} aria-label="Допомога" title="Допомога (?)"
            className="ml-auto w-8 h-8 rounded-full border border-gray-300 text-gray-600 hover:bg-gray-100">
            ?
          </button>
          {/* Скромне посилання до адмінки (без авторизації — лише розділення за URL) */}
          <Link to="/admin" className="text-sm text-gray-400 hover:text-gray-600 underline">
            Адміністрування
          </Link>
        </nav>
      </header>
      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
      <HelpDialog open={helpOpen} onClose={closeHelp} />
    </div>
  );
}
