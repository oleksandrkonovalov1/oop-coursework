import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";
import Modal from "./Modal";
import { ApiValidationError } from "../api";
import { inputCls } from "../lib/ui";
import type { Specialty, SpecialtyInput } from "../types";

/** Порожній рядок → null («форма не ведеться»), інакше — число в межах 0–100 осіб на місце. */
const formValue = z.preprocess(
  (v) => (v === "" || v == null ? null : Number(v)),
  z.number({ message: "Введіть число" })
    .min(0, "Конкурс не може бути від'ємним")
    .max(100, "Не більше 100 осіб на місце")
    .nullable(),
);

const schema = z
  .object({
    code: z.string().trim().max(10, "Не довше 10 символів"),
    name: z.string().trim().min(1, "Вкажіть назву спеціальності").max(200, "Не довше 200 символів"),
    contractPrice: z.preprocess(
      (v) => (v === "" || v == null ? null : Number(v)),
      z.number({ message: "Введіть число" })
        .positive("Вартість має бути більшою за нуль")
        .max(1_000_000, "Не більше 1 000 000 грн/рік"),
    ),
    fullTime: formValue,
    evening: formValue,
    partTime: formValue,
  })
  .refine((d) => d.fullTime != null || d.evening != null || d.partTime != null, {
    message: "Заповніть конкурс хоча б за однією формою навчання",
    path: ["fullTime"],
  });

// КРИТИЧНО: схема з z.preprocess має різні input/output типи (input = unknown),
// тому useForm потребує ТРИ дженерики — інакше TS2322 у strict-збірці
type FormInput = z.input<typeof schema>;
type FormOutput = z.output<typeof schema>;

interface SpecialtyFormProps {
  open: boolean;
  initial: Specialty | null;
  onSubmit: (input: SpecialtyInput) => Promise<void>;
  onClose: () => void;
}

/** Форма додавання/редагування спеціальності з конкурсами за формами навчання. */
export default function SpecialtyForm({ open, initial, onSubmit, onClose }: SpecialtyFormProps) {
  const { register, handleSubmit, reset, setError, formState: { errors, isSubmitting } } =
    useForm<FormInput, unknown, FormOutput>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (open) reset({
      code: initial?.code ?? "",
      name: initial?.name ?? "",
      contractPrice: initial?.contractPrice ?? "",
      fullTime: initial?.competition.fullTime ?? "",
      evening: initial?.competition.evening ?? "",
      partTime: initial?.competition.partTime ?? "",
    });
  }, [open, initial, reset]);

  const submit = handleSubmit(async (d) => {
    try {
      await onSubmit({
        code: d.code,
        name: d.name,
        contractPrice: d.contractPrice,
        competition: { fullTime: d.fullTime, evening: d.evening, partTime: d.partTime },
      });
      onClose();
    } catch (e) {
      if (e instanceof ApiValidationError) {
        const map: Record<string, keyof FormInput> = {
          code: "code", name: "name", contractPrice: "contractPrice", competition: "fullTime",
        };
        for (const [field, message] of Object.entries(e.errors))
          setError(map[field] ?? "name", { message });
      } else {
        setError("name", { message: e instanceof Error ? e.message : "Не вдалося зберегти. Спробуйте ще раз." });
      }
    }
  });

  const errText = "text-sm text-red-600 mt-1";

  return (
    <Modal open={open} onClose={onClose}
      title={initial ? "Редагування спеціальності" : "Додавання спеціальності"}
      hint="Enter — зберегти · Esc — скасувати">
      <form onSubmit={submit} className="space-y-4">
        <div className="grid grid-cols-3 gap-3">
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="sp-code">Код</label>
            <input id="sp-code" className={inputCls} autoFocus {...register("code")} />
            {errors.code && <p className={errText}>{errors.code.message}</p>}
          </div>
          <div className="col-span-2">
            <label className="block text-sm font-medium mb-1" htmlFor="sp-name">Назва</label>
            <input id="sp-name" className={inputCls} {...register("name")} />
            {errors.name && <p className={errText}>{errors.name.message}</p>}
          </div>
        </div>
        <div>
          <label className="block text-sm font-medium mb-1" htmlFor="sp-price">Вартість контракту, грн/рік</label>
          <input id="sp-price" type="number" className={inputCls} {...register("contractPrice")} />
          {errors.contractPrice && <p className={errText}>{errors.contractPrice.message}</p>}
        </div>
        <fieldset>
          <legend className="text-sm font-medium mb-1">
            Конкурс минулого року, осіб на місце <span className="text-gray-400">(порожнє — форма не ведеться)</span>
          </legend>
          <div className="grid grid-cols-3 gap-3">
            <div>
              <label className="block text-xs text-gray-500 mb-1" htmlFor="sp-ft">Денна</label>
              <input id="sp-ft" type="number" step="0.1" className={inputCls} {...register("fullTime")} />
            </div>
            <div>
              <label className="block text-xs text-gray-500 mb-1" htmlFor="sp-ev">Вечірня</label>
              <input id="sp-ev" type="number" step="0.1" className={inputCls} {...register("evening")} />
            </div>
            <div>
              <label className="block text-xs text-gray-500 mb-1" htmlFor="sp-pt">Заочна</label>
              <input id="sp-pt" type="number" step="0.1" className={inputCls} {...register("partTime")} />
            </div>
          </div>
          {(errors.fullTime || errors.evening || errors.partTime) && (
            <p className={errText}>
              {(errors.fullTime ?? errors.evening ?? errors.partTime)?.message}
            </p>
          )}
        </fieldset>
        <div className="flex justify-end gap-3 pt-2">
          <button type="button" onClick={onClose}
            className="px-4 py-2 rounded-md border border-gray-300 hover:bg-gray-100">Скасувати</button>
          <button type="submit" disabled={isSubmitting}
            className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50">
            Зберегти
          </button>
        </div>
      </form>
    </Modal>
  );
}
