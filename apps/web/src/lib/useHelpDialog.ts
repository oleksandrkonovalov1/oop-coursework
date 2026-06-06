import { useState } from "react";
import { useHotkeys } from "react-hotkeys-hook";

export function useHelpDialog() {
  const [helpOpen, setHelpOpen] = useState(false);
  useHotkeys("?", () => setHelpOpen(true), { useKey: true });
  return {
    helpOpen,
    openHelp: () => setHelpOpen(true),
    closeHelp: () => setHelpOpen(false),
  };
}
