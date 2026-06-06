import { useState } from "react";
import { useHotkeys } from "react-hotkeys-hook";

/** Стан і хоткей «?» для вікна допомоги — спільні для публічного й адмін-каркасів. */
export function useHelpDialog() {
  const [helpOpen, setHelpOpen] = useState(false);
  // useKey:true матчить event.key — саме так ловиться символ «?» (Shift+/ дає key="?")
  useHotkeys("?", () => setHelpOpen(true), { useKey: true });
  return {
    helpOpen,
    openHelp: () => setHelpOpen(true),
    closeHelp: () => setHelpOpen(false),
  };
}
