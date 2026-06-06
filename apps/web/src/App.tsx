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
          {/* Вхід до адмінки — символом (шестерня), щоб шапка не стрибала при переході */}
          <Link to="/admin" aria-label="Адміністрування" title="Адміністрування"
            className="w-8 h-8 flex items-center justify-center rounded-full border border-gray-300 text-gray-600 hover:bg-gray-100">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
              strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4" aria-hidden="true">
              <circle cx="12" cy="12" r="3" />
              <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z" />
            </svg>
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
