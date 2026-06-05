import Modal from "./Modal";

interface ConfirmDialogProps {
  open: boolean;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
}

/** Діалог підтвердження незворотної дії (вимога методички про цілісність даних). */
export default function ConfirmDialog({ open, message, onConfirm, onCancel }: ConfirmDialogProps) {
  return (
    <Modal open={open} onClose={onCancel} title="Підтвердження" hint="Esc — скасувати">
      <p className="mb-6 text-gray-700">{message}</p>
      <div className="flex justify-end gap-3">
        {/* Фокус за замовчуванням Radix ставить на перший табельний елемент —
            кнопку «×» у Modal: безпечний дефолт для незворотної дії
            (Enter не видалить випадково) */}
        <button
          onClick={onCancel}
          className="px-4 py-2 rounded-md border border-gray-300 hover:bg-gray-100"
        >
          Ні
        </button>
        <button
          onClick={onConfirm}
          className="px-4 py-2 rounded-md bg-red-600 text-white hover:bg-red-700"
        >
          Так, видалити
        </button>
      </div>
    </Modal>
  );
}
