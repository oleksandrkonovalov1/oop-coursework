interface RowActionsProps {
  onEdit: () => void;
  onDelete: () => void;
}

/** Пара кнопок «Ред./Вид.» у рядку таблиці — спільна для адмін-таблиць вузів і спеціальностей. */
export default function RowActions({ onEdit, onDelete }: RowActionsProps) {
  return (
    <div className="flex gap-2">
      <button
        onClick={onEdit}
        aria-label="Редагувати"
        title="Редагувати"
        className="px-2 py-1 text-xs rounded border border-gray-300 hover:bg-gray-100"
      >
        Ред.
      </button>
      <button
        onClick={onDelete}
        aria-label="Видалити"
        title="Видалити"
        className="px-2 py-1 text-xs rounded border border-red-300 text-red-600 hover:bg-red-50"
      >
        Вид.
      </button>
    </div>
  );
}
