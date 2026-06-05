import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";
import Modal from "./Modal";
import { ApiValidationError } from "../api";
import { inputCls } from "../lib/ui";
import type { University, UniversityInput } from "../types";

const schema = z.object({
  name: z.string().trim().min(1, "Вкажіть найменування вузу").max(200, "Не довше 200 символів"),
  address: z.string().trim().min(1, "Вкажіть адресу вузу").max(300, "Не довше 300 символів"),
});

interface UniversityFormProps {
  open: boolean;
  initial: University | null; // null — додавання, інакше — редагування
  onSubmit: (input: UniversityInput) => Promise<void>;
  onClose: () => void;
}

/** Форма додавання/редагування вузу. Enter — зберегти, Esc — скасувати. */
export default function UniversityForm({ open, initial, onSubmit, onClose }: UniversityFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<UniversityInput>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (open) reset({ name: initial?.name ?? "", address: initial?.address ?? "" });
  }, [open, initial, reset]);

  const submit = handleSubmit(async (data) => {
    try {
      await onSubmit(data);
      onClose();
    } catch (e) {
      if (e instanceof ApiValidationError) {
        for (const [field, message] of Object.entries(e.errors))
          setError(field as keyof UniversityInput, { message });
      } else {
        setError("name", {
          message: e instanceof Error ? e.message : "Не вдалося зберегти. Спробуйте ще раз.",
        });
      }
    }
  });

  const errText = "text-sm text-red-600 mt-1";

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={initial ? "Редагування вузу" : "Додавання вузу"}
      hint="Enter — зберегти · Esc — скасувати"
    >
      <form onSubmit={submit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1" htmlFor="uni-name">
            Найменування
          </label>
          <input id="uni-name" className={inputCls} autoFocus {...register("name")} />
          {errors.name && <p className={errText}>{errors.name.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium mb-1" htmlFor="uni-address">
            Адреса
          </label>
          <input id="uni-address" className={inputCls} {...register("address")} />
          {errors.address && <p className={errText}>{errors.address.message}</p>}
        </div>
        <div className="flex justify-end gap-3 pt-2">
          <button
            type="button"
            onClick={onClose}
            className="px-4 py-2 rounded-md border border-gray-300 hover:bg-gray-100"
          >
            Скасувати
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50"
          >
            Зберегти
          </button>
        </div>
      </form>
    </Modal>
  );
}
