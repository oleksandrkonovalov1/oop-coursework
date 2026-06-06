import Modal from "./Modal";

interface HelpDialogProps {
  open: boolean;
  onClose: () => void;
}

export default function HelpDialog({ open, onClose }: HelpDialogProps) {
  const rows: [string, string][] = [
    ["Enter", "Підтвердити форму (зберегти)"],
    ["Esc", "Закрити вікно / скасувати"],
    ["Tab / Shift+Tab", "Перехід між полями форми"],
    ["?", "Це вікно допомоги"],
  ];
  return (
    <Modal open={open} onClose={onClose} title="Допомога — клавіатура">
      <table className="w-full text-sm">
        <thead className="sr-only">
          <tr>
            <th scope="col">Клавіша</th>
            <th scope="col">Дія</th>
          </tr>
        </thead>
        <tbody>
          {rows.map(([key, desc]) => (
            <tr key={key} className="border-t border-gray-100">
              <td className="py-2 pr-4 font-mono font-semibold whitespace-nowrap">{key}</td>
              <td className="py-2 text-gray-700">{desc}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="flex justify-end pt-4">
        <button onClick={onClose} className="px-4 py-2 rounded-md border border-gray-300 hover:bg-gray-100">
          Закрити
        </button>
      </div>
    </Modal>
  );
}
