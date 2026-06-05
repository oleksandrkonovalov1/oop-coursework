import { useState } from "react";
import { NavLink, Outlet } from "react-router";
import { useHotkeys } from "react-hotkeys-hook";
import { useQuery } from "@tanstack/react-query";
import { api } from "./api";
import HelpDialog from "./components/HelpDialog";

/** Каркас застосунку: шапка з навігацією, хоткей «?» для допомоги, область сторінок. */
export default function App() {
  const [helpOpen, setHelpOpen] = useState(false);
  // useKey:true матчить event.key — саме так ловиться символ «?» (Shift+/ дає key="?")
  useHotkeys("?", () => setHelpOpen(true), { useKey: true });
  const { data: status } = useQuery({ queryKey: ["status"], queryFn: api.status });

  const link = ({ isActive }: { isActive: boolean }) =>
    `px-3 py-2 rounded-md text-sm font-medium ${isActive ? "bg-blue-600 text-white" : "text-gray-700 hover:bg-gray-200"}`;
  return (
    <div className="min-h-screen bg-gray-50">
      {status?.loadProblem && (
        <div className="bg-amber-100 border-b border-amber-300 text-amber-900 text-sm px-4 py-2 text-center">
          Частину файлів даних не вдалося прочитати — відповідні колекції розпочато порожніми.
        </div>
      )}
      <header className="bg-white border-b border-gray-200">
        <nav className="mx-auto max-w-5xl flex items-center gap-2 px-4 py-3">
          <span className="font-semibold text-lg mr-4">Довідник абітурієнта</span>
          <NavLink to="/" className={link} end>Вузи</NavLink>
          <NavLink to="/specialties" className={link}>Спеціальності</NavLink>
          <button onClick={() => setHelpOpen(true)} title="Допомога (?)"
            className="ml-auto w-8 h-8 rounded-full border border-gray-300 text-gray-600 hover:bg-gray-100">
            ?
          </button>
        </nav>
      </header>
      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
      <HelpDialog open={helpOpen} onClose={() => setHelpOpen(false)} />
    </div>
  );
}
