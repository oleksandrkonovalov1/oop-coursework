import { Link, NavLink, Outlet } from "react-router";
import HelpDialog from "./components/HelpDialog";
import LoadProblemBanner from "./components/LoadProblemBanner";
import { useHelpDialog } from "./lib/useHelpDialog";

/** Адмін-каркас: візуально відмінна (темна) шапка з бейджем, повний CRUD, без авторизації. */
export default function AdminApp() {
  const { helpOpen, openHelp, closeHelp } = useHelpDialog();

  const link = ({ isActive }: { isActive: boolean }) =>
    `px-3 py-2 rounded-md text-sm font-medium ${isActive ? "bg-blue-500 text-white" : "text-gray-300 hover:bg-gray-700"}`;
  return (
    <div className="min-h-screen bg-gray-50">
      <LoadProblemBanner />
      <header className="bg-gray-900 text-white">
        <nav className="mx-auto max-w-5xl flex items-center gap-2 px-4 py-3">
          <span className="font-semibold text-lg mr-2">Довідник абітурієнта</span>
          <span className="rounded bg-amber-400 text-gray-900 text-xs font-bold px-2 py-0.5 mr-4">
            Адміністрування
          </span>
          <NavLink to="/admin" className={link} end>Вузи</NavLink>
          <button onClick={openHelp} aria-label="Допомога" title="Допомога (?)"
            className="ml-auto w-8 h-8 rounded-full border border-gray-500 text-gray-300 hover:bg-gray-700">
            ?
          </button>
          {/* Повернення на публічний сайт */}
          <Link to="/" className="text-sm text-gray-300 hover:text-white underline">
            ← Публічний сайт
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
