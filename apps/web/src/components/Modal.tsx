import * as Dialog from "@radix-ui/react-dialog";
import type { ReactNode } from "react";

interface ModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  /** Рядок-підказка про клавіатуру під контентом (напр. «Enter — зберегти · Esc — скасувати») */
  hint?: string;
}

/** Модальне вікно на Radix Dialog: Esc закриває, фокус замкнено всередині. */
export default function Modal({ open, onClose, title, children, hint }: ModalProps) {
  return (
    <Dialog.Root open={open} onOpenChange={(o) => !o && onClose()}>
      <Dialog.Portal>
        <Dialog.Overlay className="fixed inset-0 bg-black/40" />
        <Dialog.Content
          aria-describedby={undefined}
          className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-white rounded-lg shadow-xl p-6 w-full max-w-md"
        >
          {/* Заголовок з кнопкою закриття «×» праворуч */}
          <div className="flex items-center justify-between mb-4">
            <Dialog.Title className="text-lg font-semibold">{title}</Dialog.Title>
            <Dialog.Close
              className="text-gray-400 hover:text-gray-600 text-xl leading-none -mr-1"
              aria-label="Закрити"
            >
              ×
            </Dialog.Close>
          </div>

          {children}

          {hint && (
            <p className="mt-3 text-xs text-gray-400 text-center">
              {hint}
            </p>
          )}
        </Dialog.Content>
      </Dialog.Portal>
    </Dialog.Root>
  );
}
